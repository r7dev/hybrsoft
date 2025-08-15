using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class StudentDetailsWithRelativesViewModel(IStudentService studentService, IStudentRelativeService studentRelativeService, IFilePickerService filePickerService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public StudentDetailsViewModel StudentDetails { get; set; } = new StudentDetailsViewModel(studentService, filePickerService, commonServices);
		public StudentRelativeListViewModel StudentRelativeList { get; set; } = new StudentRelativeListViewModel(studentRelativeService, commonServices);

		public async Task LoadAsync(StudentDetailsArgs args)
		{
			await StudentDetails.LoadAsync(args);

			long StudentId = args?.StudentID ?? 0;
			if (StudentId > 0)
			{
				await StudentRelativeList.LoadAsync(new StudentRelativeListArgs { StudentID = args.StudentID });
			}
			else
			{
				await StudentRelativeList.LoadAsync(new StudentRelativeListArgs { IsEmpty = true }, silent: true);
			}
		}
		public void Unload()
		{
			StudentDetails.CancelEdit();
			StudentDetails.Unload();
			StudentRelativeList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<StudentDetailsViewModel, StudentDto>(this, OnMessage);
			StudentDetails.Subscribe();
			StudentRelativeList.Subscribe();
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			StudentDetails.Unsubscribe();
			StudentRelativeList.Unsubscribe();
		}

		private async void OnMessage(StudentDetailsViewModel viewModel, string message, StudentDto student)
		{
			if (viewModel == StudentDetails && (message == "NewItemSaved" || message == "ItemChanged"))
			{
				await StudentRelativeList.LoadAsync(new StudentRelativeListArgs { StudentID = student.StudentID });
			}
		}
	}
}
