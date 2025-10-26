using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class RelativeListArgs
	{
		public static RelativeListArgs CreateEmpty() => new() { IsEmpty = true };

		public RelativeListArgs()
		{
			OrderBys = [(r => r.FirstName, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<Relative, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
