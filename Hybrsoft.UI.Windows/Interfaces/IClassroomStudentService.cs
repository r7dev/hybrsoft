using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IClassroomStudentService
	{
		Task<ClassroomStudentModel> GetClassroomStudentAsync(long id);
		Task<IList<ClassroomStudentModel>> GetClassroomStudentsAsync(DataRequest<ClassroomStudent> request);
		Task<IList<ClassroomStudentModel>> GetClassroomStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<IList<long>> GetAddedStudentKeysInClassroomAsync(long parentID);
		Task<int> GetClassroomStudentsCountAsync(DataRequest<ClassroomStudent> request);

		Task<int> UpdateClassroomStudentAsync(ClassroomStudentModel model);

		Task<int> DeleteClassroomStudentAsync(ClassroomStudentModel model);
		Task<int> DeleteClassroomStudentRangeAsync(int index, int length, DataRequest<ClassroomStudent> request);
	}
}
