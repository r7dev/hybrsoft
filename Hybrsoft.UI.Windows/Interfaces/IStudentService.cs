using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IStudentService
	{
		Task<StudentModel> GetStudentAsync(long id);
		Task<IList<StudentModel>> GetStudentsAsync(DataRequest<Student> request);
		Task<IList<StudentModel>> GetStudentsAsync(int skip, int take, DataRequest<Student> request);
		Task<int> GetStudentsCountAsync(DataRequest<Student> request);

		Task<int> UpdateStudentAsync(StudentModel model);

		Task<int> DeleteStudentAsync(StudentModel model);
		Task<int> DeleteStudentRangeAsync(int index, int length, DataRequest<Student> request);
	}
}
