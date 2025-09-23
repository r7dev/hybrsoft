namespace Hybrsoft.DTOs
{
	public class SubscriptionInfoDto
	{
		public bool IsActivated { get; set; }
		public DateTimeOffset? StartDate { get; set; }
		public DateTimeOffset? ExpirationDate { get; set; }
		public string? LicenseData { get; set; }
		public string? LicensedTo { get; set; }
		public string? Message { get; set; }
		public string? Uid { get; set; }

	}
}
