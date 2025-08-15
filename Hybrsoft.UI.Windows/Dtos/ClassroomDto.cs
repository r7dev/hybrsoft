using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public partial class ClassroomDto : ObservableObject
	{
		static public ClassroomDto CreateEmpty() => new() { ClassroomID = -1, IsEmpty = true };
		public long ClassroomID { get; set; }
		public string Name { get; set; }
		public Int16 Year { get; set; }
		public Int16 MinimumYear { get; set; }
		public Int16 MaximumYear { get; set; }
		public Int16 EducationLevel { get; set; }
		public Int16 MinimumEducationLevel { get; set; }
		public Int16 MaximumEducationLevel { get; set; }
		public Int16 ScheduleTypeID { get; set; }
		public bool IsNew => ClassroomID <= 0;

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public ScheduleTypeDto ScheduleType { get; set; }

		public override void Merge(ObservableObject source)
		{
			if (source is ClassroomDto model)
			{
				Merge(model);
			}
		}

		public void Merge(ClassroomDto source)
		{
			if (source != null)
			{
				ClassroomID = source.ClassroomID;
				Name = source.Name;
				Year = source.Year;
				MinimumYear = source.MinimumYear;
				MaximumYear = source.MaximumYear;
				EducationLevel = source.EducationLevel;
				MinimumEducationLevel = source.MinimumEducationLevel;
				MaximumEducationLevel = source.MaximumEducationLevel;
				ScheduleTypeID = source.ScheduleTypeID;
				ScheduleType = source.ScheduleType;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
