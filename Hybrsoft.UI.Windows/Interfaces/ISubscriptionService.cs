using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ISubscriptionService
	{
		Task<SubscriptionDto> GetSubscriptionAsync(long id);
		Task<IList<SubscriptionDto>> GetSubscriptionsAsync(DataRequest<Subscription> request);
		Task<IList<SubscriptionDto>> GetSubscriptionsAsync(int skip, int take, DataRequest<Subscription> request);
		Task<int> GetSubscriptionsCountAsync(DataRequest<Subscription> request);

		Task<int> UpdateSubscriptionAsync(SubscriptionDto model);

		Task<int> DeleteSubscriptionAsync(SubscriptionDto model);
		Task<int> DeleteSubscriptionRangeAsync(int index, int length, DataRequest<Subscription> request);
	}
}
