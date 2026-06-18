using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IRelativeService
	{
		Task<RelativeModel> GetRelativeAsync(long id);
		Task<IList<RelativeModel>> GetRelativesAsync(DataRequest<Relative> request);
		Task<IList<RelativeModel>> GetRelativesAsync(int skip, int take, DataRequest<Relative> request);
		Task<int> GetRelativesCountAsync(DataRequest<Relative> request);

		Task<int> UpdateRelativeAsync(RelativeModel model);

		Task<int> DeleteRelativeAsync(RelativeModel model);
		Task<int> DeleteRelativeRangeAsync(int index, int length, DataRequest<Relative> request);
	}
}
