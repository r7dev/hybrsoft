using Hybrsoft.Enums;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Infrastructure.DataServices;
using System;

namespace Hybrsoft.EnterpriseManager.Services.DataServiceFactory
{
	public class DataServiceFactory : IDataServiceFactory
	{
		private static readonly Random _random = new(0);
		public IDataService CreateDataService()
		{
			if (AppSettings.Current.IsRandomErrorsEnabled)
			{
				if (_random.Next(20) == 0)
				{
					throw new InvalidOperationException("Random error simulation");
				}
			}

			return AppSettings.Current.DataProvider switch
			{
				DataProviderType.SQLServer => new SQLServerDataService(AppSettings.Current.SQLServerConnectionString),
				_ => throw new NotImplementedException()
			};
		}
	}
}
