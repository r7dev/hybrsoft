using Hybrsoft.Infrastructure.Enums;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface IUserPermissionService
	{
		bool HasPermission(Permissions permission);
	}
}
