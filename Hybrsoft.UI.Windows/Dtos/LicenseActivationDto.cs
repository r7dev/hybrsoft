using Hybrsoft.Enums;

namespace Hybrsoft.UI.Windows.Dtos
{
	public class LicenseActivationDto
	{
		public string Email { get; set; }
		public string LicenseKey { get; set; }
		public AppType ProductType { get; set; }
	}
}
