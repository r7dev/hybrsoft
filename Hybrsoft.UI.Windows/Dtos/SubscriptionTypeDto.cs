using Hybrsoft.UI.Windows.Infrastructure.ViewModels;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class SubscriptionTypeDto : ObservableObject
	{
		public short SubscriptionTypeID { get; set; }
		public string DisplayName { get; set; }
	}
}
