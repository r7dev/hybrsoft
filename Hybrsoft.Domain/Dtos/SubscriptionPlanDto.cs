using Hybrsoft.Domain.Infrastructure.ViewModels;

namespace Hybrsoft.Domain.Dtos
{
	public partial class SubscriptionPlanDto : ObservableObject
	{
		public short SubscriptionPlanID { get; set; }
		public string Name { get; set; }
		public short DurationMonths { get; set; }
	}
}
