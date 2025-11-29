using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RelativesViewModel : ViewModelBase
	{
		public RelativesViewModel(IRelativeService relativeService, ICommonServices commonServices) : base(commonServices)
		{
			_relativeService = relativeService;
			RelativeList = new RelativeListViewModel(_relativeService, commonServices);
		}

		private readonly IRelativeService _relativeService;

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
