using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class RolePermissionListViewModel(IRolePermissionService rolePermissionService, ICommonServices commonServices) : GenericListViewModel<RolePermissionDto>(commonServices)
	{
		public IRolePermissionService RolePermissionService { get; } = rolePermissionService;

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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RolePermissionListViewModel), "_LoadingRolePermissions"));
				StartStatusMessage(startMessage);
				if (await RefreshAsync())
				{
					string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RolePermissionListViewModel), "_RolePermissionsLoaded"));
					EndStatusMessage(endMessage);
				}
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc,
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
				string resourceKey = string.Concat(nameof(RolePermissionListViewModel), "_ErrorLoadingRolePermissions0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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

		private async Task<IList<RolePermissionDto>> GetItemsAsync()
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RolePermissionListViewModel), "_LoadingRolePermissions"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RolePermissionListViewModel), "_RolePermissionsLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(RolePermissionListViewModel), "_AreYouSureYouWantToDeleteSelectedRolePermissions"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(RolePermissionListViewModel), "_Deleting0RolePermissions");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
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
					string resourceKey = string.Concat(nameof(RolePermissionListViewModel), "_ErrorDeleting0RolePermissions1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("RolePermissions", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(RolePermissionListViewModel), "_0RolePermissionsDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage(message, LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<RolePermissionDto> models)
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

			List<RolePermissionDto> models = [];
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
			if (ViewModelArgs.RoleId > 0)
			{
				request.Where = (r) => r.RoleID == ViewModelArgs.RoleId;
			}
			return request;
		}

		private void LogWarning(RolePermissionDto model)
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
