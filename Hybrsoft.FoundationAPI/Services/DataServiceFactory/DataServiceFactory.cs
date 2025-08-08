using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.DataServices;

namespace Hybrsoft.FoundationAPI.Services.DataServiceFactory
{
	public class DataServiceFactory(ISettingsService settingsService) : IDataServiceFactory
	{
		private readonly ISettingsService _settingsService = settingsService;

		public IDataService CreateDataService()
		{
			return _settingsService.DataProvider switch
			{
				DataProviderType.SQLServer => new SQLServerDataService(_settingsService.SQLServerConnectionString),
				_ => throw new NotImplementedException($"Data provider '{_settingsService.DataProvider}' is not implemented.")
			};
		}
	}
}
