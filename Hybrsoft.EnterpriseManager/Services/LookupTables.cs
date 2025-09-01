using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class LookupTables(IDataServiceFactory dataServiceFactory, ILogService logService, IResourceService resourceService) : ILookupTables
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;
		public IResourceService ResourceService { get; } = resourceService;

		public IList<CountryModel> Countries { get; private set; }
		public IList<PermissionModel> Permissions { get; private set; }
		public IList<ScheduleTypeModel> ScheduleTypes { get; private set; }
		public IList<SubscriptionPlanModel> SubscriptionPlans { get; private set; }
		public IList<SubscriptionStatusModel> SubscriptionStatuses { get; private set; }
		public IList<SubscriptionTypeModel> SubscriptionTypes { get; private set; }
		public IList<RelativeTypeModel> RelativeTypes { get; private set; }

		public async Task InitializeAsync()
		{
			Countries = await GetCountriesAsync();
			Permissions = await GetPermissionsByUserAsync(AppSettings.Current.UserID);
			ScheduleTypes = await GetScheduleTypesAsync();
			SubscriptionPlans = await GetSubscriptionPlansAsync();
			SubscriptionStatuses = await GetSubscriptionStatusesAsync();
			SubscriptionTypes = await GetSubscriptionTypesAsync();
			RelativeTypes = await GetRelativeTypesAsync();
		}

		public string GetCountry(Int16 countryID)
		{
			return countryID == 0
				? string.Empty
				: Countries.Where(r => r.CountryID == countryID)
				.Select(r => r.Name)
				.FirstOrDefault();
		}

		private async Task<IList<CountryModel>> GetCountriesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetCountriesAsync();
				return [.. items.Select(r => new CountryModel
				{
					CountryID = r.CountryID,
					Name = string.IsNullOrEmpty(r.Uid) ? r.Name : ResourceService.GetString(nameof(ResourceFiles.UI), r.Uid),
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load Countries", ex);
			}
			return [];
		}

		private async Task<IList<PermissionModel>> GetPermissionsByUserAsync(long userID)
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetPermissionsByUserAsync(userID);
				return [.. items.Select(r => new PermissionModel
				{
					PermissionID = r.PermissionID,
					Name = r.Name,
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load Permissions", ex);
			}
			return [];
		}

		public string GetScheduleType(Int16 scheduleTypeID)
		{
			return scheduleTypeID == 0
				? string.Empty
				: ScheduleTypes.Where(r => r.ScheduleTypeID == scheduleTypeID)
				.Select(r => r.Name)
				.FirstOrDefault();
		}

		private async Task<IList<ScheduleTypeModel>> GetScheduleTypesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetScheduleTypesAsync();
				return [.. items.Select(r => new ScheduleTypeModel
				{
					ScheduleTypeID = r.ScheduleTypeID,
					Name = string.IsNullOrEmpty(r.Uid) ? r.Name : ResourceService.GetString(nameof(ResourceFiles.UI), r.Uid),
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load ScheduleTypes", ex);
			}
			return [];
		}

		public string GetSubscriptionPlan(short subscriptionPlanID)
		{
			return subscriptionPlanID == 0
				? string.Empty
				: SubscriptionPlans.Where(r => r.SubscriptionPlanID == subscriptionPlanID)
				.Select(r => r.Name)
				.FirstOrDefault();
		}

		private async Task<IList<SubscriptionPlanModel>> GetSubscriptionPlansAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetSubscriptionPlansAsync();
				return [.. items.Select(r => new SubscriptionPlanModel
				{
					SubscriptionPlanID = r.SubscriptionPlanID,
					Name = string.IsNullOrEmpty(r.Uid) ? r.Name : ResourceService.GetString(nameof(ResourceFiles.UI), r.Uid),
					DurationMonths = r.DurationMonths
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load SubscriptionPlans", ex);
			}
			return [];
		}

		public string GetSubscriptionStatus(short subscriptionStatusID)
		{
			return SubscriptionStatuses.Where(r => r.SubscriptionStatusID == subscriptionStatusID)
				.Select(r => r.DisplayName)
				.FirstOrDefault();
		}

		private async Task<IList<SubscriptionStatusModel>> GetSubscriptionStatusesAsync()
		{
			await Task.CompletedTask;
			return [.. Enum.GetValues<SubscriptionStatus>()
				.Select(r => new SubscriptionStatusModel
				{
					SubscriptionStatusID = (short)r,
					DisplayName = ResourceService.GetString(nameof(ResourceFiles.UI), $"SubscriptionStatus_{r}")
				})];
		}

		private static async Task<IList<SubscriptionTypeModel>> GetSubscriptionTypesAsync()
		{
			await Task.CompletedTask;
			return [.. Enum.GetValues<SubscriptionType>()
				.Select(r => new SubscriptionTypeModel
				{
					SubscriptionTypeID = (short)r,
					DisplayName = r.ToString()
				})];
		}

		public string GetRelativeType(Int16 relativeTypeID)
		{
			return relativeTypeID == 0
				? string.Empty
				: RelativeTypes.Where(r => r.RelativeTypeID == relativeTypeID)
				.Select(r => r.Name)
				.FirstOrDefault();
		}

		private async Task<IList<RelativeTypeModel>> GetRelativeTypesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetRelativeTypesAsync();
				return [.. items.Select(r => new RelativeTypeModel
				{
					RelativeTypeID = r.RelativeTypeID,
					Name = string.IsNullOrEmpty(r.Uid) ? r.Name : ResourceService.GetString(nameof(ResourceFiles.UI), r.Uid),
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load RelativeTypes", ex);
			}
			return [];
		}

		private async void LogException(string source, string action, Exception exception)
		{
			await LogService.WriteAsync(LogType.Error, source, action, exception.Message, exception.ToString());
		}
	}
}
