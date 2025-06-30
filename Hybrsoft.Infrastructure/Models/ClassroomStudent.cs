using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class ClassroomStudent
	{
		public long ClassroomStudentID { get; set; }
		public long ClassroomID { get; set; }
		public long StudentID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }
		public string SearchTermsDismissibleStudent { get; set; }

		public string BuildSearchTerms() => $"{StudentID} {SearchTerms}".ToLower();
		public string BuildSearchTermsDismissibleStudent() => $"{ClassroomID} {SearchTermsDismissibleStudent} {SearchTerms}".ToLower();

		public virtual Classroom Classroom { get; set; }
		public virtual Student Student { get; set; }
	}
}
