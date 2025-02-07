using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Enums;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class UserPermissionService(IDataServiceFactory dataServiceFactory) : IUserPermissionService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public bool HasPermission(long userId, Permissions permission)
		{
			var dataService = DataServiceFactory.CreateDataService();
			return dataService.HasPermission(userId, permission.ToString());
		}
	}
}
