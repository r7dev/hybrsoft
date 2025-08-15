namespace Hybrsoft.UI.Windows.ViewModels
{
	public class RolePermissionDetailsArgs
	{
		public static RolePermissionDetailsArgs CreateDefault() => new();

		public long RolePermissionId { get; set; }
		public long RoleId { get; set; }

		public bool IsNew => RolePermissionId <= 0;
	}
}
