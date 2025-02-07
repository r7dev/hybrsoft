using Hybrsoft.Infrastructure.DataContexts;
using System;
using System.Linq;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase(IDataSource dataSource) : IDataService, IDisposable
	{
		private readonly IDataSource _dataSource = dataSource;

		public bool HasPermission(long userId, string permissionName)
		{
			return _dataSource.UserRoles
				.Join(_dataSource.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => new { ur, rp })
				.Join(_dataSource.Permissions, urrp => urrp.rp.PermissionId, p => p.PermissionId, (urrp, p) => new { urrp.ur, urrp.rp, p })
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
				_dataSource?.Dispose();
			}
		}
		#endregion
	}
}
