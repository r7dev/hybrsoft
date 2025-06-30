using System;

namespace Hybrsoft.Infrastructure.Models
{
	public class Classroom
	{
		public long ClassroomID { get; set; }
		public string Name { get; set; }
		public Int16 Year { get; set; }
		public Int16 MinimumYear { get; set; }
		public Int16 MaximumYear { get; set; }
		public Int16 EducationLevel { get; set; }
		public Int16 MinimumEducationLevel { get; set; }
		public Int16 MaximumEducationLevel { get; set; }
		public Int16 ScheduleTypeID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }
		public string SearchTerms { get; set; }

		public string BuildSearchTerms() => $"{ClassroomID} {Name} {Year} {SearchTerms}".ToLower();

		public virtual ScheduleType ScheduleType { get; set; }
	}
}
