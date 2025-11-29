using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class SubscriptionsViewModel : ViewModelBase
	{
		public SubscriptionsViewModel(ISubscriptionService subscriptionService,
			ICommonServices commonServices) : base(commonServices)
		{
			_subscriptionService = subscriptionService;
			SubscriptionList = new SubscriptionListViewModel(_subscriptionService, commonServices);
		}

		private readonly ISubscriptionService _subscriptionService;

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
