using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class RolePermissionListArgs
	{
		static public RolePermissionListArgs CreateEmpty() => new() { IsEmpty = true };

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
