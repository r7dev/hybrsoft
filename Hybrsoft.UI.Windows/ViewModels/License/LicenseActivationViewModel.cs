using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LicenseActivationViewModel(
		ILicenseService licenseService,
		INetworkService networkService,
		ICommonServices commonServices) : ShellViewModel(commonServices)
	{
		ILicenseService LicenseService { get; } = licenseService;
		INetworkService NetworkService { get; } = networkService;

		private bool _IsInternetAvailable;
		public bool IsInternetAvailable
		{
			get { return _IsInternetAvailable; }
			set { Set(ref _IsInternetAvailable, value); }
		}

		private bool _IsBusy;
		public bool IsBusy
		{
			get { return _IsBusy; }
			set { Set(ref _IsBusy, value); }
		}

		private string _licenseKey;
		public string LicenseKey
		{
			get { return _licenseKey; }
			set { Set(ref _licenseKey, value); }
		}

		private bool _hasInfo;
		public bool HasInfo
		{
			get { return _hasInfo; }
			set { Set(ref _hasInfo, value); }
		}
		private string _infoGlyph;
		public string InfoGlyph
		{
			get { return _infoGlyph; }
			set { Set(ref _infoGlyph, value); }
		}
		private Brush _infoForeground;
		public Brush InfoForeground
		{
			get { return _infoForeground; }
			set { Set(ref _infoForeground, value); }
		}

		public override async Task LoadAsync(ShellArgs args)
		{
			ViewModelArgs = args;
			IsInternetAvailable = await NetworkService.IsInternetAvailableAsync();

			await base.LoadAsync(args);
		}
		override public void Subscribe()
		{
			MessageService.Subscribe<LicenseActivationViewModel>(this, OnMessage);
			base.Subscribe();
		}

		override public void Unsubscribe()
		{
			base.Unsubscribe();
		}

		public override void Unload()
		{
			base.Unload();
		}

		public ICommand ActivateLicenseCommand => new RelayCommand(ActivateLicense);

		private async void ActivateLicense()
		{
			IsBusy = true;
			HasInfo = false;

			IsInternetAvailable = await NetworkService.IsInternetAvailableAsync();
			if (!IsInternetAvailable)
			{
				string resourceKeyTitle = $"{nameof(LicenseActivationViewModel)}_NoInternetConnection";
				string title = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKeyTitle);
				string resourceKeyMessage = $"{nameof(LicenseActivationViewModel)}_PleaseCheckYourConnectionAndTryAgain";
				string message = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKeyMessage);
				StatusErrorYourself(new StatusInfoDto(title, message));
				return;
			}

			LicenseActivationModel license = (LicenseActivationModel)ViewModelArgs.Parameter;
			license.LicenseKey = LicenseKey;
			var response = await LicenseService.ActivateSubscriptionOnlineAsync(license);
			await Task.Delay(200);

			if (response.IsActivated)
			{
				LicenseService.SaveLicenseLocally(LicenseService.CreateSubscriptionInfoDto(response));

				InfoGlyph = "\uE8FB";
				InfoForeground = new SolidColorBrush(Colors.Green);
				HasInfo = true;

				string title = response.TitleUid != null
					? ResourceService.GetString(nameof(ResourceFiles.InfoMessages), response.TitleUid)
					: string.Empty;

				string message = response.MessageUid != null
					? ResourceService.GetString(nameof(ResourceFiles.InfoMessages), response.MessageUid)
					: response.Message;
				SuccessMessageYourselt(new StatusInfoDto(title, message));

				await Task.Delay(4000);
				EnterApplication();
			}
			else
			{
				InfoGlyph = "\uE783";
				InfoForeground = new SolidColorBrush(Colors.Red);
				HasInfo = true;

				string title = response.TitleUid != null
					? ResourceService.GetString(nameof(ResourceFiles.Errors), response.TitleUid)
					: string.Empty;

				string message = response.MessageUid != null
					? ResourceService.GetString(nameof(ResourceFiles.Errors), response.MessageUid)
					: response.Message;
				StatusErrorYourself(new StatusInfoDto(title, message));
			}
			IsBusy = false;
		}

		public ICommand CheckConnectionCommand => new RelayCommand(CheckConnection);

		private async void CheckConnection()
		{
			IsInternetAvailable = await NetworkService.IsInternetAvailableAsync();
			if (IsInternetAvailable)
			{
				string resourceKeyTitle = $"{nameof(LicenseActivationViewModel)}_ConnectionRestored";
				string title = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKeyTitle);
				string resourceKeyMessage = $"{nameof(LicenseActivationViewModel)}_InternetConnectionIsAvailable";
				string message = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKeyMessage);
				StatusMessageYourself(new StatusInfoDto(title, message));
				IsBusy = false;
			}
		}

		private void EnterApplication()
		{
			NavigationService.Navigate<MainShellViewModel>(ViewModelArgs);
		}

		private void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "StatusError":
				case "StatusMessage":
				case "SucessMessage":
					break;
			}
		}
	}
}
