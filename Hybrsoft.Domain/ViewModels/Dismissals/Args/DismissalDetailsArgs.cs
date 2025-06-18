namespace Hybrsoft.Domain.ViewModels
{
	public class DismissalDetailsArgs
	{
		static public DismissalDetailsArgs CreateDefault() => new();

		public long DismissalID { get; set; }
		public long ClassroomID { get; set; }
		public long StudentID { get; set; }

		public bool IsNew => DismissalID <= 0;
	}
}
