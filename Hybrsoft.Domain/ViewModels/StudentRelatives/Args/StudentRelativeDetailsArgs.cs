namespace Hybrsoft.Domain.ViewModels
{
	public class StudentRelativeDetailsArgs
	{
		static public StudentRelativeDetailsArgs CreateDefault() => new();

		public long StudentRelativeID { get; set; }
		public long StudentID { get; set; }

		public bool IsNew => StudentRelativeID <= 0;
	}
}
