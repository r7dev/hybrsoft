using System;
using System.Collections.Generic;

namespace Hybrsoft.Infrastructure.Models
{
	public class Role
	{
		public long RoleId { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RoleId} {Name}".ToLower();

		public virtual ICollection<RolePermission> RolePermissions { get; set; }
	}
}
