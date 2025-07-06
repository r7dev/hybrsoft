using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class CompanyListArgs
	{
		public static CompanyListArgs CreateEmpty() => new() { IsEmpty = true };

		public CompanyListArgs()
		{
			OrderBy = r => r.LegalName;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Company, object>> OrderBy { get; set; }
		public Expression<Func<Company, object>> OrderByDesc { get; set; }
	}
}
