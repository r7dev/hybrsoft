using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
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
		static public ILookupTables LookupTables => LookupTablesProxy.Instance;

		public async Task<ClassroomDto> GetClassroomAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetClassroomAsync(dataService, id);
		}

		static private async Task<ClassroomDto> GetClassroomAsync(IDataService dataService, long id)
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
			var classroom = id > 0
				? await dataService.GetClassroomAsync(model.ClassroomID)
				: new Classroom() { ScheduleType = new ScheduleType() };
			if (classroom != null)
			{
				UpdateClassroomFromDto(classroom, model);
				await dataService.UpdateClassroomAsync(classroom);
				model.Merge(await GetClassroomAsync(dataService, classroom.ClassroomId));
			}
			return 0;
		}

		public async Task<int> DeleteClassroomAsync(ClassroomDto model)
		{
			var Classroom = new Classroom { ClassroomId = model.ClassroomID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteClassroomsAsync(Classroom);
		}

		public async Task<int> DeleteClassroomRangeAsync(int index, int length, DataRequest<Classroom> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomKeysAsync(index, length, request);
			return await dataService.DeleteClassroomsAsync([.. items]);
		}

		static public async Task<ClassroomDto> CreateClassroomDtoAsync(Classroom source, bool includeAllFields)
		{
			var model = new ClassroomDto()
			{
				ClassroomID = source.ClassroomId,
				Name = source.Name,
				Year = source.Year,
				EducationLevel = source.EducationLevel,
				ScheduleType = CreateSchedulerTypeDto(source.ScheduleType, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.ScheduleTypeID = source.ScheduleTypeId;
				model.MinimumYear = source.MinimumYear;
				model.MaximumYear = source.MaximumYear;
				model.MinimumEducationLevel = source.MinimumEducationLevel;
				model.MaximumEducationLevel = source.MaximumEducationLevel;
			}
			await Task.CompletedTask;
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
			target.ScheduleTypeId = source.ScheduleTypeID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.ScheduleType?.Name;
		}

		static public ScheduleTypeDto CreateSchedulerTypeDto(ScheduleType source, bool includeAllFields)
		{
			var model = new ScheduleTypeDto()
			{
				ScheduleTypeID = source.ScheduleTypeId,
				Name = string.IsNullOrEmpty(source.Uid)
					? source.Name
					: LookupTables.ScheduleTypes.FirstOrDefault(r => r.ScheduleTypeID == source.ScheduleTypeId).Name,
			};
			if (includeAllFields)
			{
			}
			return model;
		}
	}
}
