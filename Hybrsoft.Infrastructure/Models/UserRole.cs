using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class UserRole
	{
		public long UserRoleId { get; set; }
		public long UserId { get; set; }
		public long RoleId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RoleId} {SearchTerms}".ToLower();

		public virtual Role Role { get; set; }
	}
}
