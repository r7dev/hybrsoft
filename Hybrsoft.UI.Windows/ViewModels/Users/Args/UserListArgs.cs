using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class UserListArgs
	{
		public static UserListArgs CreateEmpty() => new() { IsEmpty = true };

		public UserListArgs()
		{
			OrderBys = [(r => r.FirstName, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<User, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
