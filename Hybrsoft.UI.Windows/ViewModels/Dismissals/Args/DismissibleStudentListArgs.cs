using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class DismissibleStudentListArgs
	{
		public static DismissibleStudentListArgs CreateEmpty() => new() { IsEmpty = true };

		public DismissibleStudentListArgs()
		{
			OrderBys = [(r => r.Student.FirstName, OrderBy.Asc)];
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<ClassroomStudent, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
