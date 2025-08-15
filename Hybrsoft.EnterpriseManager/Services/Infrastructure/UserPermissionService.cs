using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Enums;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class UserPermissionService(IDataServiceFactory dataServiceFactory, ISettingsService settingsService) : IUserPermissionService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ISettingsService SettingsService { get; } = settingsService;

		public bool HasPermission(Permissions permission)
		{
			var dataService = DataServiceFactory.CreateDataService();
			return dataService.HasPermission(SettingsService.UserID, permission.ToString());
		}
	}
}
