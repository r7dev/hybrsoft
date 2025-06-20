using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class RolePermission
	{
		public long RolePermissionId { get; set; }
		public long RoleId { get; set; }
		public long PermissionId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{PermissionId} {SearchTerms}".ToLower();

		public virtual Permission Permission { get; set; }
	}
}
