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
	public class StudentService(IDataServiceFactory dataServiceFactory, ILogService logService) : IStudentService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;

		public async Task<StudentDto> GetStudentAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetStudentAsync(dataService, id);
		}

		private static async Task<StudentDto> GetStudentAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetStudentAsync(id);
			if (item != null)
			{
				return await CreateStudentDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<StudentDto>> GetStudentsAsync(DataRequest<Student> request)
		{
			var collection = new StudentCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<StudentDto>> GetStudentsAsync(int skip, int take, DataRequest<Student> request)
		{
			var models = new List<StudentDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateStudentDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetStudentsCountAsync(DataRequest<Student> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetStudentsCountAsync(request);
		}

		public async Task<int> UpdateStudentAsync(StudentDto model)
		{
			long id = model.StudentID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetStudentAsync(model.StudentID) : new Student();
			if (item != null)
			{
				UpdateStudentFromDto(item, model);
				await dataService.UpdateStudentAsync(item);
				model.Merge(await GetStudentAsync(dataService, item.StudentID));
			}
			return 0;
		}

		public async Task<int> DeleteStudentAsync(StudentDto model)
		{
			var item = new Student { StudentID = model.StudentID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteStudentsAsync(item);
		}

		public async Task<int> DeleteStudentRangeAsync(int index, int length, DataRequest<Student> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentKeysAsync(index, length, request);
			return await dataService.DeleteStudentsAsync([.. items]);
		}

		public static async Task<StudentDto> CreateStudentDtoAsync(Student source, bool includeAllFields)
		{
			var model = new StudentDto()
			{
				StudentID = source.StudentID,
				FirstName = source.FirstName,
				LastName = source.LastName,
				Email = source.Email,
				Thumbnail = source.Thumbnail,
				ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.MiddleName = source.MiddleName;
				model.Picture = source.Picture;
				model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
			}
			return model;
		}

		private static void UpdateStudentFromDto(Student target, StudentDto source)
		{
			target.FirstName = source.FirstName;
			target.MiddleName = source.MiddleName;
			target.LastName = source.LastName;
			target.Email = source.Email;
			target.Picture = source.Picture;
			target.Thumbnail = source.Thumbnail;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
