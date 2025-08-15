using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class SubscriptionCollection(ISubscriptionService subscriptionService, ILogService logService) : VirtualCollection<SubscriptionModel>(logService)
	{
		private DataRequest<Subscription> _dataRequest = null;

		public ISubscriptionService SubscriptionService { get; } = subscriptionService;

		private readonly SubscriptionModel _defaultItem = SubscriptionModel.CreateEmpty();
		protected override SubscriptionModel DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Subscription> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await SubscriptionService.GetSubscriptionsCountAsync(_dataRequest);
				Ranges[0] = await SubscriptionService.GetSubscriptionsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<SubscriptionModel>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await SubscriptionService.GetSubscriptionsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("SubscriptionCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
