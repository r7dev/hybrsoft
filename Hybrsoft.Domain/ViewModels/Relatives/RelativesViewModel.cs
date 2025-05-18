using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class RelativesViewModel : ViewModelBase
	{
		public RelativesViewModel(IRelativeService relativeService, ICommonServices commonServices) : base(commonServices)
		{
			RelativeService = relativeService;
			RelativeList = new RelativeListViewModel(RelativeService, commonServices);
		}

		public IRelativeService RelativeService { get; }

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
