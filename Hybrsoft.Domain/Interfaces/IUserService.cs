using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IUserService
	{
		Task<UserDto> GetUserAsync(long id, bool includePassword = false);
		Task<UserDto> GetUserByEmailAsync(string email, bool includePassword = false);
		Task<IList<UserDto>> GetUsersAsync(DataRequest<User> request);
		Task<IList<UserDto>> GetUsersAsync(int skip, int take, DataRequest<User> request);
		Task<int> GetUsersCountAsync(DataRequest<User> request);

		Task<int> UpdateUserAsync(UserDto model);

		Task<int> DeleteUserAsync(UserDto model);
		Task<int> DeleteUserRangeAsync(int index, int length, DataRequest<User> request);
	}
}
