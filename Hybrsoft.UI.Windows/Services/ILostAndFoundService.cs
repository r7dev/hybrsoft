using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ILostAndFoundService
	{
		Task<LostAndFoundModel> GetLostAndFoundAsync(long id);
		Task<IList<LostAndFoundModel>> GetLostAndFoundAsync(DataRequest<LostAndFound> request);
		Task<IList<LostAndFoundModel>> GetLostAndFoundAsync(int skip, int take, DataRequest<LostAndFound> request);
		Task<int> GetLostAndFoundCountAsync(DataRequest<LostAndFound> request);

		Task<int> UpdateLostAndFoundAsync(LostAndFoundModel model);

		Task<int> DeleteLostAndFoundAsync(LostAndFoundModel model);
		Task<int> DeleteLostAndFoundRangeAsync(int index, int length, DataRequest<LostAndFound> request);

		Task<int> UpdateLostAndFoundEmbeddingAsync(LostAndFoundModel model);
	}
}
