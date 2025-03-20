using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class SettingsService(IDialogService dialogService) : ISettingsService
	{
		public IDialogService DialogService { get; } = dialogService;

		public string AppName => AppSettings.Current.AppName;
		public string Version => AppSettings.Current.Version;

		public long UserID
		{
			get => AppSettings.Current.UserID;
			set => AppSettings.Current.UserID = value;
		}

		public string UserName
		{
			get => AppSettings.Current.UserName;
			set => AppSettings.Current.UserName = value;
		}

		public char PasswordChar
		{
			get => AppSettings.Current.PasswordChar;
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
