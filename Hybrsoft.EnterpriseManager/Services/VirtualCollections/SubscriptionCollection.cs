using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class SubscriptionCollection(ISubscriptionService subscriptionService, ILogService logService) : VirtualCollection<SubscriptionDto>(logService)
	{
		private DataRequest<Subscription> _dataRequest = null;

		public ISubscriptionService SubscriptionService { get; } = subscriptionService;

		private readonly SubscriptionDto _defaultItem = SubscriptionDto.CreateEmpty();
		protected override SubscriptionDto DefaultItem => _defaultItem;

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

		protected override async Task<IList<SubscriptionDto>> FetchDataAsync(int rangeIndex, int rangeSize)
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
