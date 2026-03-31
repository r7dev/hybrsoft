using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class StudentBelongingListArgs
	{
		public static StudentBelongingListArgs CreateEmpty() => new() { IsEmpty = true };

		public StudentBelongingListArgs()
		{
			OrderBys = [(r => r.DisplayName, OrderBy.Asc)];
		}

		public long StudentID { get; set; }

		public bool IsEmpty { get; set; }

		public string Query { get; set; }

		public List<(Expression<Func<StudentBelonging, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
