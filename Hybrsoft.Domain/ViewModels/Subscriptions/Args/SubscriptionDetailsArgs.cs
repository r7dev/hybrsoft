namespace Hybrsoft.Domain.ViewModels
{
	public class SubscriptionDetailsArgs
	{
		public static SubscriptionDetailsArgs CreateDefault() => new();

		public long SubscriptionID { get; set; }

		public bool IsNew => SubscriptionID <= 0;
	}
}
