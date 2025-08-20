using Hybrsoft.Enums;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IAuthorizationService
	{
		bool HasPermission(Permissions permission);
	}
}
