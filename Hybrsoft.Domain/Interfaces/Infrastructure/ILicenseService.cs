using Hybrsoft.Domain.Dtos;
using Hybrsoft.DTOs;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ILicenseService
	{
		Task<LicenseResponse> ActivateSubscriptionOnlineAsync(LicenseActivationDto license);

		Task<LicenseResponse> ValidateSubscriptionOnlineAsync(string email);
		void SaveLicenseLocally(SubscriptionInfoDto model);
		Task<bool> IsLicenseValidOfflineAsync();

		SubscriptionInfoDto CreateSubscriptionInfoDto(LicenseResponse source);
	}
}
