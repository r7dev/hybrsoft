using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class DismissalListArgs
	{
		public static DismissalListArgs CreateEmpty() => new() { IsEmpty = true };

		public DismissalListArgs()
		{
			OrderByDesc = r => r.CreatedOn;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Dismissal, object>> OrderBy { get; set; }
		public Expression<Func<Dismissal, object>> OrderByDesc { get; set; }
	}
}
