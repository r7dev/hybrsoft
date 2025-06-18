using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class DismissalsViewModel : ViewModelBase
	{
		public DismissalsViewModel(IDismissalService dismissalService, ICommonServices commonServices) : base(commonServices)
		{
			DismissalService = dismissalService;
			DismissalList = new DismissalListViewModel(DismissalService, commonServices);
		}

		public IDismissalService DismissalService { get; }

		public DismissalListViewModel DismissalList { get; set; }

		public async Task LoadAsync(DismissalListArgs args)
		{
			await DismissalList.LoadAsync(args);
		}

		public void Unload()
		{
			DismissalList.Unload();
		}

		public void Subscribe()
		{
			DismissalList.Subscribe();
		}
		public void Unsubscribe()
		{
			DismissalList.Unsubscribe();
		}
	}
}
