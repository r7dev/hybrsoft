using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Dismissal
	{
		public long DismissalId { get; set; }
		public long ClassroomId { get; set; }
		public long StudentId { get; set; }
		public long RelativeId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? DismissedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{DismissalId} {StudentId} {SearchTerms}".ToLower();

		public virtual Classroom Classroom { get; set; }
		public virtual Student Student { get; set; }
		public virtual Relative Relative { get; set; }
	}
}
