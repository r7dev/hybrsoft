using Hybrsoft.UI.Windows.Infrastructure.ViewModels;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class SubscriptionPlanDto : ObservableObject
	{
		public short SubscriptionPlanID { get; set; }
		public string Name { get; set; }
		public short DurationMonths { get; set; }
	}
}
