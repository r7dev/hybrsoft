using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IStudentRelativeService
	{
		Task<StudentRelativeModel> GetStudentRelativeAsync(long id);
		Task<IList<StudentRelativeModel>> GetStudentRelativesAsync(DataRequest<StudentRelative> request);
		Task<IList<StudentRelativeModel>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request);
		Task<IList<long>> GetAddedRelativeKeysInStudentAsync(long parentID);
		Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request);

		Task<int> UpdateStudentRelativeAsync(StudentRelativeModel model);

		Task<int> DeleteStudentRelativeAsync(StudentRelativeModel model);
		Task<int> DeleteStudentRelativeRangeAsync(int index, int length, DataRequest<StudentRelative> request);
	}
}
