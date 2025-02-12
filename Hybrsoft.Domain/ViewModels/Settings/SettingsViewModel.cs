using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
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

		public SettingsArgs ViewModelArgs { get; private set; }

		public Task LoadAsync(SettingsArgs args)
		{
			ViewModelArgs = args ?? SettingsArgs.CreateDefault();

			StatusReady();

			return Task.CompletedTask;
		}
	}
}
