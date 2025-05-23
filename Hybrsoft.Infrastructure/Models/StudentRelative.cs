using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class StudentRelative
	{
		public long StudentRelativeId { get; set; }
		public long StudentId { get; set; }
		public long RelativeId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{RelativeId} {SearchTerms}".ToLower();

		public virtual Relative Relative { get; set; }
	}
}
