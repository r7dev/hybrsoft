using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IUserRoleService
	{
		Task<UserRoleModel> GetUserRoleAsync(long id);
		Task<IList<UserRoleModel>> GetUserRolesAsync(DataRequest<UserRole> request);
		Task<IList<UserRoleModel>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<long>> GetAddedRoleKeysInUserAsync(long parentID);
		Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request);

		Task<int> UpdateUserRoleAsync(UserRoleModel model);

		Task<int> DeleteUserRoleAsync(UserRoleModel model);
		Task<int> DeleteUserRoleRangeAsync(int index, int length, DataRequest<UserRole> request);
	}
}
