using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Enums;
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

		public IList<CountryDto> Countries { get; private set; }
		public IList<ScheduleTypeDto> ScheduleTypes { get; private set; }
		public IList<SubscriptionPlanDto> SubscriptionPlans { get; private set; }
		public IList<SubscriptionStatusDto> SubscriptionStatuses { get; private set; }
		public IList<SubscriptionTypeDto> SubscriptionTypes { get; private set; }
		public IList<RelativeTypeDto> RelativeTypes { get; private set; }

		public async Task InitializeAsync()
		{
			Countries = await GetCountriesAsync();
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

		private async Task<IList<CountryDto>> GetCountriesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetCountriesAsync();
				return [.. items.Select(r => new CountryDto
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

		public string GetScheduleType(Int16 scheduleTypeID)
		{
			return scheduleTypeID == 0
				? string.Empty
				: ScheduleTypes.Where(r => r.ScheduleTypeID == scheduleTypeID)
				.Select(r => r.Name)
				.FirstOrDefault();
		}

		private async Task<IList<ScheduleTypeDto>> GetScheduleTypesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetScheduleTypesAsync();
				return [.. items.Select(r => new ScheduleTypeDto
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

		private async Task<IList<SubscriptionPlanDto>> GetSubscriptionPlansAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetSubscriptionPlansAsync();
				return [.. items.Select(r => new SubscriptionPlanDto
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

		private async Task<IList<SubscriptionStatusDto>> GetSubscriptionStatusesAsync()
		{
			await Task.CompletedTask;
			return [.. Enum.GetValues<SubscriptionStatus>()
				.Select(r => new SubscriptionStatusDto
				{
					SubscriptionStatusID = (short)r,
					DisplayName = ResourceService.GetString(nameof(ResourceFiles.UI), $"SubscriptionStatus_{r}")
				})];
		}

		private static async Task<IList<SubscriptionTypeDto>> GetSubscriptionTypesAsync()
		{
			await Task.CompletedTask;
			return [.. Enum.GetValues<SubscriptionType>()
				.Select(r => new SubscriptionTypeDto
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

		private async Task<IList<RelativeTypeDto>> GetRelativeTypesAsync()
		{
			try
			{
				using var dataService = DataServiceFactory.CreateDataService();
				var items = await dataService.GetRelativeTypesAsync();
				return [.. items.Select(r => new RelativeTypeDto
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
