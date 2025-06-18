using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class DismissibleStudentListArgs
	{
		static public DismissibleStudentListArgs CreateEmpty() => new() { IsEmpty = true };

		public DismissibleStudentListArgs()
		{
			OrderBy = r => r.Student.FirstName;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<ClassroomStudent, object>> OrderBy { get; set; }
		public Expression<Func<ClassroomStudent, object>> OrderByDesc { get; set; }
	}
}
