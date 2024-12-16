using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class AppLogListArgs
	{
		static public AppLogListArgs CreateEmpty() => new() { IsEmpty = true };
		public AppLogListArgs()
		{
			OrderByDesc = r => r.CreateOn;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<AppLog, object>> OrderBy { get; set; }
		public Expression<Func<AppLog, object>> OrderByDesc { get; set; }
	}
}
