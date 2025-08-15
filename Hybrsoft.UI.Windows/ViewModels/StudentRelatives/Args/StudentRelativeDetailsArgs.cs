namespace Hybrsoft.UI.Windows.ViewModels
{
	public class StudentRelativeDetailsArgs
	{
		public static StudentRelativeDetailsArgs CreateDefault() => new();

		public long StudentRelativeID { get; set; }
		public long StudentID { get; set; }

		public bool IsNew => StudentRelativeID <= 0;
	}
}
