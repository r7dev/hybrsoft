using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class UserListArgs
	{
		public static UserListArgs CreateEmpty() => new() { IsEmpty = true };

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
