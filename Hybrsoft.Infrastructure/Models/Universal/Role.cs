using System;
using System.Collections.Generic;

namespace Hybrsoft.Infrastructure.Models
{
	public class Role
	{
		public long RoleID { get; set; }
		public string Name { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RoleID} {Name}".ToLower();
	}
}
