using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class PermissionListViewModel(IPermissionService permissionService, ICommonServices commonServices) : GenericListViewModel<PermissionModel>(commonServices)
	{
		public IPermissionService PermissionService { get; } = permissionService;

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), $"{nameof(PermissionListViewModel)}_Prefix");

		public PermissionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(PermissionListArgs args)
		{
			ViewModelArgs = args ?? PermissionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(PermissionListViewModel)}_LoadingPermissions");
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(PermissionListViewModel)}_PermissionsLoaded");
				EndStatusMessage(endMessage);
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;

			// Release heavy collections.
			(Items as IDisposable)?.Dispose();
			Items = null;
			SelectedItems = null;
			SelectedIndexRanges = null;
		}
		public void Subscribe()
		{
			MessageService.Subscribe<PermissionListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public PermissionListArgs CreateArgs()
		{
			return new PermissionListArgs
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		public async Task<bool> RefreshAsync()
		{
			bool isOk = true;

			Items = null;
			ItemsCount = 0;
			SelectedItem = null;

			try
			{
				Items = await GetItemsAsync();
			}
			catch (Exception ex)
			{
				Items = [];
				string resourceKey = $"{nameof(PermissionListViewModel)}_ErrorLoadingPermissions0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Permissions", "Refresh", ex);
				isOk = false;
			}

			ItemsCount = Items.Count;
			if (!IsMultipleSelection)
			{
				SelectedItem = Items.FirstOrDefault();
			}
			NotifyPropertyChanged(nameof(Title));

			return isOk;
		}

		private async Task<IList<PermissionModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Permission> request = BuildDataRequest();
				return await PermissionService.GetPermissionsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<PermissionDetailsViewModel>(new PermissionDetailsArgs { PermissionID = SelectedItem.PermissionID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<PermissionDetailsViewModel>(new PermissionDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<PermissionDetailsViewModel>(new PermissionDetailsArgs());
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(PermissionListViewModel)}_LoadingPermissions");
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(PermissionListViewModel)}_PermissionsLoaded");
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), $"{nameof(PermissionListViewModel)}_AreYouSureYouWantToDeleteSelectedPermissions");
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string resourceKey = $"{nameof(PermissionListViewModel)}_Deleting0Permissions";
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string resourceKey = $"{nameof(PermissionListViewModel)}_ErrorDeleting0Permissions1";
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("Permissions", "Delete", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						string resourceKey = $"{nameof(PermissionListViewModel)}_0PermissionsDeleted";
						string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
						string message = string.Format(resourceValue, count);
						EndStatusMessage(message, LogType.Warning);
					}
				}
				else
				{
					string message = ResourceService.GetString(nameof(ResourceFiles.Errors), "DeleteNotAllowed");
					StatusError(message);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<PermissionModel> models)
		{
			foreach (var model in models)
			{
				await PermissionService.DeletePermissionAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Permission> request = BuildDataRequest();

			List<PermissionModel> models = [];
			foreach (var range in ranges)
			{
				var items = await PermissionService.GetPermissionsAsync(range.Index, range.Length, request);
				var item = items.FirstOrDefault(f => !f.IsEnabled);
				if (item != null)
				{
					string title = ResourceService.GetString(nameof(ResourceFiles.Errors), "DeleteNotAllowed");
					string resourceKey = $"{nameof(PermissionListViewModel)}_DeselectThe0Permission";
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, item.DisplayName);
					await DialogService.ShowAsync(title, new ArgumentException(message));
					return false;
				}
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await PermissionService.DeletePermissionRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Permission> BuildDataRequest()
		{
			return new DataRequest<Permission>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(PermissionModel model)
		{
			LogWarning("Permission", "Delete", "Permission deleted", $"Permission {model.PermissionID} '{model.FullName}' was deleted.");
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemDeleted":
				case "ItemsDeleted":
				case "ItemRangesDeleted":
					await ContextService.RunAsync(async () =>
					{
						await RefreshAsync();
					});
					break;
			}
		}
	}
}
