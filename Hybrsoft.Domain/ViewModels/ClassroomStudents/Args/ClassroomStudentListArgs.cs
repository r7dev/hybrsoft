using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class ClassroomStudentListArgs
	{
		public static ClassroomStudentListArgs CreateEmpty() => new() { IsEmpty = true };

		public ClassroomStudentListArgs()
		{
			OrderBy = r => r.Student.FirstName;
		}

		public long ClassroomID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<ClassroomStudent, object>> OrderBy { get; set; }
		public Expression<Func<ClassroomStudent, object>> OrderByDesc { get; set; }
	}
}
