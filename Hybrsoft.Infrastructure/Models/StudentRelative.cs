using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class StudentRelative
	{
		public long StudentRelativeID { get; set; }
		public long StudentID { get; set; }
		public long RelativeID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RelativeID} {SearchTerms}".ToLower();

		public virtual Relative Relative { get; set; }
	}
}
