using Hybrsoft.Infrastructure.DataContexts;
using Hybrsoft.Infrastructure.DataServices.Base;

namespace Hybrsoft.Infrastructure.DataServices
{
	public partial class SQLServerDataService(string connectionString) : DataServiceBase(new SQLServerDb(connectionString))
	{
	}
}
