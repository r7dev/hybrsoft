using Hybrsoft.Enums;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IUserPermissionService
	{
		bool HasPermission(Permissions permission);
	}
}
