using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class StudentsViewModel : ViewModelBase
	{
		public StudentsViewModel(IStudentService studentService, ICommonServices commonServices) : base(commonServices)
		{
			StudentService = studentService;
			StudentList = new StudentListViewModel(StudentService, commonServices);
		}

		public IStudentService StudentService { get; }

		public StudentListViewModel StudentList { get; set; }

		public async Task LoadAsync(StudentListArgs args)
		{
			await StudentList.LoadAsync(args);
		}

		public void Unload()
		{
			StudentList.Unload();
		}

		public void Subscribe()
		{
			StudentList.Subscribe();
		}
		public void Unsubscribe()
		{
			StudentList.Unsubscribe();
		}
	}
}
