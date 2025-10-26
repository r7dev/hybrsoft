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
	public partial class RolePermissionListViewModel(IRolePermissionService rolePermissionService, ICommonServices commonServices) : GenericListViewModel<RolePermissionModel>(commonServices)
	{
		public IRolePermissionService RolePermissionService { get; } = rolePermissionService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.InfoMessages, "LoadingRolePermissions");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.InfoMessages, "RolePermissionsLoaded");

		public RolePermissionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RolePermissionListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? RolePermissionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			if (silent)
			{
				await RefreshAsync();
			}
			else
			{
				await RefreshWithStatusAsync();
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
			MessageService.Subscribe<RolePermissionListViewModel>(this, OnMessage);
			MessageService.Subscribe<RolePermissionDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public RolePermissionListArgs CreateArgs()
		{
			return new RolePermissionListArgs
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys,
				RoleId = ViewModelArgs.RoleId
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
				string message = ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.Errors, "ErrorLoadingRolePermissions0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("RolePermissions", "Refresh", ex);
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

		private async Task<IList<RolePermissionModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<RolePermission> request = BuildDataRequest();
				return await RolePermissionService.GetRolePermissionsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<RolePermissionDetailsViewModel>(new RolePermissionDetailsArgs { RolePermissionId = SelectedItem.RolePermissionID, RoleId = SelectedItem.RoleID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<RolePermissionDetailsViewModel>(new RolePermissionDetailsArgs { RoleId = ViewModelArgs.RoleId });
			}
			else
			{
				NavigationService.Navigate<RolePermissionDetailsViewModel>(new RolePermissionDetailsArgs { RoleId = ViewModelArgs.RoleId });
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
			string content = ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedRolePermissions");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.InfoMessages, "Deleting0RolePermissions");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
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
					string message = ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.Errors, "ErrorDeleting0RolePermissions1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("RolePermissions", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<RolePermissionListViewModel>(ResourceFiles.InfoMessages, "0RolePermissionsDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<RolePermissionModel> models)
		{
			foreach (var model in models)
			{
				await RolePermissionService.DeleteRolePermissionAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<RolePermission> request = BuildDataRequest();

			List<RolePermissionModel> models = [];
			foreach (var range in ranges)
			{
				var items = await RolePermissionService.GetRolePermissionsAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await RolePermissionService.DeleteRolePermissionRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<RolePermission> BuildDataRequest()
		{
			var request = new DataRequest<RolePermission>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
			if (ViewModelArgs.RoleId > 0)
			{
				request.Where = (r) => r.RoleID == ViewModelArgs.RoleId;
			}
			return request;
		}

		private void LogWarning(RolePermissionModel model)
		{
			LogWarning("RolePermission", "Delete", "Role Permission deleted", $"Role Permission #{model.RoleID}, '{model.Permission.DisplayName}' was deleted.");
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemChanged":
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
