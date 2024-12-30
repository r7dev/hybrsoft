namespace Hybrsoft.Domain.ViewModels
{
	public class UserDetailsArgs
	{
		static public UserDetailsArgs CreateDefault() => new();

		public long UserID { get; set; }

		public bool IsNew => UserID <= 0;
	}
}
