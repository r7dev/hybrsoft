using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class ClassroomStudent
	{
		public long ClassroomStudentId { get; set; }
		public long ClassroomId { get; set; }
		public long StudentId { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{StudentId} {SearchTerms}".ToLower();

		public virtual Classroom Classroom { get; set; }
		public virtual Student Student { get; set; }
	}
}
