using Hybrsoft.Enums;
using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Subscription
	{
		public long SubscriptionID { get; set; }
		public short SubscriptionPlanID { get; set; }
		public short DurationDays { get; set; }
		public DateTimeOffset? StartDate { get; set; }
		public DateTimeOffset? ExpirationDate { get; set; }
		public SubscriptionType Type { get; set; }
		public long? CompanyID { get; set; }
		public long? UserID { get; set; }
		public string LicenseKey { get; set; }
		public SubscriptionStatus Status { get; set; }
		public DateTimeOffset? CancelledOn { get; set; }
		public DateTimeOffset? LastValidatedOn { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{SubscriptionID} {LicenseKey} {SearchTerms}".ToLower();

		public virtual Company Company { get; set; }
		public virtual User User { get; set; }
	}
}
