using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Classroom
	{
		public long ClassroomID { get; set; }
		public string Name { get; set; }
		public short Year { get; set; }
		public short MinimumYear { get; set; }
		public short MaximumYear { get; set; }
		public short EducationLevel { get; set; }
		public short MinimumEducationLevel { get; set; }
		public short MaximumEducationLevel { get; set; }
		public short ScheduleTypeID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{ClassroomID} {Name} {Year} {SearchTerms}".ToLower();

		public virtual ScheduleType ScheduleType { get; set; }
	}
}
