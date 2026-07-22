using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Dismissal
	{
		public long DismissalID { get; set; }
		public long ClassroomID { get; set; }
		public long StudentID { get; set; }
		public long RelativeID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? DismissedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{DismissalID} {StudentID} {SearchTerms}".ToLower();

		public virtual Classroom Classroom { get; set; }
		public virtual Student Student { get; set; }
		public virtual Relative Relative { get; set; }
	}
}
