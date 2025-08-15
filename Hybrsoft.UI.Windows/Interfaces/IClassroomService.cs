using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IClassroomService
	{
		Task<ClassroomDto> GetClassroomAsync(long id);
		Task<IList<ClassroomDto>> GetClassroomsAsync(DataRequest<Classroom> request);
		Task<IList<ClassroomDto>> GetClassroomsAsync(int skip, int take, DataRequest<Classroom> request);
		Task<int> GetClassroomsCountAsync(DataRequest<Classroom> request);

		Task<int> UpdateClassroomAsync(ClassroomDto model);

		Task<int> DeleteClassroomAsync(ClassroomDto model);
		Task<int> DeleteClassroomRangeAsync(int index, int length, DataRequest<Classroom> request);
	}
}
