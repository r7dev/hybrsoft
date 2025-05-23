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
		public async Task<StudentRelative> GetStudentRelativeAsync(long studentRelativeId)
		{
			return await _learnDataSource.StudentRelatives
				.Where(r => r.StudentRelativeId == studentRelativeId)
				.Include(r => r.Relative)
				.ThenInclude(r => r.RelativeType)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<StudentRelative>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request)
		{
			IQueryable<StudentRelative> items = GetStudentRelatives(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Include(r => r.Relative)
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<StudentRelative>> GetStudentRelativeKeysAsync(int skip, int take, DataRequest<StudentRelative> request)
		{
			IQueryable<StudentRelative> items = GetStudentRelatives(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new StudentRelative
				{
					StudentId = r.StudentId,
					RelativeId = r.RelativeId
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<StudentRelative> GetStudentRelatives(DataRequest<StudentRelative> request, bool skipSorting = false)
		{
			IQueryable<StudentRelative> items = _learnDataSource.StudentRelatives;

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

		public async Task<IList<long>> GetAddedRelativeKeysAsync(long studentId)
		{
			return await _learnDataSource.StudentRelatives
				.AsNoTracking()
				.Where(r => r.StudentId == studentId)
				.Select(r => r.RelativeId)
				.ToListAsync();
		}

		public async Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request)
		{
			return await GetStudentRelatives(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateStudentRelativeAsync(StudentRelative studentRelative)
		{
			if (studentRelative.StudentRelativeId > 0)
			{
				_learnDataSource.Entry(studentRelative).State = EntityState.Modified;
			}
			else
			{
				studentRelative.StudentRelativeId = UIDGenerator.Next();
				studentRelative.CreatedOn = DateTimeOffset.Now;
				_learnDataSource.Entry(studentRelative).State = EntityState.Added;
			}
			studentRelative.LastModifiedOn = DateTimeOffset.Now;
			studentRelative.SearchTerms = studentRelative.BuildSearchTerms();
			return await _learnDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteStudentRelativesAsync(params StudentRelative[] studentRelatives)
		{
			return await _learnDataSource.StudentRelatives
				.Where(r => studentRelatives.Contains(r))
				.ExecuteDeleteAsync();
		}
	}
}
