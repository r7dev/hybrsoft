namespace Hybrsoft.Domain.ViewModels
{
	public class RoleDetailsArgs
	{
		static public RoleDetailsArgs CreateDefault() => new();

		public long RoleID { get; set; }

		public bool IsNew => RoleID <= 0;
	}
}
