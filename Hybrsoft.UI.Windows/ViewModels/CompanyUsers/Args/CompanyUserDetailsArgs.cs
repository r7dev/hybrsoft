namespace Hybrsoft.UI.Windows.ViewModels
{
	public class CompanyUserDetailsArgs
	{
		public static CompanyUserDetailsArgs CreateDefault() => new();

		public long CompanyUserID { get; set; }
		public long CompanyID { get; set; }

		public bool IsNew => CompanyUserID <= 0;
	}
}
