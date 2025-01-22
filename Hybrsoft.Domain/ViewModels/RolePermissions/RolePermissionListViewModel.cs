using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
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
				StartStatusMessage("Loading role permissions...");
				if (await RefreshAsync())
				{
					EndStatusMessage("RolePermissions loaded");
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
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
				StatusError($"Error loading Role permissions: {ex.Message}");
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
				await NavigationService.CreateNewViewAsync<RolePermissionDetailsViewModel>(new RolePermissionDetailsArgs { RolePermissionId = SelectedItem.RolePermissionId, RoleId = SelectedItem.RoleId });
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
			StartStatusMessage("Loading role permissions...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Role permissions loaded");
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected role permissions?", "Ok", "Cancel"))
			{
				int count = 0;
				try
				{
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage($"Deleting {count} role permissions...");
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage($"Deleting {count} role permissions...");
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					StatusError($"Error deleting {count} role permissions: {ex.Message}");
					LogException("RolePermissions", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					EndStatusMessage($"{count} role permissions deleted", LogType.Warning);
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
				var rolePermissions = await RolePermissionService.GetRolePermissionsAsync(range.Index, range.Length, request);
				models.AddRange(rolePermissions);
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
				request.Where = (r) => r.RoleId == ViewModelArgs.RoleId;
			}
			return request;
		}

		private void LogWarning(RolePermissionDto model)
		{
			LogWarning("RolePermission", "Delete", "Role Permission deleted", $"Role Permission #{model.RoleId}, '{model.Permission.DisplayName}' was deleted.");
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
