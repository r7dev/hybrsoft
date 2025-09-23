﻿using Hybrsoft.DTOs;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class LicenseService(
		IUserService userService,
		IWindowsSecurityService windowsSecurityService,
		INetworkService networkService,
		ISettingsService settingsService,
		ILogService logService) : ILicenseService
	{
		private const string _licenseData = "LicenseData";
		private const string _lastLicenseSync = "LastLicenseSync";
		private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
		private string _jwtToken;

		private readonly IUserService _userService = userService;
		private readonly IWindowsSecurityService _windowsSecurityService = windowsSecurityService;
		public INetworkService NetworkService { get; } = networkService;
		public ISettingsService SettingsService { get; } = settingsService;
		public ILogService LogService { get; } = logService;

		public async Task<LicenseResponse> ActivateSubscriptionOnlineAsync(LicenseActivationModel license)
		{
			try
			{
				_jwtToken = await AuthenticateAsync(license.Email, license.Password);
				if (string.IsNullOrEmpty(_jwtToken))
				{
					return new LicenseResponse { IsActivated = false, Message = "Authentication failed." };
				}

				var client = NetworkService.GetHttpClient(_jwtToken);
				var payload = new { license.Email, license.LicenseKey, ProductType = AppType.EnterpriseManager };
				var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
				var response = await client.PostAsync(AppConfig.ApiBaseUrl + "/subscription/activate", content);
				var json = await response.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<LicenseResponse>(json, _jsonOptions);
			}
			catch (Exception ex)
			{
				await LogService.WriteAsync(LogType.Error, "License", "Activate", ex.Message, ex.ToString());
				return new LicenseResponse { IsActivated = false };
			}
		}

		public async Task<LicenseResponse> ValidateSubscriptionOnlineAsync(string email, string password)
		{
			try
			{
				_jwtToken = await AuthenticateAsync(email, password);
				if (string.IsNullOrEmpty(_jwtToken))
				{
					return new LicenseResponse { IsActivated = false, Message = "Authentication failed." };
				}

				var client  = NetworkService.GetHttpClient(_jwtToken);
				var payload = new { Email = email, ProductType = AppType.EnterpriseManager };
				var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
				var response = await client.PostAsync(AppConfig.ApiBaseUrl + "/subscription/validate", content);
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
			var encryptedData = _windowsSecurityService.EncryptData(model);
			SettingsService.SaveSettingAsync(_licenseData, encryptedData);
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
					var license = _windowsSecurityService.DecryptData<SubscriptionInfoDto>(encryptedData);
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
				LicensedTo = source.LicensedTo,
				Message = source.Message,
			};
		}

		private async Task<string> AuthenticateAsync(string email, string password)
		{
			try
			{
				if (IsTokenValid())
				{
					return AppSettings.Current.JwtToken;
				}

				bool isPasswordEncrypted = false;
				if (string.IsNullOrEmpty(password))
				{
					var user = await _userService.GetUserByEmailAsync(email, true);
					var parts = user?.Password?.Split('-');
					password = parts?.Length > 1 ? parts[0] : user?.Password;
					isPasswordEncrypted = true;
				}

				var client = NetworkService.GetHttpClient();
				var payload = new
				{
					Username = email,
					Password = password,
					IsEncrypted = isPasswordEncrypted
				};
				var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
				var response = await client.PostAsync(AppConfig.ApiBaseUrl + "/auth/login", content);
				var json = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<AuthenticateResponse>(json, _jsonOptions);

				AppSettings.Current.JwtToken = result?.Token;
				var handler = new JwtSecurityTokenHandler();
				var jwt = handler.ReadJwtToken(result?.Token);
				AppSettings.Current.JwtTokenExpiration = jwt.ValidTo.ToLocalTime();

				return result?.Token;
			}
			catch (Exception ex)
			{
				await LogService.WriteAsync(LogType.Error, "License", "Authenticate", ex.Message, ex.ToString());
				return null;
			}
		}

		private static bool IsTokenValid()
		{
			return !string.IsNullOrEmpty(AppSettings.Current.JwtToken)
				&& AppSettings.Current.JwtTokenExpiration > DateTime.Now;
		}
	}
}
