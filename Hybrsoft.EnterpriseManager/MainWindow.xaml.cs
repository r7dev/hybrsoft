using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure; //Used with SKIP_LOGIN
using Hybrsoft.EnterpriseManager.Configuration; //Used with SKIP_LOGIN
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Services.Infrastructure;
using Hybrsoft.EnterpriseManager.Tools;
using Hybrsoft.EnterpriseManager.Views;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow(IActivatedEventArgs args)
		{
			this.InitializeComponent();
			((App)Application.Current).MainWindow = this;
			((App)Application.Current).CurrentView = this;
			this.Title = AppInfo.Current.DisplayInfo.DisplayName;
			ThemeExtensions.TrySetMicaBackdrop(this, true);

			Navigate(args);
		}

		private async void Navigate(IActivatedEventArgs args)
		{
			var activationInfo = ActivationService.GetActivationInfo(args);
			var shellArgs = new ShellArgs
			{
				ViewModel = activationInfo.EntryViewModel,
				Parameter = activationInfo.EntryArgs,
				UserInfo = await TryGetUserInfoAsync(args as IActivatedEventArgsWithUser)
			};
#if SKIP_LOGIN
			rootFrame.Navigate(typeof(MainShellView), shellArgs);
			var loginService = ServiceLocator.Current.GetService<ILoginService>();
			loginService.IsAuthenticated = true;
#else
			rootFrame.Navigate(typeof(LoginView), shellArgs);
#endif
		}

		private static async Task<UserInfo> TryGetUserInfoAsync(IActivatedEventArgsWithUser argsWithUser)
		{
			if (argsWithUser != null)
			{
				var user = argsWithUser.User;
				var userInfo = new UserInfo
				{
					AccountName = await user.GetPropertyAsync(KnownUserProperties.AccountName) as String,
					FirstName = await user.GetPropertyAsync(KnownUserProperties.FirstName) as String,
					LastName = await user.GetPropertyAsync(KnownUserProperties.LastName) as String
				};
				if (!userInfo.IsEmpty)
				{
					if (String.IsNullOrEmpty(userInfo.AccountName))
					{
						userInfo.AccountName = $"{userInfo.FirstName} {userInfo.LastName}";
					}
					var pictureStream = await user.GetPictureAsync(UserPictureSize.Size64x64);
					if (pictureStream != null)
					{
						userInfo.PictureSource = await BitmapTools.LoadBitmapAsync(pictureStream);
					}
					return userInfo;
				}
			}
			return UserInfo.Default;
		}
	}
}
