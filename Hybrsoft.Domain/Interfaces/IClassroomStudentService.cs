using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IClassroomStudentService
	{
		Task<ClassroomStudentDto> GetClassroomStudentAsync(long classroomStudentId);
		Task<IList<ClassroomStudentDto>> GetClassroomStudentsAsync(DataRequest<ClassroomStudent> request);
		Task<IList<ClassroomStudentDto>> GetClassroomStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<IList<long>> GetAddedStudentKeysAsync(long classroomId);
		Task<int> GetClassroomStudentsCountAsync(DataRequest<ClassroomStudent> request);

		Task<int> UpdateClassroomStudentAsync(ClassroomStudentDto model);

		Task<int> DeleteClassroomStudentAsync(ClassroomStudentDto model);
		Task<int> DeleteClassroomStudentRangeAsync(int index, int length, DataRequest<ClassroomStudent> request);
	}
}
