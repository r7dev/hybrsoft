using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Permission
	{
		public long PermissionID { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public bool IsEnabled { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{PermissionID} {Name} {DisplayName} {Description}".ToLower();
	}
}
