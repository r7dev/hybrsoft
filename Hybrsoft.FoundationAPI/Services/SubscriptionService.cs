using Hybrsoft.DTOs;
using Hybrsoft.Enums;
using Hybrsoft.FoundationAPI.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;

namespace Hybrsoft.FoundationAPI.Services
{
	public class SubscriptionService(IDataServiceFactory dataServiceFactory) : ISubscriptionService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private const string _uidPrefix = $"{nameof(FoundationAPI)}_{nameof(SubscriptionService)}_";

		public async Task<SubscriptionInfoDto> Activate(string licenseKey)
		{
			if (string.IsNullOrWhiteSpace(licenseKey))
			{
				return new SubscriptionInfoDto
				{
					IsActivated = false,
					Message = "Activation Failed. License key is required.",
					Uid =  $"{_uidPrefix}ActivationFailed_LicenseKeyIsRequired"
				};
			}

			var dataRequest = new DataRequest<Subscription>
			{
				Where = r => r.LicenseKey == licenseKey
					&& (r.Status == SubscriptionStatus.WaitingActivation
					|| r.Status == SubscriptionStatus.Active),
			};
			var dataService = _dataServiceFactory.CreateDataService();
			var subscriptions = await dataService.GetSubscriptionsAsync(0, 5, dataRequest);
			if (subscriptions == null || !subscriptions.Any())
			{
				return new SubscriptionInfoDto
				{
					IsActivated = false,
					Message = "Invalid license key.",
					Uid = $"{_uidPrefix}InvalidLicenseKey"
				};
			}
			var subscription = subscriptions
				.Where(r => r.Status == SubscriptionStatus.WaitingActivation)
				.FirstOrDefault();
			if (subscription == null)
			{
				subscription = subscriptions.FirstOrDefault(r => r.Status == SubscriptionStatus.Active);
				if (subscription != null)
				{
					subscription = await dataService.GetSubscriptionAsync(subscription.SubscriptionID);
				}
				return new SubscriptionInfoDto
				{
					IsActivated = true,
					StartDate = subscription?.StartDate,
					ExpirationDate = subscription?.ExpirationDate,
					Message = "License key is already active.",
					Uid = $"{_uidPrefix}LicenseKeyIsAlreadyActive"
				};
			}
			else
			{
				subscription = await dataService.GetSubscriptionAsync(subscription.SubscriptionID);
				var today = DateTimeOffset.Now;
				subscription.StartDate = today;
				subscription.ExpirationDate = today.AddDays(subscription.DurationDays);
				subscription.LastValidatedOn = today;
				subscription.SearchTerms = BuildSearchTerms(subscription);
				await dataService.UpdateSubscriptionAsync(subscription);
			}

			return new SubscriptionInfoDto
			{
				IsActivated = true,
				StartDate = subscription.StartDate,
				ExpirationDate = subscription.ExpirationDate,
				Message = "Your license has been successfully activated.",
				Uid = $"{_uidPrefix}YourLicenseHasBeenSuccessfullyActivated"
			};
		}

		public async Task<SubscriptionInfoDto> Validate(string email, AppType productType)
		{
			var dataRequest = new DataRequest<CompanyUser>
			{
				Where = r => r.User.Email == email,
			};
			var dataService = _dataServiceFactory.CreateDataService();
			var companyUsers = await dataService.GetCompanyUsersAsync(0, 5, dataRequest);
			var companyUser = companyUsers.FirstOrDefault();

			DateTimeOffset today = DateTimeOffset.Now;
			var dataRequestSubscription = new DataRequest<Subscription>
			{
				Where = r => (r.User.Email == email || r.CompanyID == companyUser.CompanyID)
					&& r.Status == SubscriptionStatus.Active
					&& r.ExpirationDate > today,
			};
			IList<Subscription> subscriptions = await dataService.GetSubscriptionsAsync(0, 5, dataRequestSubscription);
			if (subscriptions == null || !subscriptions.Any())
			{
				return new SubscriptionInfoDto { IsActivated = false, Message = "No active subscription found." };
			}
			var subscription = subscriptions.FirstOrDefault();
			if (subscription != null)
			{
				subscription = await dataService.GetSubscriptionAsync(subscription.SubscriptionID);
				subscription.LastValidatedOn = today;
				subscription.SearchTerms = BuildSearchTerms(subscription);
				await dataService.UpdateSubscriptionAsync(subscription);
			}
			return new SubscriptionInfoDto
			{
				IsActivated = true,
				StartDate = subscription?.StartDate,
				ExpirationDate = subscription?.ExpirationDate,
				LicenseData = subscription?.LicenseKey,
				Message = "Subscription is valid."
			};
		}

		private static string BuildSearchTerms(Subscription subscription)
		{
			if (subscription == null) return string.Empty;
			return subscription.Type == SubscriptionType.Enterprise
				? $"{subscription.Company.LegalName} ({subscription.Company.TradeName})"
				: $"{subscription.User.FirstName} {subscription.User.LastName}";
		}
	}
}
