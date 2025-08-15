using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IRoleService
	{
		Task<RoleDto> GetRoleAsync(long id);
		Task<IList<RoleDto>> GetRolesAsync(DataRequest<Role> request);
		Task<IList<RoleDto>> GetRolesAsync(int skip, int take, DataRequest<Role> request);
		Task<int> GetRolesCountAsync(DataRequest<Role> request);

		Task<int> UpdateRoleAsync(RoleDto model);

		Task<int> DeleteRoleAsync(RoleDto model);
		Task<int> DeleteRoleRangeAsync(int index, int length, DataRequest<Role> request);
	}
}
