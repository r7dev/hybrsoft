using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class RolePermissionListArgs
	{
		public static RolePermissionListArgs CreateEmpty() => new() { IsEmpty = true };

		public RolePermissionListArgs()
		{
			OrderBy = r => r.Permission.DisplayName;
		}

		public long RoleId { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<RolePermission, object>> OrderBy { get; set; }
		public Expression<Func<RolePermission, object>> OrderByDesc { get; set; }
	}
}
