namespace Hybrsoft.Domain.ViewModels
{
	public class CompanyDetailsArgs
	{
		public static CompanyDetailsArgs CreateDefault() => new();

		public long CompanyID { get; set; }

		public bool IsNew => CompanyID <= 0;
	}
}
