using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class ClassroomStudentListArgs
	{
		public static ClassroomStudentListArgs CreateEmpty() => new() { IsEmpty = true };

		public ClassroomStudentListArgs()
		{
			OrderBys = [(r => r.Student.FirstName, OrderBy.Asc)];
		}

		public long ClassroomID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<ClassroomStudent, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
