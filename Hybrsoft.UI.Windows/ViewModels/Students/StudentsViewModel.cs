using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class StudentsViewModel : ViewModelBase
	{
		public StudentsViewModel(IStudentService studentService, IStudentRelativeService studentRelativeService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
		{
			StudentService = studentService;
			StudentList = new StudentListViewModel(StudentService, commonServices);
			StudentDetails = new StudentDetailsViewModel(StudentService, filePickerService, commonServices);
			StudentRelativeList = new StudentRelativeListViewModel(studentRelativeService, commonServices);
		}

		public IStudentService StudentService { get; }

		public StudentListViewModel StudentList { get; set; }

		public StudentDetailsViewModel StudentDetails { get; set; }

		public StudentRelativeListViewModel StudentRelativeList { get; set; }

		public async Task LoadAsync(StudentListArgs args)
		{
			await StudentList.LoadAsync(args);
		}

		public void Unload()
		{
			StudentDetails.CancelEdit();
			StudentList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<StudentListViewModel>(this, OnMessage);
			StudentList.Subscribe();
			StudentDetails.Subscribe();
			StudentRelativeList.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			StudentList.Unsubscribe();
			StudentDetails.Unsubscribe();
			StudentRelativeList.Unsubscribe();
		}

		private async void OnMessage(StudentListViewModel viewModel, string message, object args)
		{
			if (viewModel == StudentList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (StudentDetails.IsEditMode)
			{
				StatusReady();
				StudentDetails.CancelEdit();
			}
			StudentRelativeList.IsMultipleSelection = false;
			var selected = StudentList.SelectedItem;
			if (!StudentRelativeList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
					await PopulateStudentRelatives(selected);
				}
			}
			StudentDetails.Item = selected;
		}

		private async Task PopulateDetails(StudentDto selected)
		{
			try
			{
				var model = await StudentService.GetStudentAsync(selected.StudentID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Students", "Load Details", ex);
			}
		}

		private async Task PopulateStudentRelatives(StudentDto selectedItem)
		{
			try
			{
				if (selectedItem != null)
				{
					await StudentRelativeList.LoadAsync(new StudentRelativeListArgs { StudentID = selectedItem.StudentID }, silent: true);
				}
			}
			catch (Exception ex)
			{
				LogException("Students", "Load the Student's Relatives", ex);
			}
		}
	}
}
