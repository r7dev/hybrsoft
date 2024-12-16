using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ILogService
	{
		Task WriteAsync(LogType type, string source, string action, string message, string description);
		Task WriteAsync(LogType type, string source, string action, Exception ex);
		Task<AppLogDto> GetLogAsync(long id);
		Task<IList<AppLogDto>> GetLogsAsync(DataRequest<AppLog> request);
		Task<IList<AppLogDto>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request);
		Task<int> GetLogsCountAsync(DataRequest<AppLog> request);
		Task<int> CreateLogAsync(AppLog appLog);
		Task<int> DeleteLogAsync(AppLogDto model);
		Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<AppLog> request);
		Task MarkAllAsReadAsync();
	}
}
