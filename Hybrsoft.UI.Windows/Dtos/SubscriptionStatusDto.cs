using Hybrsoft.UI.Windows.Infrastructure.ViewModels;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class SubscriptionStatusDto : ObservableObject
	{
		public short SubscriptionStatusID { get; set; }
		public string DisplayName { get; set; }
	}
}
