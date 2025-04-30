namespace Hybrsoft.Domain.ViewModels
{
	public class ClassroomDetailsArgs
	{
		static public ClassroomDetailsArgs CreateDefault() => new();

		public long ClassroomID { get; set; }

		public bool IsNew => ClassroomID <= 0;
	}
}
