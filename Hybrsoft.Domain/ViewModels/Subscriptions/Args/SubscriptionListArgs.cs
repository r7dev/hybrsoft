using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Linq.Expressions;

namespace Hybrsoft.Domain.ViewModels
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
			OrderByDesc = s => s.CreatedOn;
		}

		public long SubscriptionID { get; set; }

		public bool IsEmpty { get; set; }

		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string Query { get; set; }

		public Expression<Func<Subscription, object>> OrderBy { get; set; }
		public Expression<Func<Subscription, object>> OrderByDesc { get; set; }	
	}
}
