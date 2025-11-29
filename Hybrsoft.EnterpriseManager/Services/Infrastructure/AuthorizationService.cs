using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Services;
using System.Linq;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class AuthorizationService(ILookupTables lookupTables) : IAuthorizationService
	{
		private readonly ILookupTables _lookupTables = lookupTables;

		public bool HasPermission(Permissions permission)
		{
			return _lookupTables.Permissions.Any(r => r.Name == permission.ToString());
		}
	}
}
