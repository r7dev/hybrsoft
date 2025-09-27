using Hybrsoft.UI.Windows.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ILookupTables
	{
		Task InitializeAsync();
		Task LoadAfterLoginAsync();

		IList<CountryModel> Countries { get; }
		IList<PermissionModel> Permissions { get; }
		IList<ScheduleTypeModel> ScheduleTypes { get; }
		IList<SubscriptionPlanModel> SubscriptionPlans { get; }
		IList<SubscriptionStatusModel> SubscriptionStatuses { get; }
		IList<SubscriptionTypeModel> SubscriptionTypes { get; }
		IList<RelativeTypeModel> RelativeTypes { get; }

		string GetCountry(short countryID);
		string GetScheduleType(short scheduleTypeID);
		string GetSubscriptionPlan(short subscriptionPlanID);
		string GetSubscriptionStatus(short subscriptionPlanID);
		string GetRelativeType(short relativeTypeID);
	}

	public class LookupTablesProxy
	{
		static public ILookupTables Instance { get; set; }
	}
}
