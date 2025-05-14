using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class ClassroomDetailsWithStudentsViewModel(IClassroomService classroomService, IClassroomStudentService classroomStudentService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public ClassroomDetailsViewModel ClassroomDetails { get; set; } = new ClassroomDetailsViewModel(classroomService, commonServices);
		public ClassroomStudentListViewModel ClassroomStudentList { get; set; } = new ClassroomStudentListViewModel(classroomStudentService, commonServices);

		public async Task LoadAsync(ClassroomDetailsArgs args)
		{
			await ClassroomDetails.LoadAsync(args);

			long classroomId = args?.ClassroomID ?? 0;
			if (classroomId > 0)
			{
				await ClassroomStudentList.LoadAsync(new ClassroomStudentListArgs { ClassroomID = args.ClassroomID });
			}
			else
			{
				await ClassroomStudentList.LoadAsync(new ClassroomStudentListArgs { IsEmpty = true }, silent: true);
			}
		}
		public void Unload()
		{
			ClassroomDetails.CancelEdit();
			ClassroomDetails.Unload();
			ClassroomStudentList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<ClassroomDetailsViewModel, ClassroomDto>(this, OnMessage);
			ClassroomDetails.Subscribe();
			ClassroomStudentList.Subscribe();
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			ClassroomDetails.Unsubscribe();
			ClassroomStudentList.Unsubscribe();
		}

		private async void OnMessage(ClassroomDetailsViewModel viewModel, string message, ClassroomDto classroom)
		{
			if (viewModel == ClassroomDetails && (message == "NewItemSaved" || message == "ItemChanged"))
			{
				await ClassroomStudentList.LoadAsync(new ClassroomStudentListArgs { ClassroomID = classroom.ClassroomID });
			}
		}
	}
}
