using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IStudentBelongingService
	{
		Task<StudentBelongingModel> GetStudentBelongingAsync(long id);
		Task<IList<StudentBelongingModel>> GetStudentBelongingsAsync(DataRequest<StudentBelonging> request);
		Task<IList<StudentBelongingModel>> GetStudentBelongingsAsync(int skip, int take, DataRequest<StudentBelonging> request);
		Task<int> GetStudentBelongingsCountAsync(DataRequest<StudentBelonging> request);

		Task<int> UpdateStudentBelongingAsync(StudentBelongingModel model);

		Task<int> DeleteStudentBelongingAsync(StudentBelongingModel model);
		Task<int> DeleteStudentBelongingRangeAsync(int index, int length, DataRequest<StudentBelonging> request);
	}
}
