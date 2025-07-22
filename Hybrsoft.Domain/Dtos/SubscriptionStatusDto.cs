using Hybrsoft.Domain.Infrastructure.ViewModels;

namespace Hybrsoft.Domain.Dtos
{
	public partial class SubscriptionStatusDto : ObservableObject
	{
		public short SubscriptionStatusID { get; set; }
		public string DisplayName { get; set; }
	}
}
