using Hybrsoft.Enums;

namespace Hybrsoft.UI.Windows.Interfaces.Infrastructure
{
	public interface IUserPermissionService
	{
		bool HasPermission(Permissions permission);
	}
}
