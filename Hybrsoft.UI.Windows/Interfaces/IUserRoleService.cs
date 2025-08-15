using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface IUserRoleService
	{
		Task<UserRoleDto> GetUserRoleAsync(long id);
		Task<IList<UserRoleDto>> GetUserRolesAsync(DataRequest<UserRole> request);
		Task<IList<UserRoleDto>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<long>> GetAddedRoleKeysInUserAsync(long parentID);
		Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request);

		Task<int> UpdateUserRoleAsync(UserRoleDto model);

		Task<int> DeleteUserRoleAsync(UserRoleDto model);
		Task<int> DeleteUserRoleRangeAsync(int index, int length, DataRequest<UserRole> request);
	}
}
