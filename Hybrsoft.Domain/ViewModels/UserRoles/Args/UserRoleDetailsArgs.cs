namespace Hybrsoft.Domain.ViewModels
{
	public class UserRoleDetailsArgs
	{
		public static UserRoleDetailsArgs CreateDefault() => new();

		public long UserRoleId { get; set; }
		public long UserId { get; set; }

		public bool IsNew => UserRoleId <= 0;
	}
}
