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
		public async Task<StudentBelonging> GetStudentBelongingAsync(long id)
		{
			return await _learnDataSource.StudentBelongings
				.Where(r => r.StudentBelongingID == id)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<StudentBelonging>> GetStudentBelongingsAsync(int skip, int take, DataRequest<StudentBelonging> request)
		{
			IQueryable<StudentBelonging> items = GetStudentBelongings(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new StudentBelonging
				{
					StudentBelongingID = r.StudentBelongingID,
					StudentID = r.StudentID,
					DisplayName = r.DisplayName,
					Thumbnail = r.Thumbnail
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<StudentBelonging>> GetStudentBelongingKeysAsync(int skip, int take, DataRequest<StudentBelonging> request)
		{
			IQueryable<StudentBelonging> items = GetStudentBelongings(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new StudentBelonging
				{
					StudentBelongingID = r.StudentBelongingID
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<StudentBelonging> GetStudentBelongings(DataRequest<StudentBelonging> request, bool skipSorting = false)
		{
			IQueryable<StudentBelonging> items = _learnDataSource.StudentBelongings;

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
							? ((IOrderedQueryable<StudentBelonging>)items).ThenByDescending(keySelector)
							: ((IOrderedQueryable<StudentBelonging>)items).ThenBy(keySelector);
					}
				}
			}

			return items;
		}

		public async Task<int> GetStudentBelongingsCountAsync(DataRequest<StudentBelonging> request)
		{
			return await GetStudentBelongings(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateStudentBelongingAsync(StudentBelonging entity)
		{
			if (entity.StudentBelongingID > 0)
			{
				_learnDataSource.Entry(entity).State = EntityState.Modified;
			}
			else
			{
				entity.StudentBelongingID = UIDGenerator.Next();
				entity.CreatedOn = DateTimeOffset.Now;
				_learnDataSource.Entry(entity).State = EntityState.Added;
			}
			entity.LastModifiedOn = DateTimeOffset.Now;
			entity.SearchTerms = entity.BuildSearchTerms();
			return await _learnDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteStudentBelongingsAsync(params StudentBelonging[] entities)
		{
			return await _learnDataSource.StudentBelongings
				.Where(r => entities.Contains(r))
				.ExecuteDeleteAsync();
		}
	}
}
