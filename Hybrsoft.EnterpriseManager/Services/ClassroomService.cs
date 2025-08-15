using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class ClassroomService(IDataServiceFactory dataServiceFactory, ILogService logService) : IClassroomService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;
		public static ILookupTables LookupTables => LookupTablesProxy.Instance;

		public async Task<ClassroomDto> GetClassroomAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetClassroomAsync(dataService, id);
		}

		private static async Task<ClassroomDto> GetClassroomAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetClassroomAsync(id);
			if (item != null)
			{
				return await CreateClassroomDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<ClassroomDto>> GetClassroomsAsync(DataRequest<Classroom> request)
		{
			var collection = new ClassroomCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<ClassroomDto>> GetClassroomsAsync(int skip, int take, DataRequest<Classroom> request)
		{
			var models = new List<ClassroomDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateClassroomDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetClassroomsCountAsync(DataRequest<Classroom> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetClassroomsCountAsync(request);
		}

		public async Task<int> UpdateClassroomAsync(ClassroomDto model)
		{
			long id = model.ClassroomID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0
				? await dataService.GetClassroomAsync(model.ClassroomID)
				: new Classroom() { ScheduleType = new ScheduleType() };
			if (item != null)
			{
				UpdateClassroomFromDto(item, model);
				await dataService.UpdateClassroomAsync(item);
				model.Merge(await GetClassroomAsync(dataService, item.ClassroomID));
			}
			return 0;
		}

		public async Task<int> DeleteClassroomAsync(ClassroomDto model)
		{
			var item = new Classroom { ClassroomID = model.ClassroomID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteClassroomsAsync(item);
		}

		public async Task<int> DeleteClassroomRangeAsync(int index, int length, DataRequest<Classroom> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomKeysAsync(index, length, request);
			return await dataService.DeleteClassroomsAsync([.. items]);
		}

		public static async Task<ClassroomDto> CreateClassroomDtoAsync(Classroom source, bool includeAllFields)
		{
			var model = new ClassroomDto()
			{
				ClassroomID = source.ClassroomID,
				Name = source.Name,
				Year = source.Year,
				EducationLevel = source.EducationLevel,
				ScheduleType = await CreateSchedulerTypeDtoAsync(source.ScheduleType, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.ScheduleTypeID = source.ScheduleTypeID;
				model.MinimumYear = source.MinimumYear;
				model.MaximumYear = source.MaximumYear;
				model.MinimumEducationLevel = source.MinimumEducationLevel;
				model.MaximumEducationLevel = source.MaximumEducationLevel;
			}
			return model;
		}

		private static void UpdateClassroomFromDto(Classroom target, ClassroomDto source)
		{
			target.Name = source.Name;
			target.Year = source.Year;
			target.MinimumYear = source.MinimumYear;
			target.MaximumYear = source.MaximumYear;
			target.EducationLevel = source.EducationLevel;
			target.MinimumEducationLevel = source.MinimumEducationLevel;
			target.MaximumEducationLevel = source.MaximumEducationLevel;
			target.ScheduleTypeID = source.ScheduleTypeID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.ScheduleType?.Name;
		}

		private static async Task<ScheduleTypeDto> CreateSchedulerTypeDtoAsync(ScheduleType source, bool includeAllFields)
		{
			var model = new ScheduleTypeDto()
			{
				ScheduleTypeID = source.ScheduleTypeID,
				Name = string.IsNullOrEmpty(source.Uid)
					? source.Name
					: LookupTables.GetScheduleType(source.ScheduleTypeID),
			};
			if (includeAllFields) { }
			await Task.CompletedTask;
			return model;
		}
	}
}
