using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class UserRoleListArgs
	{
		static public UserRoleListArgs CreateEmpty() => new() { IsEmpty = true };

		public UserRoleListArgs()
		{
			OrderBy = r => r.Role.Name;
		}

		public long UserId { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<UserRole, object>> OrderBy { get; set; }
		public Expression<Func<UserRole, object>> OrderByDesc { get; set; }
	}
}
