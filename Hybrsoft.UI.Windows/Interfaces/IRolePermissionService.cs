using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IRolePermissionService
	{
		Task<RolePermissionModel> GetRolePermissionAsync(long id);
		Task<IList<RolePermissionModel>> GetRolePermissionsAsync(DataRequest<RolePermission> request);
		Task<IList<RolePermissionModel>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<IList<long>> GetAddedPermissionKeysInRoleAsync(long parentID);
		Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request);

		Task<int> UpdateRolePermissionAsync(RolePermissionModel model);

		Task<int> DeleteRolePermissionAsync(RolePermissionModel model);
		Task<int> DeleteRolePermissionRangeAsync(int index, int length, DataRequest<RolePermission> request);
	}
}
