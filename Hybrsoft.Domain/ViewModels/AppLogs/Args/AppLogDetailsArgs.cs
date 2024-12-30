namespace Hybrsoft.Domain.ViewModels
{
	public class AppLogDetailsArgs
	{
		static public AppLogDetailsArgs CreateDefault() => new();

		public long AppLogID { get; set; }
	}
}
