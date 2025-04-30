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

		public IList<ScheduleTypeDto> ScheduleTypes { get; private set; }

		public async Task InitializeAsync()
		{
			ScheduleTypes = await GetScheduleTypesAsync();
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
				var currentLanguage = ResourceService.GetCurrentLanguageItem();
				var items = await dataService.GetScheduleTypesByLanguageAsync(currentLanguage.Tag);
				return [.. items.Select(r => new ScheduleTypeDto
				{
					ScheduleTypeID = r.ScheduleTypeId,
					Name = r.Name,
					LanguageTag = r.LanguageTag,
				})];
			}
			catch (Exception ex)
			{
				LogException("LookupTables", "Load ScheduleTypes", ex);
			}
			return [];
		}

		private async void LogException(string source, string action, Exception exception)
		{
			await LogService.WriteAsync(LogType.Error, source, action, exception.Message, exception.ToString());
		}
	}
}
