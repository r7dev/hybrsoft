using System;

namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class DateRangeTools
	{
		const int DefaultPreviousDays = -1;

		public static DateTimeOffset GetStartDate(int previousDays = DefaultPreviousDays)
		{
			return GetStartDate(DateTime.Today, previousDays);
		}
		public static DateTimeOffset GetStartDate(DateTimeOffset startDate, int previousDays = DefaultPreviousDays)
		{
			if (previousDays > 0)
			{
				previousDays = -previousDays;
			}
			return startDate.AddDays(previousDays);
		}
		public static DateTimeOffset GetEndDate()
		{
			return GetEndDate(DateTime.Today);
		}
		public static DateTimeOffset GetEndDate(DateTimeOffset endDate)
		{
			return endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
		}
	}
}
