using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ISubscriptionService
	{
		Task<SubscriptionModel> GetSubscriptionAsync(long id);
		Task<IList<SubscriptionModel>> GetSubscriptionsAsync(DataRequest<Subscription> request);
		Task<IList<SubscriptionModel>> GetSubscriptionsAsync(int skip, int take, DataRequest<Subscription> request);
		Task<int> GetSubscriptionsCountAsync(DataRequest<Subscription> request);

		Task<int> UpdateSubscriptionAsync(SubscriptionModel model);

		Task<int> DeleteSubscriptionAsync(SubscriptionModel model);
		Task<int> DeleteSubscriptionRangeAsync(int index, int length, DataRequest<Subscription> request);
	}
}
