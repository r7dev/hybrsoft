using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ILogService
	{
		Task WriteAsync(LogType type, string source, string action, string message, string description);
		Task WriteAsync(LogType type, string source, string action, Exception ex);
		Task<AppLogModel> GetLogAsync(long id);
		Task<IList<AppLogModel>> GetLogsAsync(DataRequest<AppLog> request);
		Task<IList<AppLogModel>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request);
		Task<int> GetLogsCountAsync(DataRequest<AppLog> request);
		Task<int> CreateLogAsync(AppLog appLog);
		Task<int> DeleteLogAsync(AppLogModel model);
		Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<AppLog> request);
		Task MarkAllAsReadAsync();
	}
}
