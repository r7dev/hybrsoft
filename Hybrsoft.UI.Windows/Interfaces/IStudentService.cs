using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IStudentService
	{
		Task<StudentDto> GetStudentAsync(long id);
		Task<IList<StudentDto>> GetStudentsAsync(DataRequest<Student> request);
		Task<IList<StudentDto>> GetStudentsAsync(int skip, int take, DataRequest<Student> request);
		Task<int> GetStudentsCountAsync(DataRequest<Student> request);

		Task<int> UpdateStudentAsync(StudentDto model);

		Task<int> DeleteStudentAsync(StudentDto model);
		Task<int> DeleteStudentRangeAsync(int index, int length, DataRequest<Student> request);
	}
}
