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
		public async Task<Student> GetStudentAsync(long id)
		{
			return await _learnDataSource.Students.Where(r => r.StudentId == id).FirstOrDefaultAsync();
		}

		public async Task<IList<Student>> GetStudentsAsync(int skip, int take, DataRequest<Student> request)
		{
			IQueryable<Student> items = GetStudents(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Student
				{
					StudentId = r.StudentId,
					FirstName = r.FirstName,
					LastName = r.LastName,
					Email = r.Email,
					Thumbnail = r.Thumbnail,
					LastModifiedOn = r.LastModifiedOn,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Student>> GetStudentKeysAsync(int skip, int take, DataRequest<Student> request)
		{
			IQueryable<Student> items = GetStudents(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Student
				{
					StudentId = r.StudentId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<Student> GetStudents(DataRequest<Student> request, bool skipSorting = false)
		{
			IQueryable<Student> items = _learnDataSource.Students;

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

		public async Task<int> GetStudentsCountAsync(DataRequest<Student> request)
		{
			return await GetStudents(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateStudentAsync(Student Student)
		{
			if (Student.StudentId > 0)
			{
				_learnDataSource.Entry(Student).State = EntityState.Modified;
			}
			else
			{
				Student.StudentId = UIDGenerator.Next();
				Student.CreatedOn = DateTime.UtcNow;
				_learnDataSource.Entry(Student).State = EntityState.Added;
			}
			Student.LastModifiedOn = DateTime.UtcNow;
			Student.SearchTerms = Student.BuildSearchTerms();
			int res = await _learnDataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> DeleteStudentsAsync(params Student[] Students)
		{
			_learnDataSource.Students.RemoveRange(Students);
			return await _learnDataSource.SaveChangesAsync();
		}
	}
}
