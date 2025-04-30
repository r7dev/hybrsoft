using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
{
	public class ClassroomListArgs
	{
		static public ClassroomListArgs CreateEmpty() => new() { IsEmpty = true };

		public ClassroomListArgs()
		{
			OrderBy = r => r.Name;
			OrderByDesc = r => r.Year;
		}

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public Expression<Func<Classroom, object>> OrderBy { get; set; }
		public Expression<Func<Classroom, object>> OrderByDesc { get; set; }
	}
}
