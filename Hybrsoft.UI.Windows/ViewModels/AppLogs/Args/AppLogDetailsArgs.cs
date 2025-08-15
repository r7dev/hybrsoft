namespace Hybrsoft.UI.Windows.ViewModels
{
	public class AppLogDetailsArgs
	{
		public static AppLogDetailsArgs CreateDefault() => new();

		public long AppLogID { get; set; }
	}
}
