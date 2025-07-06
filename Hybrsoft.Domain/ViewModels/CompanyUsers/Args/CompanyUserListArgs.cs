using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class CompanyUserListArgs
	{
		public static CompanyUserListArgs CreateEmpty() => new() { IsEmpty = true };

		public CompanyUserListArgs()
		{
			OrderBy = r => r.User.FirstName;
		}

		public long CompanyID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<CompanyUser, object>> OrderBy { get; set; }
		public Expression<Func<CompanyUser, object>> OrderByDesc { get; set; }
	}
}
