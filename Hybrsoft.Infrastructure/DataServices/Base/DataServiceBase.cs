using Hybrsoft.Infrastructure.DataContexts;
using System;
using System.Linq;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase(IUniversalDataSource universalDataSource, ILearnDataSource learnDataSource) : IDataService, IDisposable
	{
		private readonly IUniversalDataSource _universalDataSource = universalDataSource;
		private readonly ILearnDataSource _learnDataSource = learnDataSource;

		public bool HasPermission(long userId, string permissionName)
		{
			return _universalDataSource.UserRoles
				.Join(_universalDataSource.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => new { ur, rp })
				.Join(_universalDataSource.Permissions, urrp => urrp.rp.PermissionId, p => p.PermissionId, (urrp, p) => new { urrp.ur, urrp.rp, p })
				.Any(x => x.ur.UserId == userId && x.p.Name == permissionName);
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
			}
		}
		#endregion
	}
}
