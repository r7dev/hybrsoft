using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class SettingsViewModel(ISettingsService settingsService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ISettingsService SettingsService { get; } = settingsService;

		public string AppName => $"{SettingsService.AppName}";
		public string Version => $"{SettingsService.Version}";

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
			string resourceKey = string.Concat(nameof(SettingsViewModel), "_TheLanguageHasBeenChangedAndTheApplicationMustBeRestarted");
			string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
			WarningMessage(message);
			LogSuccess("Settings", "LanguageChanged", "Language changed successfully",$"The language has been successfully changed to {value.DisplayName} ({value.Tag}).");

			_ = ResourceService.SetLanguageAsync(value);
		}

		public SettingsArgs ViewModelArgs { get; private set; }

		public Task LoadAsync(SettingsArgs args)
		{
			ViewModelArgs = args ?? SettingsArgs.CreateDefault();
			_availableLanguages = ResourceService.LanguageOptions;
			_selectedLanguage = ResourceService.GetCurrentLanguageItem();

			StatusReady();

			return Task.CompletedTask;
		}
	}
}
