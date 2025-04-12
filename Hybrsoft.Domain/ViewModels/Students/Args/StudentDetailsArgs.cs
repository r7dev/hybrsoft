namespace Hybrsoft.Domain.ViewModels
{
	public class StudentDetailsArgs
	{
		static public StudentDetailsArgs CreateDefault() => new();

		public long StudentID { get; set; }

		public bool IsNew => StudentID <= 0;
	}
}