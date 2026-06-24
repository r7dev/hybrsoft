using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LostAndFoundsViewModel : ViewModelBase
	{
		public LostAndFoundsViewModel(ILostAndFoundService lostAndFoundService,
			ISettingsService settingsService,
			ICommonServices commonServices) : base(commonServices)
		{
			_lostAndFoundService = lostAndFoundService;
			_settingsService = settingsService;
			LostAndFoundList = new LostAndFoundListViewModel(_lostAndFoundService, _settingsService, commonServices);
		}

		private readonly ILostAndFoundService _lostAndFoundService;
		private readonly ISettingsService _settingsService;

		public LostAndFoundListViewModel LostAndFoundList { get; set; }

		public async Task LoadAsync(LostAndFoundListArgs args)
		{
			await LostAndFoundList.LoadAsync(args);
		}

		public void Unload()
		{
			LostAndFoundList.Unload();
		}

		public void Subscribe()
		{
			LostAndFoundList.Subscribe();
		}
		public void Unsubscribe()
		{
			LostAndFoundList.Unsubscribe();
		}
	}
}
