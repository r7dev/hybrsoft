using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IUserRoleService
	{
		Task<UserRoleDto> GetUserRoleAsync(long userRoleId);
		Task<IList<UserRoleDto>> GetUserRolesAsync(DataRequest<UserRole> request);
		Task<IList<UserRoleDto>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<long>> GetAddedRoleKeysAsync(long userId);
		Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request);

		Task<int> UpdateUserRoleAsync(UserRoleDto model);

		Task<int> DeleteUserRoleAsync(UserRoleDto model);
		Task<int> DeleteUserRoleRangeAsync(int index, int length, DataRequest<UserRole> request);
	}
}
