using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.DTOs;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class LicenseService(
		INetworkService networkService,
		ISettingsService settingsService,
		ILogService logService) : ILicenseService
	{
		private const string _licenseData = "LicenseData";
		private const string _lastLicenseSync = "LastLicenseSync";
		private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

		public INetworkService NetworkService { get; } = networkService;
		public ISettingsService SettingsService { get; } = settingsService;
		public ILogService LogService { get; } = logService;

		public async Task<LicenseResponse> ActivateSubscriptionOnlineAsync(LicenseActivationDto license)
		{
			try
			{
				var client = NetworkService.GetHttpClient();
				var payload = new { license.Email, license.LicenseKey, ProductType = AppType.EnterpriseManager };
				var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
				var response = await client.PostAsync(AppConfig.ApiBaseUrl + "subscription/activate", content);
				var json = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<LicenseResponse>(json, _jsonOptions);
			}
			catch (Exception ex)
			{
				await LogService.WriteAsync(LogType.Error, "License", "Activate", ex.Message, ex.ToString());
				return new LicenseResponse { IsActivated = false };
			}
		}

		public async Task<LicenseResponse> ValidateSubscriptionOnlineAsync(string email)
		{
			try
			{
				var client  = NetworkService.GetHttpClient();
				var payload = new { Email = email, ProductType = AppType.EnterpriseManager };
				var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
				var response = await client.PostAsync(AppConfig.ApiBaseUrl + "subscription/validate", content);
				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<LicenseResponse>(json, _jsonOptions);
				}
				return new LicenseResponse { IsActivated = false };
			}
			catch (Exception ex)
			{
				await LogService.WriteAsync(LogType.Error, "License", "Validate Online", ex.Message, ex.ToString());
				return new LicenseResponse { IsActivated = false };
			}
		}

		public void SaveLicenseLocally(SubscriptionInfoDto model)
		{
			byte[] encryptedData = ProtectedData.Protect(
				Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)),
				null,
				DataProtectionScope.CurrentUser);

			SettingsService.SaveSettingAsync(_licenseData, Convert.ToBase64String(encryptedData));
			SettingsService.SaveSettingAsync(_lastLicenseSync, DateTimeOffset.Now.ToString("o"));
		}

		public async Task<bool> IsLicenseValidOfflineAsync()
		{
			var encryptedData = await SettingsService.ReadSettingAsync<string>(_licenseData);
			var lastSyncStr = await SettingsService.ReadSettingAsync<string>(_lastLicenseSync);
			if (string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(lastSyncStr))
				return false;

			if (DateTimeOffset.TryParse(lastSyncStr, out DateTimeOffset lastSync))
			{
				// Allow offline use for 7 days
				if ((DateTimeOffset.Now - lastSync).TotalDays > 7)
					return false;

				try
				{
					byte[] decryptedData = ProtectedData.Unprotect(
						Convert.FromBase64String(encryptedData),
						null,
						DataProtectionScope.CurrentUser);
					var license = JsonSerializer.Deserialize<SubscriptionInfoDto>(Encoding.UTF8.GetString(decryptedData));
					return license.IsActivated && license.ExpirationDate > DateTimeOffset.Now;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		public SubscriptionInfoDto CreateSubscriptionInfoDto(LicenseResponse source)
		{
			return new SubscriptionInfoDto
			{
				IsActivated = source.IsActivated,
				StartDate = source.StartDate,
				ExpirationDate = source.ExpirationDate,
				LicenseData = source.LicenseData,
				Message = source.Message,
			};
		}
	}
}
