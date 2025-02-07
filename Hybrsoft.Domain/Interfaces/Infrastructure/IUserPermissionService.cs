using Hybrsoft.Infrastructure.Enums;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface IUserPermissionService
	{
		bool HasPermission(long userId, Permissions permission);
	}
}
