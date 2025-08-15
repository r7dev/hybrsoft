namespace Hybrsoft.UI.Windows.ViewModels
{
	public class ClassroomStudentDetailsArgs
	{
		public static ClassroomStudentDetailsArgs CreateDefault() => new();

		public long ClassroomStudentID { get; set; }
		public long ClassroomID { get; set; }

		public bool IsNew => ClassroomStudentID <= 0;
	}
}
