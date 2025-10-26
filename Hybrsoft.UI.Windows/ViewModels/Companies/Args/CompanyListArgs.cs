using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class CompanyListArgs
	{
		public static CompanyListArgs CreateEmpty() => new() { IsEmpty = true };

		public CompanyListArgs()
		{
			OrderBys = [(r => r.LegalName, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<Company, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
