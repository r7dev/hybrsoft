using Hybrsoft.DTOs;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Services;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class SettingsService(IWindowsSecurityService windowsSecurityService) : ISettingsService
	{
		private readonly IWindowsSecurityService _windowsSecurityService = windowsSecurityService;

		public string AppName => AppSettings.Current.AppName;
		public string Version => AppSettings.Current.Version;

		public string UserName
		{
			get => AppSettings.Current.UserName;
			set => AppSettings.Current.UserName = value;
		}

		public char PasswordChar
		{
			get => AppSettings.Current.PasswordChar;
		}

		public EnvironmentType Environment
		{
			get => ReadSettingAsync<EnvironmentType>(nameof(Environment)).Result;
			set => SaveSettingAsync<EnvironmentType>(nameof(Environment), value).Wait();
		}

		public async Task<string> GetLicensedToAsync()
		{
			var encryptedData = await ReadSettingAsync<string>("LicenseData");
			var license = _windowsSecurityService.DecryptData<SubscriptionInfoDto>(encryptedData);
			return license.LicensedTo;
		}

		public async Task<T> ReadSettingAsync<T>(string key)
		{
			if (AppSettings.Current.LocalSettings.Values.TryGetValue(key, out object value))
			{
				using MemoryStream memoryStream = new(Encoding.UTF8.GetBytes((string)value));
				return await JsonSerializer.DeserializeAsync<T>(memoryStream);
			}

			return default;
		}

		public async Task SaveSettingAsync<T>(string key, T value)
		{
			using MemoryStream memoryStream = new();
			await JsonSerializer.SerializeAsync(memoryStream, value);
			memoryStream.Position = 0; // Reset the stream position

			using StreamReader reader = new(memoryStream);
			string jsonString = await reader.ReadToEndAsync();
			AppSettings.Current.LocalSettings.Values[key] = jsonString;
		}
	}
}
