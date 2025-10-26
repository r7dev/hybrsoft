using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class UserRoleListArgs
	{
		public static UserRoleListArgs CreateEmpty() => new() { IsEmpty = true };

		public UserRoleListArgs()
		{
			OrderBys = [(r => r.Role.Name, OrderBy.Asc)];
		}

		public long UserId { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<UserRole, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
