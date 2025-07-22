using Hybrsoft.Domain.Infrastructure.ViewModels;

namespace Hybrsoft.Domain.Dtos
{
	public partial class SubscriptionTypeDto : ObservableObject
	{
		public short SubscriptionTypeID { get; set; }
		public string DisplayName { get; set; }
	}
}
