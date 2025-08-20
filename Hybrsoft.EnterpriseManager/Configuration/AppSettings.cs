using Hybrsoft.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics;
using Windows.Storage;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	public class AppSettings
	{
		static AppSettings()
		{
			Current = new AppSettings();
		}
		public static AppSettings Current { get; }

		public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

		public string AppName => AppInfo.Current.DisplayInfo.DisplayName;
		public string Version
		{
			get
			{
				var ver = Package.Current.Id.Version;
				return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
			}
		}

		public string Language
		{
			get => GetSettingsValue(nameof(Language), "\"en-US\"");
			set => SetSettingsValue(nameof(Language), value);
		}

		public SizeInt32 WindowSizeDefault
		{
			get => new() { Width = WindowWidth, Height = WindowHeight };
			set { WindowWidth = value.Width; WindowHeight = value.Height; }
		}
		private int WindowWidth
		{
			get => GetSettingsValue(nameof(WindowWidth), 1280);
			set => SetSettingsValue(nameof(WindowWidth), value);
		}
		private int WindowHeight
		{
			get => GetSettingsValue(nameof(WindowHeight), 840);
			set => SetSettingsValue(nameof(WindowHeight), value);
		}

		public long UserID
		{
			get => GetSettingsValue(nameof(UserID), default(long));
			set => SetSettingsValue(nameof(UserID), value);
		}

		public string UserName
		{
			get => GetSettingsValue(nameof(UserName), default(String));
			set => SetSettingsValue(nameof(UserName), value);
		}

		public string WindowsHelloPublicKeyHint
		{
			get => GetSettingsValue(nameof(WindowsHelloPublicKeyHint), default(String));
			set => SetSettingsValue(nameof(WindowsHelloPublicKeyHint), value);
		}

		public DataProviderType DataProvider
		{
			get => (DataProviderType)GetSettingsValue(nameof(DataProvider), (int)DataProviderType.SQLServer);
			set => SetSettingsValue(nameof(DataProvider), (int)value);
		}

		public string SQLServerConnectionString
		{
			get
			{
				string defaultConnectionString = GetDefaultSQLServerConnectionString(AppConfig.IsDevelopment);
				var valueEncrypted = GetSettingsValueAsync(nameof(SQLServerConnectionString), EncryptData(defaultConnectionString)).Result;
				return DecryptData<string>(valueEncrypted);
			}
			set => SetSettingsValueAsync(nameof(SQLServerConnectionString), EncryptData(value));
		}

		public bool IsRandomErrorsEnabled
		{
			get => GetSettingsValue(nameof(IsRandomErrorsEnabled), false);
			set => SetSettingsValue(nameof(IsRandomErrorsEnabled), value);
		}

		public char PasswordChar
		{
			get => GetSettingsValue(nameof(PasswordChar), '*');
			set => SetSettingsValue(nameof(PasswordChar), value);
		}

		public string JwtToken
		{
			get
			{
				var encryptedData = GetSettingsValueAsync(nameof(JwtToken), EncryptData(default(string))).Result;
				return DecryptData<string>(encryptedData);
			}
			set => SetSettingsValueAsync(nameof(JwtToken), EncryptData(value));
		}

		public DateTimeOffset JwtTokenExpiration
		{
			get
			{
				var encryptedData = GetSettingsValueAsync(nameof(JwtTokenExpiration), EncryptData(DateTimeOffset.MinValue)).Result;
				return DecryptData<DateTimeOffset>(encryptedData);
			}
			set => SetSettingsValueAsync(nameof(JwtTokenExpiration), EncryptData(value));
		}

		private TResult GetSettingsValue<TResult>(string name, TResult defaultValue)
		{
			try
			{
				if (!LocalSettings.Values.TryGetValue(name, out object value))
				{
					LocalSettings.Values[name] = defaultValue;
				}
				return (TResult)LocalSettings.Values[name];
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return defaultValue;
			}
		}
		private void SetSettingsValue(string name, object value)
		{
			LocalSettings.Values[name] = value;
		}

		private async Task<TResult> GetSettingsValueAsync<TResult>(string key, TResult defaultValue)
		{
			try
			{
				if (LocalSettings.Values.TryGetValue(key, out object value))
				{
					using MemoryStream memoryStream = new(Encoding.UTF8.GetBytes((string)value));
					return await JsonSerializer.DeserializeAsync<TResult>(memoryStream);
				}

				SetSettingsValueAsync(key, defaultValue);
				return defaultValue;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return defaultValue;
			}
		}
		private async void SetSettingsValueAsync(string key, object value)
		{
			try
			{
				using MemoryStream memoryStream = new();
				await JsonSerializer.SerializeAsync(memoryStream, value);
				memoryStream.Position = 0; // Reset the stream position

				using StreamReader reader = new(memoryStream);
				string jsonString = await reader.ReadToEndAsync();
				LocalSettings.Values[key] = jsonString;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		private static string EncryptData(object value)
		{
			byte[] encryptedData = ProtectedData.Protect(
				Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)),
				null,
				DataProtectionScope.CurrentUser);

			return Convert.ToBase64String(encryptedData);
		}

		private static TResult DecryptData<TResult>(string encryptedData)
		{
			if (string.IsNullOrEmpty(encryptedData))
			{
				return default;
			}
			// Decrypt the data using ProtectedData
			// Note: Ensure that the encryptedData is in Base64 format before calling this method
			byte[] decryptedData = ProtectedData.Unprotect(
				Convert.FromBase64String(encryptedData),
				null,
				DataProtectionScope.CurrentUser);
			return JsonSerializer.Deserialize<TResult>(Encoding.UTF8.GetString(decryptedData));
		}

		private static string GetDefaultSQLServerConnectionString(bool isDevelopment)
		{
			if (isDevelopment)
			{
				return new ConfigurationBuilder()
					.AddUserSecrets<AppSettings>(false, true)
					.Build()
					.GetConnectionString("SQLServerDev");
			}
			return AppConfig.SQLServerProdConnectionString;
		}
	}
}
