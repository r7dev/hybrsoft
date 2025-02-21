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
		public async Task<Role> GetRoleAsync(long id)
		{
			return await _universalDataSource.Roles.Where(r => r.RoleId == id).FirstOrDefaultAsync();
		}

		public async Task<IList<Role>> GetRolesAsync(int skip, int take, DataRequest<Role> request)
		{
			IQueryable<Role> items = GetRoles(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Role
				{
					RoleId = r.RoleId,
					Name = r.Name,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Role>> GetRoleKeysAsync(int skip, int take, DataRequest<Role> request)
		{
			IQueryable<Role> items = GetRoles(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Role
				{
					RoleId = r.RoleId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<Role> GetRoles(DataRequest<Role> request, bool skipSorting = false)
		{
			IQueryable<Role> items = _universalDataSource.Roles;

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

		public async Task<int> GetRolesCountAsync(DataRequest<Role> request)
		{
			return await GetRoles(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateRoleAsync(Role role)
		{
			if (role.RoleId > 0)
			{
				_universalDataSource.Entry(role).State = EntityState.Modified;
			}
			else
			{
				role.RoleId = UIDGenerator.Next();
				role.CreatedOn = DateTime.UtcNow;
				_universalDataSource.Entry(role).State = EntityState.Added;
			}
			role.LastModifiedOn = DateTime.UtcNow;
			role.SearchTerms = role.BuildSearchTerms();
			int res = await _universalDataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> DeleteRolesAsync(params Role[] role)
		{
			_universalDataSource.Roles.RemoveRange(role);
			return await _universalDataSource.SaveChangesAsync();
		}
	}
}
