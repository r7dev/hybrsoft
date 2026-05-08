using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class StudentBelonging
	{
		public long StudentBelongingID { get; set; }
		public long StudentID { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }

		public byte[] Picture { get; set; }
		public byte[] Thumbnail { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{StudentBelongingID} {DisplayName} {Description}".ToLower();

		public virtual Student Student { get; set; }
	}
}
