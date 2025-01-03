using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	partial class DataServiceBase
	{
		public async Task<Permission> GetPermissionAsync(long id)
		{
			return await _dataSource.Permissions.Where(r => r.PermissionId == id).FirstOrDefaultAsync();
		}

		public async Task<IList<Permission>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request)
		{
			IQueryable<Permission> items = GetPermissions(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Permission
				{
					PermissionId = r.PermissionId,
					Name = r.Name,
					DisplayName = r.DisplayName,
					Description = r.Description,
					IsEnabled = r.IsEnabled,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Permission>> GetPermissionKeysAsync(int skip, int take, DataRequest<Permission> request)
		{
			IQueryable<Permission> items = GetPermissions(request);

			// Execute
			var records = await items.Skip(skip).Take(take)
				.Select(r => new Permission
				{
					PermissionId = r.PermissionId,
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		private IQueryable<Permission> GetPermissions(DataRequest<Permission> request, bool skipSorting = false)
		{
			IQueryable<Permission> items = _dataSource.Permissions;

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

		public async Task<int> GetPermissionsCountAsync(DataRequest<Permission> request)
		{
			return await GetPermissions(request, true)
				.AsNoTracking()
				.CountAsync();
		}

		public async Task<int> UpdatePermissionAsync(Permission permission)
		{
			if (permission.PermissionId > 0)
			{
				_dataSource.Entry(permission).State = EntityState.Modified;
			}
			else
			{
				permission.PermissionId = UIDGenerator.Next();
				permission.CreatedOn = DateTime.UtcNow;
				_dataSource.Entry(permission).State = EntityState.Added;
			}
			permission.LastModifiedOn = DateTime.UtcNow;
			permission.SearchTerms = permission.BuildSearchTerms();
			int res = await _dataSource.SaveChangesAsync();
			return res;
		}

		public async Task<int> DeletePermissionsAsync(params Permission[] permission)
		{
			_dataSource.Permissions.RemoveRange(permission);
			return await _dataSource.SaveChangesAsync();
		}
	}
}
