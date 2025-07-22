using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class SubscriptionsViewModel : ViewModelBase
	{
		public SubscriptionsViewModel(ISubscriptionService subscriptionService, ICommonServices commonServices) : base(commonServices)
		{
			SubscriptionService = subscriptionService;
			SubscriptionList = new SubscriptionListViewModel(SubscriptionService, commonServices);
		}

		public ISubscriptionService SubscriptionService { get; }

		public SubscriptionListViewModel SubscriptionList { get; set; }

		public async Task LoadAsync(SubscriptionListArgs args)
		{
			await SubscriptionList.LoadAsync(args);
		}

		public void Unload()
		{
			SubscriptionList.Unload();
		}

		public void Subscribe()
		{
			SubscriptionList.Subscribe();
		}
		public void Unsubscribe()
		{
			SubscriptionList.Unsubscribe();
		}
	}
}
