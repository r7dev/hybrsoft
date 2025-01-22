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
	public partial class RoleListViewModel(IRoleService roleService, ICommonServices commonServices) : GenericListViewModel<RoleDto>(commonServices)
	{
		public IRoleService RoleService { get; } = roleService;

		public RoleListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RoleListArgs args)
		{
			ViewModelArgs = args ?? RoleListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			StartStatusMessage("Loading roles...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Roles loaded");
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
		}
		public void Subscribe()
		{
			MessageService.Subscribe<RoleListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public RoleListArgs CreateArgs()
		{
			return new RoleListArgs
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
				StatusError($"Error loading Roles: {ex.Message}");
				LogException("Roles", "Refresh", ex);
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

		private async Task<IList<RoleDto>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Role> request = BuildDataRequest();
				return await RoleService.GetRolesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<RoleDetailsViewModel>(new RoleDetailsArgs { RoleID = SelectedItem.RoleId });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<RoleDetailsViewModel>(new RoleDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<RoleDetailsViewModel>(new RoleDetailsArgs());
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			StartStatusMessage("Loading roles...");
			if (await RefreshAsync())
			{
				EndStatusMessage("Roles loaded");
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected roles?", "Delete", "Cancel"))
			{
				bool success = false;
				int count = 0;
				try
				{
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage($"Deleting {count} roles...");
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage($"Deleting {count} roles...");
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					StatusError($"Error deleting {count} Roles: {ex.Message}");
					LogException("Roles", "Delete", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						EndStatusMessage($"{count} roles deleted", LogType.Warning);
					}
				}
				else
				{
					StatusError("Delete not allowed");
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<RoleDto> models)
		{
			foreach (var model in models)
			{
				await RoleService.DeleteRoleAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Role> request = BuildDataRequest();

			List<RoleDto> models = [];
			foreach (var range in ranges)
			{
				var roles = await RoleService.GetRolesAsync(range.Index, range.Length, request);
				models.AddRange(roles);
			}
			foreach (var range in ranges.Reverse())
			{
				await RoleService.DeleteRoleRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Role> BuildDataRequest()
		{
			return new DataRequest<Role>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(RoleDto model)
		{
			LogWarning("Role", "Delete", "Role deleted", $"Role {model.RoleId} '{model.Name}' was deleted.");
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
