using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Infrastructure.DataServices;
using System;

namespace Hybrsoft.EnterpriseManager.Services.DataServiceFactory
{
	public class DataServiceFactory : IDataServiceFactory
	{
		static private Random _random = new Random(0);
		public IDataService CreateDataService()
		{
			if (AppSettings.Current.IsRandomErrorsEnabled)
			{
				if (_random.Next(20) == 0)
				{
					throw new InvalidOperationException("Random error simulation");
				}
			}

			switch (AppSettings.Current.DataProvider)
			{
				//case DataProviderType.SQLite:
				//	return new SQLiteDataService(AppSettings.Current.SQLiteConnectionString);

				case DataProviderType.SQLServer:
					return new SQLServerDataService(AppSettings.Current.SQLServerConnectionString);

				default:
					throw new NotImplementedException();
			}
		}
	}
}
