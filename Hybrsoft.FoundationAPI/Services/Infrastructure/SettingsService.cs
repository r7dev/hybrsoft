using Hybrsoft.Enums;
using Hybrsoft.FoundationAPI.Configuration;
using Microsoft.Extensions.Options;

namespace Hybrsoft.FoundationAPI.Services
{
	public class SettingsService(IConfiguration configuration, IOptions<AppSettings> options) : ISettingsService
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly AppSettings _appSettings = options.Value;

		public DataProviderType DataProvider
		{
			get => _appSettings.DataProvider;
			set => _appSettings.DataProvider = value;
		}

		public string SQLServerConnectionString => _configuration.GetConnectionString("SQLServerProd") ?? string.Empty;
	}
}
