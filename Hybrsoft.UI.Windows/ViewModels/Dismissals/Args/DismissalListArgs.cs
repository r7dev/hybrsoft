using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class DismissalListArgs
	{
		public static DismissalListArgs CreateEmpty() => new() { IsEmpty = true };

		public DismissalListArgs()
		{
			OrderBys = [(r => r.CreatedOn, OrderBy.Desc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<Dismissal, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
