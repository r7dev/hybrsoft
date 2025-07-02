using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
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
			OrderByDesc = r => r.CreateOn;
		}

		public bool IsEmpty { get; set; }

		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string Query { get; set; }

		public Expression<Func<AppLog, object>> OrderBy { get; set; }
		public Expression<Func<AppLog, object>> OrderByDesc { get; set; }
	}
}
