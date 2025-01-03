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
	public partial class PermissionListViewModel(IPermissionService permissionService, ICommonServices commonServices) : GenericListViewModel<PermissionDto>(commonServices)
	{
		public IPermissionService PermissionService { get; } = permissionService;

		public PermissionListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(PermissionListArgs args)
		{
			ViewModelArgs = args ?? PermissionListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			StartStatusMessage("Loading permissions...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Permissions loaded");
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
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
				StatusError($"Error loading Permissions: {ex.Message}");
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

		private async Task<IList<PermissionDto>> GetItemsAsync()
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
				await NavigationService.CreateNewViewAsync<PermissionDetailsViewModel>(new PermissionDetailsArgs { PermissionID = SelectedItem.PermissionId });
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
			StartStatusMessage("Loading permissions...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Permissions loaded");
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected permissions?", "Delete", "Cancel"))
			{
				bool success = false;
				int count = 0;
				try
				{
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage($"Deleting {count} permissions...");
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage($"Deleting {count} permissions...");
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					StatusError($"Error deleting {count} Permissions: {ex.Message}");
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
						EndStatusMessage($"{count} permissions deleted", LogType.Warning);
					}
				}
				else
				{
					StatusError("Delete not allowed");
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<PermissionDto> models)
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

			List<PermissionDto> models = [];
			foreach (var range in ranges)
			{
				var permissions = await PermissionService.GetPermissionsAsync(range.Index, range.Length, request);
				var disabledPermission = permissions.FirstOrDefault(f => !f.IsEnabled);
				if (disabledPermission != null)
				{
					await DialogService.ShowAsync("Delete not allowed", new ArgumentException($"Deselect the {disabledPermission.DisplayName} permission!"));
					return false;
				}
				models.AddRange(permissions);
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

		private void LogWarning(PermissionDto model)
		{
			LogWarning("Permission", "Delete", "Permission deleted", $"Permission {model.PermissionId} '{model.FullName}' was deleted.");
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
