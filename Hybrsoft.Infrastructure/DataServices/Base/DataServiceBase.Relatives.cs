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
		public async Task<Relative> GetRelativeAsync(long id)
		{
			return await _learnDataSource.Relatives
				.Where(r => r.RelativeId == id)
				.Include(r => r.RelativeType)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<Relative>> GetRelativesAsync(int skip, int take, DataRequest<Relative> request)
		{
			IQueryable<Relative> items = GetRelatives(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Relative
				{
					RelativeId = r.RelativeId,
					FirstName = r.FirstName,
					LastName = r.LastName,
					Phone = r.Phone,
					Thumbnail = r.Thumbnail,
					LastModifiedOn = r.LastModifiedOn,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Relative>> GetRelativeKeysAsync(int skip, int take, DataRequest<Relative> request)
		{
			IQueryable<Relative> items = GetRelatives(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Relative
				{
					RelativeId = r.RelativeId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<Relative> GetRelatives(DataRequest<Relative> request, bool skipSorting = false)
		{
			IQueryable<Relative> items = _learnDataSource.Relatives;

			// Query
			if (!String.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => EF.Functions.Like(r.SearchTerms, "%" + request.Query + "%"));
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

		public async Task<int> GetRelativesCountAsync(DataRequest<Relative> request)
		{
			return await GetRelatives(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateRelativeAsync(Relative relative)
		{
			if (relative.RelativeId > 0)
			{
				_learnDataSource.Entry(relative).State = EntityState.Modified;
			}
			else
			{
				relative.RelativeId = UIDGenerator.Next();
				relative.CreatedOn = DateTimeOffset.Now;
				_learnDataSource.Entry(relative).State = EntityState.Added;
			}
			relative.LastModifiedOn = DateTimeOffset.Now;
			relative.SearchTerms = relative.BuildSearchTerms();
			int res = await _learnDataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> DeleteRelativesAsync(params Relative[] relatives)
		{
			return await _learnDataSource.Relatives
				.Where(r => relatives.Contains(r))
				.ExecuteDeleteAsync();
		}
	}
}
