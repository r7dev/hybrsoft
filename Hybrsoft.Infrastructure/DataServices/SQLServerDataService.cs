using Hybrsoft.Infrastructure.DataContexts;
using Hybrsoft.Infrastructure.DataServices.Base;

namespace Hybrsoft.Infrastructure.DataServices
{
	public class SQLServerDataService : DataServiceBase
	{
		public SQLServerDataService(string connectionString)
			: base(new SQLServerDb(connectionString))
		{
		}
	}
}
