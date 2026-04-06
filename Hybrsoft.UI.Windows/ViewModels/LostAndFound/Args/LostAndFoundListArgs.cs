using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class LostAndFoundListArgs
	{
		public static LostAndFoundListArgs CreateEmpty() => new() { IsEmpty = true };

		public LostAndFoundListArgs()
		{
			OrderBys = [(r => r.DisplayName, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<LostAndFound, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
