using Hybrsoft.UI.Windows.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ILookupTables
	{
		Task InitializeAsync();

		IList<CountryDto> Countries { get; }
		IList<ScheduleTypeDto> ScheduleTypes { get; }
		IList<SubscriptionPlanDto> SubscriptionPlans { get; }
		IList<SubscriptionStatusDto> SubscriptionStatuses { get; }
		IList<SubscriptionTypeDto> SubscriptionTypes { get; }
		IList<RelativeTypeDto> RelativeTypes { get; }

		string GetCountry(Int16 countryID);
		string GetScheduleType(Int16 scheduleTypeID);
		string GetSubscriptionPlan(short subscriptionPlanID);
		string GetSubscriptionStatus(short subscriptionPlanID);
		string GetRelativeType(Int16 relativeTypeID);
	}

	public class LookupTablesProxy
	{
		static public ILookupTables Instance { get; set; }
	}
}
