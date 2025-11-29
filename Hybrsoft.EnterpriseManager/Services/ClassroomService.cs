using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class ClassroomService(IDataServiceFactory dataServiceFactory, ILogService logService) : IClassroomService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly ILogService _logService = logService;
		private static ILookupTables LookupTables => LookupTablesProxy.Instance;

		public async Task<ClassroomModel> GetClassroomAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await GetClassroomAsync(dataService, id);
		}

		private static async Task<ClassroomModel> GetClassroomAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetClassroomAsync(id);
			if (item != null)
			{
				return await CreateClassroomModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<ClassroomModel>> GetClassroomsAsync(DataRequest<Classroom> request)
		{
			var collection = new ClassroomCollection(this, _logService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<ClassroomModel>> GetClassroomsAsync(int skip, int take, DataRequest<Classroom> request)
		{
			var models = new List<ClassroomModel>();
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateClassroomModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetClassroomsCountAsync(DataRequest<Classroom> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.GetClassroomsCountAsync(request);
		}

		public async Task<int> UpdateClassroomAsync(ClassroomModel model)
		{
			long id = model.ClassroomID;
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = id > 0
				? await dataService.GetClassroomAsync(model.ClassroomID)
				: new Classroom() { ScheduleType = new ScheduleType() };
			if (item != null)
			{
				UpdateClassroomFromModel(item, model);
				await dataService.UpdateClassroomAsync(item);
				model.Merge(await GetClassroomAsync(dataService, item.ClassroomID));
			}
			return 0;
		}

		public async Task<int> DeleteClassroomAsync(ClassroomModel model)
		{
			var item = new Classroom { ClassroomID = model.ClassroomID };
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.DeleteClassroomsAsync(item);
		}

		public async Task<int> DeleteClassroomRangeAsync(int index, int length, DataRequest<Classroom> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomKeysAsync(index, length, request);
			return await dataService.DeleteClassroomsAsync([.. items]);
		}

		public static async Task<ClassroomModel> CreateClassroomModelAsync(Classroom source, bool includeAllFields)
		{
			var model = new ClassroomModel()
			{
				ClassroomID = source.ClassroomID,
				Name = source.Name,
				Year = source.Year,
				EducationLevel = source.EducationLevel,
				ScheduleType = await CreateSchedulerTypeModelAsync(source.ScheduleType, includeAllFields),
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

		private static void UpdateClassroomFromModel(Classroom target, ClassroomModel source)
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

		private static async Task<ScheduleTypeModel> CreateSchedulerTypeModelAsync(ScheduleType source, bool includeAllFields)
		{
			var model = new ScheduleTypeModel()
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
