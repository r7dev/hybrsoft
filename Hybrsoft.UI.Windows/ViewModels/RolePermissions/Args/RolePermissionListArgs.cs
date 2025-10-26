using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class RolePermissionListArgs
	{
		public static RolePermissionListArgs CreateEmpty() => new() { IsEmpty = true };

		public RolePermissionListArgs()
		{
			OrderBys = [(r => r.Permission.DisplayName, OrderBy.Asc)];
		}

		public long RoleId { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<RolePermission, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
