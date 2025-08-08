using Hybrsoft.Enums;

namespace Hybrsoft.DTOs
{
	public class ActivationRequest
	{
		public required string Email { get; set; }
		public required string LicenseKey { get; set; }
		public required AppType ProductType { get; set; }
	}
}
