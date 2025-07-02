using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IRolePermissionService
	{
		Task<RolePermissionDto> GetRolePermissionAsync(long id);
		Task<IList<RolePermissionDto>> GetRolePermissionsAsync(DataRequest<RolePermission> request);
		Task<IList<RolePermissionDto>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<IList<long>> GetAddedPermissionKeysInRoleAsync(long parentID);
		Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request);

		Task<int> UpdateRolePermissionAsync(RolePermissionDto model);

		Task<int> DeleteRolePermissionAsync(RolePermissionDto model);
		Task<int> DeleteRolePermissionRangeAsync(int index, int length, DataRequest<RolePermission> request);
	}
}
