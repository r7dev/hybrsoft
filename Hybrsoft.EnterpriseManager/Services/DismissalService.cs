using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.EnterpriseManager.Tools;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class DismissalService(IDataServiceFactory dataServiceFactory, ILogService logService) : IDismissalService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;

		public async Task<IList<DismissibleStudentDto>> GetDismissibleStudentsAsync(DataRequest<ClassroomStudent> request)
		{
			var collection = new DismissibleStudentCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<DismissibleStudentDto>> GetDismissibleStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request)
		{
			var models = new List<DismissibleStudentDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetDismissibleStudentsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateDismissibleStudentDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetDismissibleStudentsCountAsync(DataRequest<ClassroomStudent> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetDismissibleStudentsCountAsync(request);
		}

		public static async Task<DismissibleStudentDto> CreateDismissibleStudentDtoAsync(ClassroomStudent item, bool includeAllFields = false)
		{
			var model = new DismissibleStudentDto
			{
				ClassroomID = item.ClassroomID,
				ClassroomName = item.Classroom.Name,
				StudentID = item.StudentID,
				FirstName = item.Student.FirstName,
				MiddleName = item.Student.MiddleName,
				LastName = item.Student.LastName,
				ThumbnailSource = await BitmapTools.LoadBitmapAsync(item.Student.Thumbnail),
			};
			if (includeAllFields) {}
			return model;
		}

		public async Task<DismissalDto> GetDismissalAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetDismissalAsync(dataService, id);
		}

		private static async Task<DismissalDto> GetDismissalAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetDismissalAsync(id);
			if (item != null)
			{
				return await CreateDismissalDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<DismissalDto>> GetDismissalsAsync(DataRequest<Dismissal> request)
		{
			var collection = new DismissalCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<DismissalDto>> GetDismissalsAsync(int skip, int take, DataRequest<Dismissal> request)
		{
			var models = new List<DismissalDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetDismissalsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateDismissalDtoAsync(item, includeAllFields: true));
			}
			return models;
		}

		public async Task<int> GetDismissalsCountAsync(DataRequest<Dismissal> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetDismissalsCountAsync(request);
		}

		public async Task<int> UpdateDismissalAsync(DismissalDto model)
		{
			long id = model.DismissalID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0
				? await dataService.GetDismissalAsync(model.DismissalID)
				: new Dismissal() { Classroom = new Classroom(), Student = new Student(), Relative = new Relative() };
			if (item != null)
			{
				UpdateDismissalFromDto(item, model);
				await dataService.UpdateDismissalAsync(item);
				model.Merge(await GetDismissalAsync(dataService, item.DismissalID));
			}
			return 0;
		}


		public async Task<int> ApproveDismissalAsync(DismissalDto model)
		{
			var item = new Dismissal { DismissalID = model.DismissalID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.ApproveDismissalsAsync(item);
		}

		public async Task<int> ApproveDismissalRangeAsync(int index, int length, DataRequest<Dismissal> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetDismissalKeysAsync(index, length, request);
			return await dataService.ApproveDismissalsAsync([.. items]);
		}

		public static async Task<DismissalDto> CreateDismissalDtoAsync(Dismissal source, bool includeAllFields)
		{
			var model = new DismissalDto()
			{
				DismissalID = source.DismissalID,
				ClassroomID = source.ClassroomID,
				StudentID = source.StudentID,
				RelativeID = source.RelativeID,
				CreatedOn = source.CreatedOn,
				DismissedOn = source.DismissedOn
			};
			if (includeAllFields)
			{
				model.Classroom = await ClassroomService.CreateClassroomDtoAsync(source.Classroom, includeAllFields);
				model.Student = await StudentService.CreateStudentDtoAsync(source.Student, includeAllFields);
				model.Relative = await RelativeService.CreateRelativeDtoAsync(source.Relative, includeAllFields);
			}
			return model;
		}

		private static void UpdateDismissalFromDto(Dismissal target, DismissalDto source)
		{
			target.ClassroomID = source.ClassroomID;
			target.StudentID = source.StudentID;
			target.RelativeID = source.RelativeID;
			target.CreatedOn = source.CreatedOn;
			target.DismissedOn = source.DismissedOn;
			target.SearchTerms = source.Student?.FullName;
		}
	}
}
