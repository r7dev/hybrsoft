using Hybrsoft.Infrastructure.DataServices;

namespace Hybrsoft.EnterpriseManager.Services.DataServiceFactory
{
	public interface IDataServiceFactory
	{
		IDataService CreateDataService();
	}
}
