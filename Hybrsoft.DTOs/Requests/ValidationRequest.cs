using Hybrsoft.Enums;

namespace Hybrsoft.DTOs
{
	public class ValidationRequest
	{
		public required string Email { get; set; }
		public required AppType	ProductType { get; set; }
	}
}
