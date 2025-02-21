using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	partial class DataServiceBase
	{
		public async Task<RolePermission> GetRolePermissionAsync(long rolePermissionId)
		{
			return await _universalDataSource.RolePermissions
				.Where(r => r.RolePermissionId == rolePermissionId)
				.Include(r => r.Permission)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<RolePermission>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request)
		{
			IQueryable<RolePermission> items = GetRolePermissions(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Include(r => r.Permission)
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<RolePermission>> GetRolePermissionKeysAsync(int skip, int take, DataRequest<RolePermission> request)
		{
			IQueryable<RolePermission> items = GetRolePermissions(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new RolePermission
				{
					RoleId = r.RoleId,
					PermissionId = r.PermissionId
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<RolePermission> GetRolePermissions(DataRequest<RolePermission> request, bool skipSorting = false)
		{
			IQueryable<RolePermission> items = _universalDataSource.RolePermissions;

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

		public async Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request)
		{
			return await GetRolePermissions(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdateRolePermissionAsync(RolePermission rolePermission)
		{
			if (rolePermission.RolePermissionId > 0)
			{
				_universalDataSource.Entry(rolePermission).State = EntityState.Modified;
			}
			else
			{
				rolePermission.RolePermissionId = UIDGenerator.Next();
				rolePermission.CreatedOn = DateTime.UtcNow;
				_universalDataSource.Entry(rolePermission).State = EntityState.Added;
			}
			rolePermission.LastModifiedOn = DateTime.UtcNow;
			rolePermission.SearchTerms = rolePermission.BuildSearchTerms();
			return await _universalDataSource.SaveChangesAsync();
		}

		public async Task<int> DeleteRolePermissionsAsync(params RolePermission[] rolePermission)
		{
			_universalDataSource.RolePermissions.RemoveRange(rolePermission);
			return await _universalDataSource.SaveChangesAsync();
		}
	}
}
