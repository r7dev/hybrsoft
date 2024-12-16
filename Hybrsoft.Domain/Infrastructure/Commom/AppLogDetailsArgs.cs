namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class AppLogDetailsArgs
	{
		static public AppLogDetailsArgs CreateDefault() => new();

		public long AppLogID { get; set; }
	}
}
