using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices
{
	public interface IDataService : IDisposable
	{
		IList<NavigationItem> GetNavigationItemByAppType(AppType appType);
		Task<AppLog> GetLogAsync(long id);
		Task<IList<AppLog>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request);
		Task<IList<AppLog>> GetLogKeysAsync(int skip, int take, DataRequest<AppLog> request);
		Task<int> GetLogsCountAsync(DataRequest<AppLog> request);
		Task<int> CreateLogAsync(AppLog appLog);
		Task<int> DeleteLogsAsync(params AppLog[] logs);
		Task MarkAllAsReadAsync();

		Task<User> GetUserAsync(long id);
		Task<IList<User>> GetUsersAsync(int skip, int take, DataRequest<User> request);
		Task<IList<User>> GetUserKeysAsync(int skip, int take, DataRequest<User> request);
		Task<int> GetUsersCountAsync(DataRequest<User> request);
		Task<int> UpdateUserAsync(User user);
		Task<int> DeleteUsersAsync(params User[] users);
	}
}
