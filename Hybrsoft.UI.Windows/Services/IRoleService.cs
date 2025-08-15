using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IRoleService
	{
		Task<RoleModel> GetRoleAsync(long id);
		Task<IList<RoleModel>> GetRolesAsync(DataRequest<Role> request);
		Task<IList<RoleModel>> GetRolesAsync(int skip, int take, DataRequest<Role> request);
		Task<int> GetRolesCountAsync(DataRequest<Role> request);

		Task<int> UpdateRoleAsync(RoleModel model);

		Task<int> DeleteRoleAsync(RoleModel model);
		Task<int> DeleteRoleRangeAsync(int index, int length, DataRequest<Role> request);
	}
}
