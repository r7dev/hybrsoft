using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IUserService
	{
		Task<UserModel> GetUserAsync(long id, bool includePassword = false);
		Task<UserModel> GetUserByEmailAsync(string email, bool includePassword = false);
		Task<IList<UserModel>> GetUsersAsync(DataRequest<User> request);
		Task<IList<UserModel>> GetUsersAsync(int skip, int take, DataRequest<User> request);
		Task<int> GetUsersCountAsync(DataRequest<User> request);

		Task<int> UpdateUserAsync(UserModel model);

		Task<int> DeleteUserAsync(UserModel model);
		Task<int> DeleteUserRangeAsync(int index, int length, DataRequest<User> request);
	}
}
