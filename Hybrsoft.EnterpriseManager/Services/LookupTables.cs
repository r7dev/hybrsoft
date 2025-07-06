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
		public IList<RelativeTypeDto> RelativeTypes { get; private set; }

		public async Task InitializeAsync()
		{
			Countries = await GetCountriesAsync();
			ScheduleTypes = await GetScheduleTypesAsync();
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
