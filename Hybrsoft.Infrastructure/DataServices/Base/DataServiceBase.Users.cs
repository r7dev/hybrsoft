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
		public async Task<User> GetUserAsync(Guid id)
		{
			return await _dataSource.Users.Where(r => r.UserId == id).FirstOrDefaultAsync();
		}

		public async Task<IList<User>> GetUsersAsync(int skip, int take, DataRequest<User> request)
		{
			IQueryable<User> items = GetUsers(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new User
				{
					UserId = r.UserId,
					FirstName = r.FirstName,
					LastName = r.LastName
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<User> GetUsers(DataRequest<User> request)
		{
			IQueryable<User> items = _dataSource.Users;

			// Query
			if (!String.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
			}

			// Where
			if (request.Where != null)
			{
				items = items.Where(request.Where);
			}

			// Order By
			if (request.OrderBy != null)
			{
				items = items.OrderBy(request.OrderBy);
			}
			if (request.OrderByDesc != null)
			{
				items = items.OrderByDescending(request.OrderByDesc);
			}

			return items;
		}

		public async Task<int> GetUsersCountAsync(DataRequest<User> request)
		{
			IQueryable<User> items = _dataSource.Users;

			// Query
			if (!String.IsNullOrEmpty(request.Query))
			{
				items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
			}

			// Where
			if (request.Where != null)
			{
				items = items.Where(request.Where);
			}

			return await items.CountAsync();
		}

		public async Task<int> UpdateUserAsync(User user)
		{
			if (user.UserId != Guid.Empty)
			{
				_dataSource.Entry(user).State = EntityState.Modified;
			}
			else
			{
				user.UserId = Guid.NewGuid();
				user.CreatedOn = DateTime.UtcNow;
				_dataSource.Entry(user).State = EntityState.Added;
			}
			user.LastModifiedOn = DateTime.UtcNow;
			user.SearchTerms = user.BuildSearchTerms();
			int res = await _dataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> DeleteUsersAsync(params User[] users)
		{
			_dataSource.Users.RemoveRange(users);
			return await _dataSource.SaveChangesAsync();
		}
	}
}
