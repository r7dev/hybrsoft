using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class ClassroomListArgs
	{
		public static ClassroomListArgs CreateEmpty() => new() { IsEmpty = true };

		public ClassroomListArgs()
		{
			OrderBys =
			[
				(r => r.Year, OrderBy.Desc),
				(r => r.Name, OrderBy.Asc)
			];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<Classroom, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
