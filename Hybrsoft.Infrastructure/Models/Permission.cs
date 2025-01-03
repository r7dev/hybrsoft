using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Permission
	{
		public long PermissionId { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public bool IsEnabled { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{PermissionId} {Name} {DisplayName} {Description}".ToLower();
	}
}
