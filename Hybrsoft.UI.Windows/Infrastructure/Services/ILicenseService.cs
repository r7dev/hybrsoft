using Hybrsoft.UI.Windows.Models;
using Hybrsoft.DTOs;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ILicenseService
	{
		Task<LicenseResponse> ActivateSubscriptionOnlineAsync(LicenseActivationModel license);

		Task<LicenseResponse> ValidateSubscriptionOnlineAsync(string email, string password);
		void SaveLicenseLocally(SubscriptionInfoDto model);
		Task<bool> IsLicenseValidOfflineAsync();

		SubscriptionInfoDto CreateSubscriptionInfoDto(LicenseResponse source);
	}
}
