using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class StudentRelativeListArgs
	{
		public static StudentRelativeListArgs CreateEmpty() => new() { IsEmpty = true };

		public StudentRelativeListArgs()
		{
			OrderBys = [(r => r.Relative.FirstName, OrderBy.Asc)];
		}

		public long StudentID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<StudentRelative, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
