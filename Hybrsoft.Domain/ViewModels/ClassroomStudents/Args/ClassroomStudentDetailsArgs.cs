namespace Hybrsoft.Domain.ViewModels
{
	public class ClassroomStudentDetailsArgs
	{
		static public ClassroomStudentDetailsArgs CreateDefault() => new();

		public long ClassroomStudentID { get; set; }
		public long ClassroomID { get; set; }

		public bool IsNew => ClassroomStudentID <= 0;
	}
}
