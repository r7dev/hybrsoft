using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RelativesViewModel : ViewModelBase
	{
		public RelativesViewModel(IRelativeService relativeService,
			ISettingsService settingsService,
			ICommonServices commonServices) : base(commonServices)
		{
			_relativeService = relativeService;
			_settingsService = settingsService;
			RelativeList = new RelativeListViewModel(_relativeService, _settingsService, commonServices);
		}

		private readonly IRelativeService _relativeService;
		private readonly ISettingsService _settingsService;

		public RelativeListViewModel RelativeList { get; set; }

		public async Task LoadAsync(RelativeListArgs args)
		{
			await RelativeList.LoadAsync(args);
		}

		public void Unload()
		{
			RelativeList.Unload();
		}

		public void Subscribe()
		{
			RelativeList.Subscribe();
		}
		public void Unsubscribe()
		{
			RelativeList.Unsubscribe();
		}
	}
}
