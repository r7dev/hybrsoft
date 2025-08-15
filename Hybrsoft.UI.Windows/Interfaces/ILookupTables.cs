using Hybrsoft.UI.Windows.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ILookupTables
	{
		Task InitializeAsync();

		IList<CountryModel> Countries { get; }
		IList<ScheduleTypeModel> ScheduleTypes { get; }
		IList<SubscriptionPlanModel> SubscriptionPlans { get; }
		IList<SubscriptionStatusModel> SubscriptionStatuses { get; }
		IList<SubscriptionTypeModel> SubscriptionTypes { get; }
		IList<RelativeTypeModel> RelativeTypes { get; }

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
