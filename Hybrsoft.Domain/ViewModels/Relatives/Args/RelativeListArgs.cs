using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class RelativeListArgs
	{
		static public RelativeListArgs CreateEmpty() => new() { IsEmpty = true };

		public RelativeListArgs()
		{
			OrderBy = r => r.FirstName;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Relative, object>> OrderBy { get; set; }
		public Expression<Func<Relative, object>> OrderByDesc { get; set; }
	}
}
