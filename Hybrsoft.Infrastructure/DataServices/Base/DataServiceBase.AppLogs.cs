using Hybrsoft.Enums;
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
		public async Task<AppLog> GetAppLogAsync(long id)
		{
			return await _universalDataSource.AppLogs.Where(r => r.AppLogID == id).FirstOrDefaultAsync();
		}

		public async Task<IList<AppLog>> GetAppLogsAsync(int skip, int take, DataRequest<AppLog> request)
		{
			IQueryable<AppLog> items = GetLogs(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<AppLog>> GetAppLogKeysAsync(int skip, int take, DataRequest<AppLog> request)
		{
			IQueryable<AppLog> items = GetLogs(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new AppLog
				{
					AppLogID = r.AppLogID,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<AppLog> GetLogs(DataRequest<AppLog> request, bool skipSorting = false)
		{
			IQueryable<AppLog> items = _universalDataSource.AppLogs;

			// Query semantic
			if (request.UseSemanticSearch && !request.QueryEmbedding.IsNull)
			{
				items = items
					.Join(_universalDataSource.AppLogEmbeddings,
						  log => log.AppLogID,
						  emb => emb.AppLogID,
						  (log, emb) => new { log, emb })
					.Where(x => EF.Functions.VectorDistance("cosine", x.emb.Embedding, request.QueryEmbedding) < 0.7) // threshold
					.OrderBy(x => EF.Functions.VectorDistance("cosine", x.emb.Embedding, request.QueryEmbedding))
					.Select(x => x.log);
				skipSorting = true;
			}
			// Query
			else if (!string.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => EF.Functions.Like(r.SearchTerms, "%" + request.Query + "%"));
			}

			// Where
			if (request.Where != null)
			{
				items = items.Where(request.Where);
			}

			// Order By
			if (!skipSorting && request.OrderBys.Count != 0)
			{
				bool first = true;
				foreach (var (keySelector, orderBy) in request.OrderBys)
				{
					if (first)
					{
						items = orderBy == OrderBy.Desc
							? items.OrderByDescending(keySelector)
							: items.OrderBy(keySelector);
						first = false;
					}
					else
					{
						items = orderBy == OrderBy.Desc
							? ((IOrderedQueryable<AppLog>)items).ThenByDescending(keySelector)
							: ((IOrderedQueryable<AppLog>)items).ThenBy(keySelector);
					}
				}
			}

			return items;
		}

		public async Task<int> GetAppLogsCountAsync(DataRequest<AppLog> request)
		{
			return await GetLogs(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> CreateAppLogAsync(AppLog entity)
		{
			entity.CreateOn = DateTimeOffset.Now;
			_universalDataSource.Entry(entity).State = EntityState.Added;
			foreach (var embedding in entity.AppLogEmbeddings)
			{
				_universalDataSource.Entry(embedding).State = EntityState.Added;
			}
			return await _universalDataSource.SaveChangesAsync();
		}

		public async Task<int> UpdateAppLogAsync(IEnumerable<AppLog> entities)
		{
			foreach (var entity in entities)
			{
				if (entity.AppLogID > 0)
				{
					_universalDataSource.Entry(entity).State = EntityState.Modified;
				}
				else
				{
					_universalDataSource.Entry(entity).State = EntityState.Added;
				}
				foreach (var embedding in entity.AppLogEmbeddings)
				{
					if (embedding.AppLogEmbeddingID > 0)
					{
						_universalDataSource.Entry(embedding).State = EntityState.Modified;
					}
					else
					{
						_universalDataSource.Entry(embedding).State = EntityState.Added;
					}
				}
			}
			return await _universalDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteAppLogsAsync(params AppLog[] entities)
		{
			return await _universalDataSource.AppLogs
				.Where(r => entities.Contains(r))
				.ExecuteDeleteAsync();
		}

		public async Task MarkAllAsReadAsync()
		{
			await _universalDataSource.AppLogs
				.Where(r => !r.IsRead && r.AppType == AppType.EnterpriseManager)
				.ExecuteUpdateAsync(r => r.SetProperty(x => x.IsRead, true));
		}

		public async Task<IList<AppLog>> GetAppLogsWithMissingEmbeddingsAsync()
		{
			string query = @$"
				SELECT log.*
				FROM [Universal].[AppLog] log
					LEFT JOIN [Universal].[AppLogEmbedding] emb
						ON log.[AppLogID] = emb.[AppLogID]
				WHERE log.[AppType] = {(int)AppType.EnterpriseManager}
				AND (emb.[AppLogID] IS NULL OR emb.[Embedding] IS NULL)";

			return await _universalDataSource.AppLogs
				.FromSqlRaw(query)
				.Include(log => log.AppLogEmbeddings)
				.ToListAsync();
		}
	}
}
