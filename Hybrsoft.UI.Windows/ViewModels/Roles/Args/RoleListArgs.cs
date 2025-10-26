using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class RoleListArgs
	{
		public static RoleListArgs CreateEmpty() => new() { IsEmpty = true };

		public RoleListArgs()
		{
			OrderBys = [(r => r.Name, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<Role, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
