using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class RoleListArgs
	{
		static public RoleListArgs CreateEmpty() => new() { IsEmpty = true };

		public RoleListArgs()
		{
			OrderBy = r => r.Name;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Role, object>> OrderBy { get; set; }
		public Expression<Func<Role, object>> OrderByDesc { get; set; }
	}
}
