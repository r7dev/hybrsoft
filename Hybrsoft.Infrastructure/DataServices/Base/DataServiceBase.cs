using Hybrsoft.Infrastructure.DataContexts;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase(IUniversalDataSource universalDataSource, ILearnDataSource learnDataSource) : IDataService, IDisposable
	{
		private readonly IUniversalDataSource _universalDataSource = universalDataSource;
		private readonly ILearnDataSource _learnDataSource = learnDataSource;

		public IList<NavigationItem> GetNavigationItemByAppType(AppType appType)
		{
			return [.. _universalDataSource.NavigationItems
				.Where(f => f.AppType == appType)
				.AsNoTracking()];
		}

		public bool HasPermission(long userId, string permissionName)
		{
			return _universalDataSource.UserRoles
				.Join(_universalDataSource.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => new { ur, rp })
				.Join(_universalDataSource.Permissions, urrp => urrp.rp.PermissionId, p => p.PermissionId, (urrp, p) => new { urrp.ur, urrp.rp, p })
				.Any(x => x.ur.UserId == userId && x.p.Name == permissionName);
		}

		public async Task<IList<ScheduleType>> GetScheduleTypesAsync()
		{
			return await _learnDataSource.ScheduleTypes
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<RelativeType>> GetRelativeTypesByLanguageAsync(string languageTag)
		{
			return await _learnDataSource.RelativeTypes
				.AsNoTracking()
				.Where(f => f.LanguageTag == languageTag)
				.ToListAsync();
		}

		#region Dispose
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_universalDataSource?.Dispose();
				_learnDataSource?.Dispose();
			}
		}
		#endregion
	}
}
