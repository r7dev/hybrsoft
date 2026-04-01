namespace Hybrsoft.UI.Windows.ViewModels
{
	public class StudentBelongingDetailsArgs
	{
		public static StudentBelongingDetailsArgs CreateDefault() => new();

		public long StudentBelongingID { get; set; }
		public long StudentID { get; set; }

		public bool IsNew => StudentBelongingID <= 0;
	}
}
