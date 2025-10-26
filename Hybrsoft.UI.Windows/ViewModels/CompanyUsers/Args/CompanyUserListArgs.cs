using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class CompanyUserListArgs
	{
		public static CompanyUserListArgs CreateEmpty() => new() { IsEmpty = true };

		public CompanyUserListArgs()
		{
			OrderBys = [(r => r.User.FirstName, OrderBy.Asc)];
		}

		public long CompanyID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<CompanyUser, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
