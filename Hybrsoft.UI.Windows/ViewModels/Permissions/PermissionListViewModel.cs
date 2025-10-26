using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
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

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<PermissionListViewModel>(ResourceFiles.InfoMessages, "LoadingPermissions");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<PermissionListViewModel>(ResourceFiles.InfoMessages, "PermissionsLoaded");
		public string Prefix => ResourceService.GetString<PermissionListViewModel>(ResourceFiles.UI, "Prefix");

		public PermissionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(PermissionListArgs args)
		{
			ViewModelArgs = args ?? PermissionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			await RefreshWithStatusAsync();
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
				OrderBys = ViewModelArgs.OrderBys
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
				string title = ResourceService.GetString(ResourceFiles.Errors, "LoadFailed");
				string message = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.Errors, "ErrorLoadingPermissions0");
				StatusError(title, string.Format(message, ex.Message));
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
			await RefreshWithStatusAsync();
		}

		private async Task<bool> RefreshWithStatusAsync()
		{
			StartStatusMessage(StartTitle, StartMessage);
			bool isOk = await RefreshAsync();
			if (isOk)
			{
				EndStatusMessage(EndTitle, EndMessage);
			}
			return isOk;
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string dialogTitle = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedPermissions");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.InfoMessages, "Deleting0Permissions");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Permissions1");
					StatusError(title, string.Format(message, count, ex.Message));
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
						string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
						string message = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.InfoMessages, "0PermissionsDeleted");
						EndStatusMessage(endTitle, string.Format(message, count), LogType.Warning);
					}
				}
				else
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString(ResourceFiles.Errors, "DeleteNotAllowed");
					StatusError(title, message);
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
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeleteNotAllowed");
					string message = ResourceService.GetString<PermissionListViewModel>(ResourceFiles.Errors, "DeselectThe0Permission");
					await DialogService.ShowAsync(title, string.Format(message, item.DisplayName));
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
				OrderBys = ViewModelArgs.OrderBys
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
