namespace Hybrsoft.UI.Windows.ViewModels
{
	public class DismissalDetailsArgs
	{
		public static DismissalDetailsArgs CreateDefault() => new();

		public long DismissalID { get; set; }
		public long ClassroomID { get; set; }
		public long StudentID { get; set; }

		public bool IsNew => DismissalID <= 0;
	}
}
