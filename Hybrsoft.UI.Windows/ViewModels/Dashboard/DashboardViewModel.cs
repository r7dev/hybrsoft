using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class DashboardViewModel(IStudentService studentService,
		ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		private readonly IStudentService _studentService = studentService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<DashboardViewModel>(ResourceFiles.InfoMessages, "LoadingDashboard");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<DashboardViewModel>(ResourceFiles.InfoMessages, "DashboardLoaded");

		private IList<StudentModel> _students = null;
		public IList<StudentModel> Students
		{
			get => _students;
			set => Set(ref _students, value);
		}

		public async Task LoadAsync()
		{
			StartStatusMessage(StartTitle, StartMessage);
			await LoadStudentsAsync();
			EndStatusMessage(EndTitle, EndMessage);
		}
		public void Unload()
		{
			// Release heavy collections.
			(Students as IDisposable)?.Dispose();
			Students = null;
		}

		private async Task LoadStudentsAsync()
		{
			try
			{
				var request = new DataRequest<Student>
				{
					OrderBys = [(r => r.CreatedOn, OrderBy.Desc)]
				};
				Students = await _studentService.GetStudentsAsync(0, 5, request);
			}
			catch (Exception ex)
			{
				LogException("Dashboard", "Load Students", ex);
			}
		}

		public void ItemSelected(string item)
		{
			switch (item)
			{
				case "Students":
					NavigationService.Navigate<StudentsViewModel>(new StudentListArgs { OrderBys = [(r => r.CreatedOn, OrderBy.Desc)] });
					break;
				default:
					break;
			}
		}
	}
}
