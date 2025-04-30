using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class ClassroomsViewModel : ViewModelBase
	{
		public ClassroomsViewModel(IClassroomService classroomService, ICommonServices commonServices) : base(commonServices)
		{
			ClassroomService = classroomService;
			ClassroomList = new ClassroomListViewModel(ClassroomService, commonServices);
		}

		public IClassroomService ClassroomService { get; }

		public ClassroomListViewModel ClassroomList { get; set; }

		public async Task LoadAsync(ClassroomListArgs args)
		{
			await ClassroomList.LoadAsync(args);
		}

		public void Unload()
		{
			ClassroomList.Unload();
		}

		public void Subscribe()
		{
			ClassroomList.Subscribe();
		}
		public void Unsubscribe()
		{
			ClassroomList.Unsubscribe();
		}
	}
}
