using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	public static class AppConfig
	{
		public static IConfiguration Configuration { get; private set; }
		public static ISettingsService SettingsService { get; private set; }

		public static void Initialize()
		{
			var configPath = Path.Combine(AppContext.BaseDirectory, "Configuration");

			Configuration = new ConfigurationBuilder()
				.SetBasePath(configPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			SettingsService = ServiceLocator.Current.GetService<ISettingsService>();
		}

		public static bool IsDevelopment => SettingsService.Environment == EnvironmentType.Development;

		public static string ApiBaseUrl => Configuration["AppSettings:ApiBaseUrl" + (IsDevelopment ? "Dev" : "Prod")];

	}
}
