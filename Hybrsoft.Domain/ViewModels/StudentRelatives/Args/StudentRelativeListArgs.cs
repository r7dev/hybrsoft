using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class StudentRelativeListArgs
	{
		static public StudentRelativeListArgs CreateEmpty() => new() { IsEmpty = true };

		public StudentRelativeListArgs()
		{
			OrderBy = r => r.Relative.FirstName;
		}

		public long StudentID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<StudentRelative, object>> OrderBy { get; set; }
		public Expression<Func<StudentRelative, object>> OrderByDesc { get; set; }
	}
}
