using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IRelativeService
	{
		Task<RelativeDto> GetRelativeAsync(long id);
		Task<IList<RelativeDto>> GetRelativesAsync(DataRequest<Relative> request);
		Task<IList<RelativeDto>> GetRelativesAsync(int skip, int take, DataRequest<Relative> request);
		Task<int> GetRelativesCountAsync(DataRequest<Relative> request);

		Task<int> UpdateRelativeAsync(RelativeDto model);

		Task<int> DeleteRelativeAsync(RelativeDto model);
		Task<int> DeleteRelativeRangeAsync(int index, int length, DataRequest<Relative> request);
	}
}
