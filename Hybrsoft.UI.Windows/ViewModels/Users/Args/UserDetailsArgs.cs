namespace Hybrsoft.UI.Windows.ViewModels
{
	public class UserDetailsArgs
	{
		public static UserDetailsArgs CreateDefault() => new();

		public long UserID { get; set; }

		public bool IsNew => UserID <= 0;
	}
}
