using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class DashboardViewModel(IStudentService studentService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public IStudentService StudentService { get; } = studentService;

		private IList<StudentDto> _students = null;
		public IList<StudentDto> Students
		{
			get => _students;
			set => Set(ref _students, value);
		}

		public async Task LoadAsync()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DashboardViewModel), "_LoadingDashboard"));
			StartStatusMessage(startMessage);
			await LoadStudentsAsync();
			string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DashboardViewModel), "_DashboardLoaded")); ;
			EndStatusMessage(endMessage);
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
					OrderByDesc = r => r.CreatedOn
				};
				Students = await StudentService.GetStudentsAsync(0, 5, request);
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
					NavigationService.Navigate<StudentsViewModel>(new StudentListArgs { OrderByDesc = r => r.CreatedOn});
					break;
				default:
					break;
			}
		}
	}
}
