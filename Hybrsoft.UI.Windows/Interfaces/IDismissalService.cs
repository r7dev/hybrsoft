using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IDismissalService
	{
		Task<IList<DismissibleStudentDto>> GetDismissibleStudentsAsync(DataRequest<ClassroomStudent> request);
		Task<IList<DismissibleStudentDto>> GetDismissibleStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<int> GetDismissibleStudentsCountAsync(DataRequest<ClassroomStudent> request);
		
		Task<DismissalDto> GetDismissalAsync(long id);
		Task<IList<DismissalDto>> GetDismissalsAsync(DataRequest<Dismissal> request);
		Task<IList<DismissalDto>> GetDismissalsAsync(int skip, int take, DataRequest<Dismissal> request);
		Task<int> GetDismissalsCountAsync(DataRequest<Dismissal> request);

		Task<int> UpdateDismissalAsync(DismissalDto model);

		Task<int> ApproveDismissalAsync(DismissalDto model);
		Task<int> ApproveDismissalRangeAsync(int index, int length, DataRequest<Dismissal> request);
	}
}
