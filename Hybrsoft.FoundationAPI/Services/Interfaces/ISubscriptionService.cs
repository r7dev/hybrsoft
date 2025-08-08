using Hybrsoft.DTOs;
using Hybrsoft.Enums;

namespace Hybrsoft.FoundationAPI.Services
{
	public interface ISubscriptionService
	{
		public Task<SubscriptionInfoDto> Activate(string licenseKey);
		public Task<SubscriptionInfoDto> Validate(string email, AppType productType);
	}
}
