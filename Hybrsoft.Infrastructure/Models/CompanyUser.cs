using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class CompanyUser
	{
		public long CompanyUserID { get; set; }
		public long CompanyID { get; set; }
		public long UserID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{UserID} {SearchTerms}".ToLower();

		public virtual Company Company { get; set; }
		public virtual User User { get; set; }
	}
}
