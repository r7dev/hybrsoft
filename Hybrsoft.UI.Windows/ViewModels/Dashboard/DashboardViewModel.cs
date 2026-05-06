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
		ILostAndFoundService lostAndFoundService,
		ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		private readonly IStudentService _studentService = studentService;
		private readonly ILostAndFoundService _lostAndFoundService = lostAndFoundService;

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

		private IList<LostAndFoundModel> _lostAndFound = null;
		public IList<LostAndFoundModel> LostAndFound
		{
			get => _lostAndFound;
			set => Set(ref _lostAndFound, value);
		}

		public async Task LoadAsync()
		{
			StartStatusMessage(StartTitle, StartMessage);
			await LoadStudentsAsync();
			await LoadLostAndFoundAsync();
			EndStatusMessage(EndTitle, EndMessage);
		}
		public void Unload()
		{
			// Release heavy collections.
			(Students as IDisposable)?.Dispose();
			Students = null;
			(LostAndFound as IDisposable)?.Dispose();
			LostAndFound = null;
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

		private async Task LoadLostAndFoundAsync()
		{
			try
			{
				var request = new DataRequest<LostAndFound>
				{
					OrderBys = [(r => r.CreatedOn, OrderBy.Desc)]
				};
				LostAndFound = await _lostAndFoundService.GetLostAndFoundAsync(0, 5, request);
			}
			catch (Exception ex)
			{
				LogException("Dashboard", "Load Lost and Founds", ex);
			}
		}

		public void ItemSelected(string item)
		{
			switch (item)
			{
				case "Students":
					NavigationService.Navigate<StudentsViewModel>(new StudentListArgs { OrderBys = [(r => r.CreatedOn, OrderBy.Desc)] });
					break;
				case "LostAndFound":
					NavigationService.Navigate<LostAndFoundsViewModel>(new LostAndFoundListArgs { OrderBys = [(r => r.CreatedOn, OrderBy.Desc)] });
					break;
				default:
					break;
			}
		}
	}
}
