using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LostAndFoundViewModel : ViewModelBase
	{
		public LostAndFoundViewModel(ILostAndFoundService lostAndFoundService,
			ICommonServices commonServices) : base(commonServices)
		{
			_lostAndFoundService = lostAndFoundService;
			LostAndFoundList = new LostAndFoundListViewModel(_lostAndFoundService, commonServices);
		}

		private readonly ILostAndFoundService _lostAndFoundService;

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
