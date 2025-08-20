using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.DataContexts;
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

		public async Task<IList<Permission>> GetPermissionsByUserAsync(long userID)
		{
			var records = await _universalDataSource.UserRoles
				.Join(_universalDataSource.RolePermissions, ur => ur.RoleID, rp => rp.RoleID, (ur, rp) => new { ur, rp })
				.Join(_universalDataSource.Permissions, urrp => urrp.rp.PermissionID, p => p.PermissionID, (urrp, p) => new { urrp.ur, urrp.rp, p })
				.Where(x => x.ur.UserID == userID)
				.Select(x => new Permission
				{
					PermissionID = x.p.PermissionID,
					Name = x.p.Name
				})
				.AsNoTracking()
				.ToListAsync();

			return records;
		}

		public async Task<IList<Country>> GetCountriesAsync()
		{
			return await _universalDataSource.Countries
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<SubscriptionPlan>> GetSubscriptionPlansAsync()
		{
			return await _universalDataSource.SubscriptionPlans
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<ScheduleType>> GetScheduleTypesAsync()
		{
			return await _learnDataSource.ScheduleTypes
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IList<RelativeType>> GetRelativeTypesAsync()
		{
			return await _learnDataSource.RelativeTypes
				.AsNoTracking()
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
