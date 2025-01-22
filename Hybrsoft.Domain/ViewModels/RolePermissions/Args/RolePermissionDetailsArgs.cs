namespace Hybrsoft.Domain.ViewModels
{
	public class RolePermissionDetailsArgs
	{
		static public RolePermissionDetailsArgs CreateDefault() => new();

		public long RolePermissionId { get; set; }
		public long RoleId { get; set; }

		public bool IsNew => RolePermissionId <= 0;
	}
}
