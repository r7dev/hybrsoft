using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
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
