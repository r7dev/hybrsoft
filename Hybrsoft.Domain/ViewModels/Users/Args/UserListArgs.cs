using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class UserListArgs
	{
		static public UserListArgs CreateEmpty() => new() { IsEmpty = true };

		public UserListArgs()
		{
			OrderBy = r => r.FirstName;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<User, object>> OrderBy { get; set; }
		public Expression<Func<User, object>> OrderByDesc { get; set; }
	}
}
