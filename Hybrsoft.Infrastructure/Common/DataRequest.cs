using Hybrsoft.Enums;
using Microsoft.Data.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.Infrastructure.Common
{
	public class DataRequest<T>
	{
		public bool UseSemanticSearch { get; set; } = false;
		public string Query { get; set; }
		public SqlVector<float> QueryEmbedding { get; set; }

		public Expression<Func<T, bool>> Where { get; set; }
		public List<(Expression<Func<T, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; } = [];
		public List<Expression<Func<T, object>>> Includes { get; set; } = [];
	}
}
