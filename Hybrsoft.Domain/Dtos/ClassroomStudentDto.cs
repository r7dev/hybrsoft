using Hybrsoft.Domain.Infrastructure.ViewModels;
using System;

namespace Hybrsoft.Domain.Dtos
{
	public partial class ClassroomStudentDto : ObservableObject
	{
		public long ClassroomStudentID { get; set; }

		public long ClassroomID { get; set; }
		public long StudentID { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? LastModifiedOn { get; set; }

		public ClassroomDto Classroom { get; set; }
		public StudentDto Student { get; set; }

		public bool IsNew => ClassroomStudentID <= 0;

		public override void Merge(ObservableObject source)
		{
			if (source is ClassroomStudentDto model)
			{
				Merge(model);
			}
		}

		public void Merge(ClassroomStudentDto source)
		{
			if (source != null)
			{
				ClassroomStudentID = source.ClassroomStudentID;
				ClassroomID = source.ClassroomID;
				Classroom = source.Classroom;
				StudentID = source.StudentID;
				Student = source.Student;
				CreatedOn = source.CreatedOn;
				LastModifiedOn = source.LastModifiedOn;
			}
		}
	}
}
