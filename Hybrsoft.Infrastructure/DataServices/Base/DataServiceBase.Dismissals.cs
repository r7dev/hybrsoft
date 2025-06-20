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
		public async Task<IList<ClassroomStudent>> GetDismissibleStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request)
		{
			IQueryable<ClassroomStudent> items = GetClassroomStudentsForDismissal(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(cs => new ClassroomStudent
				{
					ClassroomStudentId = cs.ClassroomStudentId,
					ClassroomId = cs.ClassroomId,
					StudentId = cs.StudentId,
					Classroom = new Classroom
					{
						ClassroomId = cs.Classroom.ClassroomId,
						Name = cs.Classroom.Name
					},
					Student = new Student
					{
						StudentId = cs.Student.StudentId,
						FirstName = cs.Student.FirstName,
						LastName = cs.Student.LastName,
						Thumbnail = cs.Student.Thumbnail,
					},
				})
				.AsNoTracking()
				.ToListAsync();
			return records;
		}

		public async Task<IList<ClassroomStudent>> GetDismissibleStudentKeysAsync(int skip, int take, DataRequest<ClassroomStudent> request)
		{
			IQueryable<ClassroomStudent> items = GetClassroomStudentsForDismissal(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new ClassroomStudent
				{
					ClassroomId = r.ClassroomId,
					StudentId = r.StudentId,
				})
				.AsNoTracking()
				.ToListAsync();
			return records;
		}

		private IQueryable<ClassroomStudent> GetClassroomStudentsForDismissal(DataRequest<ClassroomStudent> request, bool skipSorting = false)
		{
			IQueryable<ClassroomStudent> items = _learnDataSource.ClassroomStudents;

			// Query
			if (!String.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => EF.Functions.Like(r.SearchTermsDismissibleStudent, "%" + request.Query + "%"));
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

		public async Task<int> GetDismissibleStudentsCountAsync(DataRequest<ClassroomStudent> request)
		{
			return await GetClassroomStudentsForDismissal(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<Dismissal> GetDismissalAsync(long id)
		{
			return await _learnDataSource.Dismissals
				.Where(r => r.DismissalId == id)
				.Include(r => r.Classroom)
				.ThenInclude(r => r.ScheduleType)
				.Include(r => r.Student)
				.Include(r => r.Relative)
				.ThenInclude(r => r.RelativeType)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<Dismissal>> GetDismissalsAsync(int skip, int take, DataRequest<Dismissal> request)
		{
			IQueryable<Dismissal> items = GetDismissals(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Dismissal
				{
					DismissalId = r.DismissalId,
					ClassroomId = r.ClassroomId,
					Classroom = new Classroom
					{
						ClassroomId = r.Classroom.ClassroomId,
						Name = r.Classroom.Name,
						ScheduleType = new ScheduleType()
					},
					StudentId = r.StudentId,
					Student = new Student
					{
						StudentId = r.Student.StudentId,
						FirstName = r.Student.FirstName,
						LastName = r.Student.LastName,
						Thumbnail = r.Student.Thumbnail
					},
					RelativeId = r.RelativeId,
					Relative = new Relative
					{
						RelativeId = r.Relative.RelativeId,
						FirstName = r.Relative.FirstName,
						LastName = r.Relative.LastName,
						Thumbnail = r.Relative.Thumbnail,
						RelativeType = new RelativeType()
					},
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Dismissal>> GetDismissalKeysAsync(int skip, int take, DataRequest<Dismissal> request)
		{
			IQueryable<Dismissal> items = GetDismissals(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Dismissal
				{
					DismissalId = r.DismissalId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<Dismissal> GetDismissals(DataRequest<Dismissal> request, bool skipSorting = false)
		{
			IQueryable<Dismissal> items = _learnDataSource.Dismissals;

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

		public async Task<int> GetDismissalsCountAsync(DataRequest<Dismissal> request)
		{
			return await GetDismissals(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateDismissalAsync(Dismissal dismissal)
		{
			if (dismissal.DismissalId > 0)
			{
				_learnDataSource.Entry(dismissal).State = EntityState.Modified;
				dismissal.DismissedOn = DateTimeOffset.Now;
			}
			else
			{
				dismissal.DismissalId = UIDGenerator.Next();
				dismissal.CreatedOn = DateTimeOffset.Now;
				_learnDataSource.Entry(dismissal).State = EntityState.Added;
			}
			dismissal.SearchTerms = dismissal.BuildSearchTerms();
			int res = await _learnDataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> ApproveDismissalsAsync(params Dismissal[] dismissals)
		{
			return await _learnDataSource.Dismissals
				.Where(r => dismissals.Contains(r))
				.ExecuteUpdateAsync(r => r.SetProperty(x => x.DismissedOn, DateTimeOffset.Now));
		}
	}
}
