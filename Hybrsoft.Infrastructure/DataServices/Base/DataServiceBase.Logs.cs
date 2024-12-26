using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	partial class DataServiceBase
	{
		public async Task<AppLog> GetLogAsync(long id)
		{
			return await _dataSource.AppLogs.Where(r => r.AppLogId == id).FirstOrDefaultAsync();
		}

		public async Task<IList<AppLog>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request)
		{
			IQueryable<AppLog> items = GetLogs(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<AppLog>> GetLogKeysAsync(int skip, int take, DataRequest<AppLog> request)
		{
			IQueryable<AppLog> items = GetLogs(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new AppLog
				{
					AppLogId = r.AppLogId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<AppLog> GetLogs(DataRequest<AppLog> request, bool skipSorting = false)
		{
			IQueryable<AppLog> items = _dataSource.AppLogs;

			// Query
			if (!String.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => EF.Functions.Like(r.Message, "%" + request.Query + "%"));
			}

			// Where
			if (request.Where != null)
			{
				items = items.Where(request.Where);
			}

			// Order By
			if (!skipSorting && request.OrderBy != null)
			{
				items = items.OrderBy(request.OrderBy);
			}
			if (!skipSorting && request.OrderByDesc != null)
			{
				items = items.OrderByDescending(request.OrderByDesc);
			}

			return items;
		}

		public async Task<int> GetLogsCountAsync(DataRequest<AppLog> request)
		{
			return await GetLogs(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			appLog.CreateOn = DateTime.UtcNow;
			_dataSource.Entry(appLog).State = EntityState.Added;
			return await _dataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteLogsAsync(params AppLog[] logs)
		{
			_dataSource.AppLogs.RemoveRange(logs);
			return await _dataSource.SaveChangesAsync();
		}

		public async Task MarkAllAsReadAsync()
		{
			var items = await _dataSource.AppLogs
				.Where(r => !r.IsRead && r.AppType == Enums.AppType.EnterpriseManager)
				.ToListAsync();
			foreach (var item in items)
			{
				item.IsRead = true;
			}
			await _dataSource.SaveChangesAsync();
		}
	}
}
