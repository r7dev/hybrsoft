using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class PermissionListArgs
	{
		static public PermissionListArgs CreateEmpty() => new() { IsEmpty = true };

		public PermissionListArgs()
		{
			OrderBy = r => r.Name;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Permission, object>> OrderBy { get; set; }
		public Expression<Func<Permission, object>> OrderByDesc { get; set; }
	}
}
