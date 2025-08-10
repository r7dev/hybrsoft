using Hybrsoft.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
				var encryptedData = GetSettingsValue(nameof(SQLServerConnectionString), string.Empty);
				if (!string.IsNullOrEmpty(encryptedData))
				{
					byte[] decryptedData = ProtectedData.Unprotect(
						Convert.FromBase64String(encryptedData),
						null,
						DataProtectionScope.CurrentUser);
					return Encoding.UTF8.GetString(decryptedData);
				}

				return AppConfig.IsDevelopment
					? GetSQLServerDevelopmentConnectionString()
					: GetSQLServerProductionConnectionString();
			}
			set => SetSettingsValue(nameof(SQLServerConnectionString), value);
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

		private static string GetSQLServerDevelopmentConnectionString()
		{
			return new ConfigurationBuilder()
				.AddUserSecrets<AppSettings>(false, true)
				.Build()
				.GetConnectionString("SQLServerDev");
		}

		private static string GetSQLServerProductionConnectionString()
		{
			string configPath = Path.Combine(AppContext.BaseDirectory, "Configuration");
			return new ConfigurationBuilder()
				.SetBasePath(configPath)
				.AddJsonFile("appsettings.json", false, true)
				.Build()
				.GetConnectionString("SQLServerProd");
		}
	}
}
