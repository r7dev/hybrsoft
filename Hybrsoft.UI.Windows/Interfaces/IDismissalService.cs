using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IDismissalService
	{
		Task<IList<DismissibleStudentModel>> GetDismissibleStudentsAsync(DataRequest<ClassroomStudent> request);
		Task<IList<DismissibleStudentModel>> GetDismissibleStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<int> GetDismissibleStudentsCountAsync(DataRequest<ClassroomStudent> request);
		
		Task<DismissalModel> GetDismissalAsync(long id);
		Task<IList<DismissalModel>> GetDismissalsAsync(DataRequest<Dismissal> request);
		Task<IList<DismissalModel>> GetDismissalsAsync(int skip, int take, DataRequest<Dismissal> request);
		Task<int> GetDismissalsCountAsync(DataRequest<Dismissal> request);

		Task<int> UpdateDismissalAsync(DismissalModel model);

		Task<int> ApproveDismissalAsync(DismissalModel model);
		Task<int> ApproveDismissalRangeAsync(int index, int length, DataRequest<Dismissal> request);
	}
}
