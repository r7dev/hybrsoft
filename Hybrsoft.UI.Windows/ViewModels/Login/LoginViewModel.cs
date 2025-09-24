using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LoginViewModel(
		ILoginService loginService,
		ISettingsService settingsService,
		ILicenseService licenseService,
		INetworkService networkService,
		ILookupTables lookupTables,
		IAuthorizationService authorizationService,
		ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ILoginService LoginService { get; } = loginService;
		public ISettingsService SettingsService { get; } = settingsService;
		public ILicenseService LicenseService { get; } = licenseService;
		public INetworkService NetworkService { get; } = networkService;
		private readonly ILookupTables _lookupTables = lookupTables;
		private readonly IAuthorizationService _authorizationService = authorizationService;

		private ShellArgs ViewModelArgs { get; set; }

		private bool _isBusy = false;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { Set(ref _isBusy, value); }
		}

		private bool _isLoginWithPassword = false;
		public bool IsLoginWithPassword
		{
			get { return _isLoginWithPassword; }
			set { Set(ref _isLoginWithPassword, value); }
		}

		private bool _isLoginWithWindowsHello = false;
		public bool IsLoginWithWindowsHello
		{
			get { return _isLoginWithWindowsHello; }
			set { Set(ref _isLoginWithWindowsHello, value); }
		}

		private string _userName = null;
		public string UserName
		{
			get { return _userName; }
			set { Set(ref _userName, value); }
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set { Set(ref _password, value); }
		}

		private bool _hasSecurityAdministration;
		private bool _isLicenseValid;

		public ICommand ShowLoginWithPasswordCommand => new RelayCommand(ShowLoginWithPassword);
		public ICommand LoginWithPasswordCommand => new RelayCommand(LoginWithPassword);
		public ICommand LoginWithWindowsHelloCommand => new RelayCommand(LoginWithWindowsHello);

		public async Task<Task> LoadAsync(ShellArgs args)
		{
			ViewModelArgs = args;

			UserName = SettingsService.UserName ?? args.UserInfo.AccountName;
			IsLoginWithWindowsHello = await LoginService.IsWindowsHelloEnabledAsync(UserName);
			IsLoginWithPassword = !IsLoginWithWindowsHello;
			IsBusy = false;

			return Task.CompletedTask;
		}

		public void Login()
		{
			if (IsLoginWithPassword)
			{
				LoginWithPassword();
			}
			else
			{
				LoginWithWindowsHello();
			}
		}

		private void ShowLoginWithPassword()
		{
			IsLoginWithWindowsHello = false;
			IsLoginWithPassword = true;
		}

		public async void LoginWithPassword()
		{
			IsBusy = true;
			var result = ValidateInput();
			if (result.IsOk)
			{
				result = await LoginService.SignInWithPasswordAsync(UserName, Password);
				if (result.IsOk)
				{
					await LoadPermissionsAsync();
					if (!await LoginService.IsWindowsHelloEnabledAsync(UserName))
					{
						await LoginService.TrySetupWindowsHelloAsync(UserName);
					}
					await VerifyLicenseAsync();
					await EnterApplication();
					return;
				}
			}
			await DialogService.ShowAsync(result.Message, result.Description);
			IsBusy = false;
		}

		public async void LoginWithWindowsHello()
		{
			IsBusy = true;
			var result = await LoginService.SignInWithWindowsHelloAsync();
			if (result.IsOk)
			{
				await LoadPermissionsAsync();
				await VerifyLicenseAsync();
				await EnterApplication();
				return;
			}
			await DialogService.ShowAsync(result.Message, result.Description);
			IsBusy = false;
		}

		private Task EnterApplication()
		{
			if (ViewModelArgs.UserInfo.AccountName != UserName)
			{
				ViewModelArgs.UserInfo = new UserInfo
				{
					AccountName = UserName,
					FirstName = UserName,
					PictureSource = null
				};
			}
			if (_hasSecurityAdministration || _isLicenseValid)
			{
				NavigationService.Navigate<MainShellViewModel>(ViewModelArgs);
			}
			else
			{
				ViewModelArgs.Parameter = new LicenseActivationModel
				{
					Email = UserName,
					Password = Password,
					ProductType = AppType.EnterpriseManager
				};
				NavigationService.Navigate<LicenseActivationViewModel>(ViewModelArgs);
			}
			return Task.CompletedTask;
		}

		private Result ValidateInput()
		{
			if (String.IsNullOrWhiteSpace(UserName))
			{
				return Result.Error("Login error", "Please, enter a valid user name.");
			}
			if (String.IsNullOrWhiteSpace(Password))
			{
				return Result.Error("Login error", "Please, enter a valid password.");
			}
			return Result.Ok();
		}

		private async Task LoadPermissionsAsync()
		{
			await _lookupTables.LoadAfterLoginAsync();
			_hasSecurityAdministration = _authorizationService.HasPermission(Permissions.SecurityAdministration);
		}

		private async Task VerifyLicenseAsync()
		{
			if (!_hasSecurityAdministration)
			{
				_isLicenseValid = await IsLicenseValidAsync();
			}
		}

		private async Task<bool> IsLicenseValidAsync()
		{
			if (await LicenseService.IsLicenseValidOfflineAsync())
			{
				return true;
			}
			if (await NetworkService.IsInternetAvailableAsync())
			{
				var license = await LicenseService.ValidateSubscriptionOnlineAsync(UserName, Password);
				if (license.IsActivated)
				{
					LicenseService.SaveLicenseLocally(LicenseService.CreateSubscriptionInfoDto(license));
					return true;
				}
			}
			return false;
		}
	}
}
