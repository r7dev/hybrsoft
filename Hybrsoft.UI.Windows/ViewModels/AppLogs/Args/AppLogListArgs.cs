using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class AppLogListArgs
	{
		public static AppLogListArgs CreateEmpty() => new()
		{
			IsEmpty = true,
			StartDate = DateRangeTools.GetStartDate(),
			EndDate = DateRangeTools.GetEndDate(),
		};
		public AppLogListArgs()
		{
			StartDate = DateRangeTools.GetStartDate();
			EndDate = DateRangeTools.GetEndDate();
			OrderBys = [(r => r.CreateOn, OrderBy.Desc)];
		}

		public bool IsEmpty { get; set; }

		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string Query { get; set; }

		public List<(Expression<Func<AppLog, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
