using Hybrsoft.Infrastructure.DataServices;

namespace Hybrsoft.FoundationAPI.Services.DataServiceFactory
{
	public interface IDataServiceFactory
	{
		IDataService CreateDataService();
	}
}
