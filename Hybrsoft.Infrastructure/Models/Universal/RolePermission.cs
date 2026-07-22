using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class RolePermission
	{
		public long RolePermissionID { get; set; }
		public long RoleID { get; set; }
		public long PermissionID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{PermissionID} {SearchTerms}".ToLower();

		public virtual Permission Permission { get; set; }
	}
}
