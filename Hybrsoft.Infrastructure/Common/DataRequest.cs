using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.Infrastructure.Common
{
	public class DataRequest<T>
	{
		public string Query { get; set; }

		public Expression<Func<T, bool>> Where { get; set; }
		public List<(Expression<Func<T, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; } = [];
	}
}
