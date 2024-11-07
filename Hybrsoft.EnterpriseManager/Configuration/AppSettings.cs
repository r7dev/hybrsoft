﻿using Hybrsoft.Infrastructure.Enums;
using Microsoft.Extensions.Configuration;
using System;
using Windows.Storage;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	public class AppSettings
	{
		static AppSettings()
		{
			Current = new AppSettings();
		}
		static public AppSettings Current { get; }

		//public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;
		public ApplicationDataContainer LocalSettings
		{
			get
			{
				var localSettings = ApplicationData.Current.LocalSettings;
				return localSettings;
			}
		}

		public string UserName
		{
			get => GetSettingsValue("UserName", default(String));
			set => LocalSettings.Values["UserName"] = value;
		}

		public DataProviderType DataProvider
		{
			get => (DataProviderType)GetSettingsValue("DataProvider", (int)DataProviderType.SQLServer);
			set => LocalSettings.Values["DataProvider"] = (int)value;
		}

		public string SQLServerConnectionString
		{
			get
			{
				var config = new ConfigurationBuilder()
				.SetBasePath("C:\\Users\\ricar\\AppData\\Roaming\\Microsoft\\UserSecrets\\e3462127-a2fe-4121-a768-e126e4ed23f2")
				.AddJsonFile("secrets.json")
				.Build();
				return config.GetConnectionString("EnvironmentDev");
			}
			//get => GetSettingsValue("SQLServerConnectionString", @"Data Source=.\SQLExpress;Initial Catalog=VanArsdelDb;Integrated Security=SSPI");
			set => SetSettingsValue("SQLServerConnectionString", value);
		}

		public bool IsRandomErrorsEnabled
		{
			get => GetSettingsValue("IsRandomErrorsEnabled", false);
			set => LocalSettings.Values["IsRandomErrorsEnabled"] = value;
		}

		private TResult GetSettingsValue<TResult>(string name, TResult defaultValue)
		{
			try
			{
				if (!LocalSettings.Values.ContainsKey(name))
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
	}
}