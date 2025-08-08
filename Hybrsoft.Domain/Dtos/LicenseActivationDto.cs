using Hybrsoft.Enums;

namespace Hybrsoft.Domain.Dtos
{
	public class LicenseActivationDto
	{
		public string Email { get; set; }
		public string LicenseKey { get; set; }
		public AppType ProductType { get; set; }
	}
}
