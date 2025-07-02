using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class StudentListArgs
	{
		public static StudentListArgs CreateEmpty() => new() { IsEmpty = true };

		public StudentListArgs()
		{
			OrderBy = r => r.FirstName;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Student, object>> OrderBy { get; set; }
		public Expression<Func<Student, object>> OrderByDesc { get; set; }
	}
}
