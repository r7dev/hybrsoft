using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.DTOs;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces.Infrastructure
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
