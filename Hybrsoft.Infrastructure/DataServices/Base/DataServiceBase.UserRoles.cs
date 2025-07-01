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
		public async Task<UserRole> GetUserRoleAsync(long id)
		{
			return await _universalDataSource.UserRoles
				.Where(r => r.UserRoleID == id)
				.Include(r => r.Role)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<UserRole>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request)
		{
			IQueryable<UserRole> items = GetUserRoles(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new UserRole
				{
					UserRoleID = r.UserRoleID,
					UserID = r.UserID,
					RoleID = r.RoleID,
					Role = new Role
					{
						RoleID = r.Role.RoleID,
						Name = r.Role.Name
					}
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<UserRole>> GetUserRoleKeysAsync(int skip, int take, DataRequest<UserRole> request)
		{
			IQueryable<UserRole> items = GetUserRoles(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new UserRole
				{
					UserID = r.UserID,
					RoleID = r.RoleID
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<UserRole> GetUserRoles(DataRequest<UserRole> request, bool skipSorting = false)
		{
			IQueryable<UserRole> items = _universalDataSource.UserRoles;

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

		public async Task<IList<long>> GetAddedRoleKeysAsync(long userID)
		{
			return await _universalDataSource.UserRoles
				.AsNoTracking()
				.Where(r => r.UserID == userID)
				.Select(r => r.RoleID)
				.ToListAsync();
		}

		public async Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request)
		{
			return await GetUserRoles(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateUserRoleAsync(UserRole userRole)
		{
			if (userRole.UserRoleID > 0)
			{
				_universalDataSource.Entry(userRole).State = EntityState.Modified;
			}
			else
			{
				userRole.UserRoleID = UIDGenerator.Next();
				userRole.CreatedOn = DateTimeOffset.Now;
				_universalDataSource.Entry(userRole).State = EntityState.Added;
			}
			userRole.LastModifiedOn = DateTimeOffset.Now;
			userRole.SearchTerms = userRole.BuildSearchTerms();
			return await _universalDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteUserRolesAsync(params UserRole[] userRoles)
		{
			return await _universalDataSource.UserRoles
				.Where(r => userRoles.Contains(r))
				.ExecuteDeleteAsync();
		}
	}
}
