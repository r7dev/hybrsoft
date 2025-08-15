using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class UserDetailsWithRolesViewModel(IUserService userService, IUserRoleService userRoleService, ISettingsService settingsService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ISettingsService SettingsService { get; } = settingsService;
		public char PasswordChar { get => SettingsService.PasswordChar; }

		public UserDetailsViewModel UserDetails { get; set; } = new UserDetailsViewModel(userService, commonServices);
		public UserRoleListViewModel UserRoleList { get; set; } = new UserRoleListViewModel(userRoleService, commonServices);

		public async Task LoadAsync(UserDetailsArgs args)
		{
			await UserDetails.LoadAsync(args);

			long userId = args?.UserID ?? 0;
			if (userId > 0)
			{
				await UserRoleList.LoadAsync(new UserRoleListArgs { UserId = args.UserID });
			}
			else
			{
				await UserRoleList.LoadAsync(new UserRoleListArgs { IsEmpty = true }, silent: true);
			}
		}
		public void Unload()
		{
			UserDetails.CancelEdit();
			UserDetails.Unload();
			UserRoleList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<UserDetailsViewModel, UserModel>(this, OnMessage);
			UserDetails.Subscribe();
			UserRoleList.Subscribe();
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			UserDetails.Unsubscribe();
			UserRoleList.Unsubscribe();
		}

		private async void OnMessage(UserDetailsViewModel viewModel, string message, UserModel user)
		{
			if (viewModel == UserDetails && (message == "NewItemSaved" || message == "ItemChanged"))
			{
				await UserRoleList.LoadAsync(new UserRoleListArgs { UserId = user.UserID });
			}
		}
	}
}
