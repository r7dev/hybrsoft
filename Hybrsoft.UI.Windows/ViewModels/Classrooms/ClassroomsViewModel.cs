using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class ClassroomsViewModel : ViewModelBase
	{
		public ClassroomsViewModel(IClassroomService classroomService, IClassroomStudentService classroomStudentService, ICommonServices commonServices) : base(commonServices)
		{
			ClassroomService = classroomService;
			ClassroomList = new ClassroomListViewModel(ClassroomService, commonServices);
			ClassroomDetails = new ClassroomDetailsViewModel(ClassroomService, commonServices);
			ClassroomStudentList = new ClassroomStudentListViewModel(classroomStudentService, commonServices);
		}

		public IClassroomService ClassroomService { get; }

		public ClassroomListViewModel ClassroomList { get; set; }

		public ClassroomDetailsViewModel ClassroomDetails { get; set; }

		public ClassroomStudentListViewModel ClassroomStudentList { get; set; }

		public async Task LoadAsync(ClassroomListArgs args)
		{
			await ClassroomList.LoadAsync(args);
		}

		public void Unload()
		{
			ClassroomDetails.CancelEdit();
			ClassroomList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<ClassroomListViewModel>(this, OnMessage);
			ClassroomList.Subscribe();
			ClassroomDetails.Subscribe();
			ClassroomStudentList.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			ClassroomList.Unsubscribe();
			ClassroomDetails.Unsubscribe();
			ClassroomStudentList.Unsubscribe();
		}

		private async void OnMessage(ClassroomListViewModel viewModel, string message, object args)
		{
			if (viewModel == ClassroomList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (ClassroomDetails.IsEditMode)
			{
				StatusReady();
				ClassroomDetails.CancelEdit();
			}
			ClassroomStudentList.IsMultipleSelection = false;
			var selected = ClassroomList.SelectedItem;
			if (!ClassroomStudentList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
					await PopulateClassroomStudents(selected);
				}
			}
			ClassroomDetails.Item = selected;
		}

		private async Task PopulateDetails(ClassroomModel selected)
		{
			try
			{
				var model = await ClassroomService.GetClassroomAsync(selected.ClassroomID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Classrooms", "Load Details", ex);
			}
		}

		private async Task PopulateClassroomStudents(ClassroomModel selectedItem)
		{
			try
			{
				if (selectedItem != null)
				{
					await ClassroomStudentList.LoadAsync(new ClassroomStudentListArgs { ClassroomID = selectedItem.ClassroomID }, silent: true);
				}
			}
			catch (Exception ex)
			{
				LogException("Classrooms", "Load Students in the Classroom", ex);
			}
		}
	}
}
