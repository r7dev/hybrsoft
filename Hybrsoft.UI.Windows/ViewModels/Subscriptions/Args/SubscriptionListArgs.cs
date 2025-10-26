using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public class SubscriptionListArgs
	{
		public static SubscriptionListArgs CreateEmpty() => new()
		{
			IsEmpty = true,
			StartDate = DateRangeTools.GetStartDate(),
			EndDate = DateRangeTools.GetEndDate(),
		};

		public SubscriptionListArgs()
		{
			StartDate = DateRangeTools.GetStartDate();
			EndDate = DateRangeTools.GetEndDate();
			OrderBys = [(r => r.CreatedOn, OrderBy.Desc)];
		}

		public long SubscriptionID { get; set; }

		public bool IsEmpty { get; set; }

		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string Query { get; set; }

		public List<(Expression<Func<Subscription, object>> KeySelector, OrderBy OrderBy)> OrderBys { get; set; }
	}
}
