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
	public partial class UserRoleListViewModel(IUserRoleService userRoleService, ICommonServices commonServices) : GenericListViewModel<UserRoleDto>(commonServices)
	{
		public IUserRoleService UserRoleService { get; } = userRoleService;

		public UserRoleListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserRoleListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? UserRoleListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			if (silent)
			{
				await RefreshAsync();
			}
			else
			{
				StartStatusMessage("Loading user roles...");
				if (await RefreshAsync())
				{
					EndStatusMessage("UserRoles loaded");
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<UserRoleListViewModel>(this, OnMessage);
			MessageService.Subscribe<UserRoleDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public UserRoleListArgs CreateArgs()
		{
			return new UserRoleListArgs
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc,
				UserId = ViewModelArgs.UserId
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
				StatusError($"Error loading User roles: {ex.Message}");
				LogException("UserRoles", "Refresh", ex);
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

		private async Task<IList<UserRoleDto>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<UserRole> request = BuildDataRequest();
				return await UserRoleService.GetUserRolesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserRoleId = SelectedItem.UserRoleId, UserId = SelectedItem.UserId });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserId = ViewModelArgs.UserId });
			}
			else
			{
				NavigationService.Navigate<UserRoleDetailsViewModel>(new UserRoleDetailsArgs { UserId = ViewModelArgs.UserId });
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			StartStatusMessage("Loading user roles...");
			if (await RefreshAsync())
			{
				EndStatusMessage("User roles loaded");
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			if (await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected user roles?", "Ok", "Cancel"))
			{
				int count = 0;
				try
				{
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage($"Deleting {count} user roles...");
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage($"Deleting {count} user roles...");
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					StatusError($"Error deleting {count} user roles: {ex.Message}");
					LogException("UserRoles", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					EndStatusMessage($"{count} user roles deleted", LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<UserRoleDto> models)
		{
			foreach (var model in models)
			{
				await UserRoleService.DeleteUserRoleAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<UserRole> request = BuildDataRequest();

			List<UserRoleDto> models = [];
			foreach (var range in ranges)
			{
				var userRoles = await UserRoleService.GetUserRolesAsync(range.Index, range.Length, request);
				models.AddRange(userRoles);
			}
			foreach (var range in ranges.Reverse())
			{
				await UserRoleService.DeleteUserRoleRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<UserRole> BuildDataRequest()
		{
			var request = new DataRequest<UserRole>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
			if (ViewModelArgs.UserId > 0)
			{
				request.Where = (r) => r.UserId == ViewModelArgs.UserId;
			}
			return request;
		}

		private void LogWarning(UserRoleDto model)
		{
			LogWarning("UserRole", "Delete", "User Role deleted", $"User Role #{model.UserId}, '{model.Role.Name}' was deleted.");
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
