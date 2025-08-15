using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IClassroomService
	{
		Task<ClassroomModel> GetClassroomAsync(long id);
		Task<IList<ClassroomModel>> GetClassroomsAsync(DataRequest<Classroom> request);
		Task<IList<ClassroomModel>> GetClassroomsAsync(int skip, int take, DataRequest<Classroom> request);
		Task<int> GetClassroomsCountAsync(DataRequest<Classroom> request);

		Task<int> UpdateClassroomAsync(ClassroomModel model);

		Task<int> DeleteClassroomAsync(ClassroomModel model);
		Task<int> DeleteClassroomRangeAsync(int index, int length, DataRequest<Classroom> request);
	}
}
