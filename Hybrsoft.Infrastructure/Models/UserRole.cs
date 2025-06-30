using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class UserRole
	{
		public long UserRoleID { get; set; }
		public long UserID { get; set; }
		public long RoleID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RoleID} {SearchTerms}".ToLower();

		public virtual Role Role { get; set; }
	}
}
