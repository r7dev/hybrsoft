namespace Hybrsoft.Domain.ViewModels
{
	public class PermissionDetailsArgs
	{
		public static PermissionDetailsArgs CreateDefault() => new();

		public long PermissionID { get; set; }

		public bool IsNew => PermissionID <= 0;
	}
}
