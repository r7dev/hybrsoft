namespace Hybrsoft.UI.Windows.ViewModels
{
	public class LostAndFoundDetailsArgs
	{
		public static LostAndFoundDetailsArgs CreateDefault() => new();

		public long LostAndFoundID { get; set; }

		public bool IsNew => LostAndFoundID <= 0;
	}
}
