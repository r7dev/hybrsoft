using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class SettingsViewModel(ISettingsService settingsService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ISettingsService SettingsService { get; } = settingsService;

		public string AppName => $"{SettingsService.AppName}";
		public string Version => $"{SettingsService.Version}";
		public string LicenseTo;

		private bool _isBusy = false;
		public bool IsBusy
		{
			get => _isBusy;
			set => Set(ref _isBusy, value);
		}

		private List<LanguageModel> _availableLanguages;
		public List<LanguageModel> AvailableLanguages
		{
			get { return _availableLanguages; }
			set { Set(ref _availableLanguages, value); }
		}

		private LanguageModel _selectedLanguage;
		public LanguageModel SelectedLanguage
		{
			get { return _selectedLanguage; }
			set
			{
				if (Set(ref _selectedLanguage, value) is true)
				{
					OnSelectedLanguageChanged(value);
				}
			}
		}
		private void OnSelectedLanguageChanged(LanguageModel value)
		{
			string title = ResourceService.GetString<SettingsViewModel>(ResourceFiles.Warnings, "TheLanguageHasBeenSuccessfullyChanged");
			string message = ResourceService.GetString<SettingsViewModel>(ResourceFiles.Warnings, "PleaseRestartTheAppForTheChangesToBeFullyApplied");
			WarningMessage(title, message);
			LogSuccess("Settings", "LanguageChanged", "Language changed successfully",$"The language has been successfully changed to {value.DisplayName} ({value.Tag}).");

			_ = ResourceService.SetLanguageAsync(value);
		}

		public SettingsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(SettingsArgs args)
		{
			ViewModelArgs = args ?? SettingsArgs.CreateDefault();
			_availableLanguages = ResourceService.LanguageOptions;
			_selectedLanguage = ResourceService.GetCurrentLanguageItem();

			string licenseToPrefix = ResourceService.GetString<SettingsViewModel>(ResourceFiles.UI, "LicensedTo_Prefix");
			var licenseTo = await SettingsService.GetLicensedToAsync();
			LicenseTo = licenseToPrefix + licenseTo;

			StatusReady();
		}
	}
}
