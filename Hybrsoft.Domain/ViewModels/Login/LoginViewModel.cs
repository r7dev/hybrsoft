using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class LoginViewModel(ILoginService loginService, ISettingsService settingsService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ILoginService LoginService { get; } = loginService;
		public ISettingsService SettingsService { get; } = settingsService;

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
					if (! await LoginService.IsWindowsHelloEnabledAsync(UserName))
					{
						await LoginService.TrySetupWindowsHelloAsync(UserName);
					}
					EnterApplication();
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
				EnterApplication();
				return;
			}
			await DialogService.ShowAsync(result.Message, result.Description);
			IsBusy = false;
		}

		private void EnterApplication()
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
			NavigationService.Navigate<MainShellViewModel>(ViewModelArgs);
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
	}
}
