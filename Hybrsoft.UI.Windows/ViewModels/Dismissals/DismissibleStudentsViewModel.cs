using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class DismissibleStudentsViewModel : ViewModelBase
	{
		public DismissibleStudentsViewModel(IDismissalService dismissalService, ICommonServices commonServices) : base(commonServices)
		{
			DismissalService = dismissalService;
			DismissibleStudentList = new DismissibleStudentListViewModel(DismissalService, commonServices);
		}

		public IDismissalService DismissalService { get; }

		public DismissibleStudentListViewModel DismissibleStudentList { get; set; }

		public async Task LoadAsync(DismissibleStudentListArgs args)
		{
			await DismissibleStudentList.LoadAsync(args);
		}

		public void Unload()
		{
			DismissibleStudentList.Unload();
		}

		public void Subscribe()
		{
			DismissibleStudentList.Subscribe();
		}
		public void Unsubscribe()
		{
			DismissibleStudentList.Unsubscribe();
		}
	}
}
