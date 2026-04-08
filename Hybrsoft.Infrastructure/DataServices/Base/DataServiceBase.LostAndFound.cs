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
		public async Task<LostAndFound> GetLostAndFoundAsync(long id)
		{
			return await _learnDataSource.LostAndFound
				.Where(r => r.LostAndFoundID == id)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<LostAndFound>> GetLostAndFoundAsync(int skip, int take, DataRequest<LostAndFound> request)
		{
			IQueryable<LostAndFound> items = GetLostAndFound(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new LostAndFound
				{
					LostAndFoundID = r.LostAndFoundID,
					DisplayName = r.DisplayName,
					Thumbnail = r.Thumbnail,
					Status = r.Status,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<LostAndFound>> GetLostAndFoundKeysAsync(int skip, int take, DataRequest<LostAndFound> request)
		{
			IQueryable<LostAndFound> items = GetLostAndFound(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new LostAndFound
				{
					LostAndFoundID = r.LostAndFoundID,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<LostAndFound> GetLostAndFound(DataRequest<LostAndFound> request, bool skipSorting = false)
		{
			IQueryable<LostAndFound> items = _learnDataSource.LostAndFound;

			// Query
			if (!string.IsNullOrEmpty(request.Query))
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
							? ((IOrderedQueryable<LostAndFound>)items).ThenByDescending(keySelector)
							: ((IOrderedQueryable<LostAndFound>)items).ThenBy(keySelector);
					}
				}
			}

			return items;
		}

		public async Task<int> GetLostAndFoundCountAsync(DataRequest<LostAndFound> request)
		{
			return await GetLostAndFound(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateLostAndFoundAsync(LostAndFound entity)
		{
			if (entity.LostAndFoundID > 0)
			{
				_learnDataSource.Entry(entity).State = EntityState.Modified;
			}
			else
			{
				entity.LostAndFoundID = UIDGenerator.Next();
				entity.CreatedOn = DateTimeOffset.Now;
				_learnDataSource.Entry(entity).State = EntityState.Added;
			}
			entity.LastModifiedOn = DateTimeOffset.Now;
			entity.SearchTerms = entity.BuildSearchTerms();
			return await _learnDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteLostAndFoundAsync(params LostAndFound[] entities)
		{
			return await _learnDataSource.LostAndFound
				.Where(r => entities.Contains(r))
				.ExecuteDeleteAsync();
		}
	}
}
