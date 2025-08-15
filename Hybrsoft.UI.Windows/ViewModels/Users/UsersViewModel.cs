using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class UsersViewModel : ViewModelBase
	{
		public UsersViewModel(IUserService userService, IUserRoleService userRoleService, ICommonServices commonServices) : base(commonServices)
		{
			UserService = userService;
			UserList = new UserListViewModel(UserService, commonServices);
			UserDetails = new UserDetailsViewModel(UserService, commonServices);
			UserRoleList = new UserRoleListViewModel(userRoleService, commonServices);
		}

		public IUserService UserService { get; }

		public UserListViewModel UserList { get; set; }

		public UserDetailsViewModel UserDetails { get; set; }

		public UserRoleListViewModel UserRoleList { get; set; }

		public async Task LoadAsync(UserListArgs args)
		{
			await UserList.LoadAsync(args);
		}

		public void Unload()
		{
			UserDetails.CancelEdit();
			UserList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<UserListViewModel>(this, OnMessage);
			UserList.Subscribe();
			UserDetails.Subscribe();
			UserRoleList.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			UserList.Unsubscribe();
			UserDetails.Unsubscribe();
			UserRoleList.Unsubscribe();
		}

		private async void OnMessage(UserListViewModel viewModel, string message, object args)
		{
			if (viewModel == UserList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (UserDetails.IsEditMode)
			{
				StatusReady();
				UserDetails.CancelEdit();
			}
			UserRoleList.IsMultipleSelection = false;
			var selected = UserList.SelectedItem;
			if (!UserList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
					await PopulateUserRoles(selected);
				}
			}
			UserDetails.Item = selected;
		}

		private async Task PopulateDetails(UserModel selected)
		{
			try
			{
				var model = await UserService.GetUserAsync(selected.UserID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Users", "Load Details", ex);
			}
		}

		private async Task PopulateUserRoles(UserModel selectedItem)
		{
			try
			{
				if (selectedItem != null)
				{
					await UserRoleList.LoadAsync(new UserRoleListArgs { UserId = selectedItem.UserID }, silent: true);
				}
			}
			catch (Exception ex)
			{
				LogException("Users", "Load UserRoles", ex);
			}
		}
	}
}
