using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Tools;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class StudentBelongingService(IDataServiceFactory dataServiceFactory) : IStudentBelongingService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;

		public async Task<StudentBelongingModel> GetStudentBelongingAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await GetStudentBelongingAsync(dataService, id);
		}
		private static async Task<StudentBelongingModel> GetStudentBelongingAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetStudentBelongingAsync(id);
			if (item != null)
			{
				return await CreateStudentBelongingModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<StudentBelongingModel>> GetStudentBelongingsAsync(DataRequest<StudentBelonging> request)
		{
			// StudentBelongings are not virtualized
			return GetStudentBelongingsAsync(0, 100, request);
		}

		public async Task<IList<StudentBelongingModel>> GetStudentBelongingsAsync(int skip, int take, DataRequest<StudentBelonging> request)
		{
			var models = new List<StudentBelongingModel>();
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentBelongingsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateStudentBelongingModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetStudentBelongingsCountAsync(DataRequest<StudentBelonging> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.GetStudentBelongingsCountAsync(request);
		}

		public async Task<int> UpdateStudentBelongingAsync(StudentBelongingModel model)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = model.StudentBelongingID > 0
				? await dataService.GetStudentBelongingAsync(model.StudentBelongingID)
				: new StudentBelonging();
			if (item != null)
			{
				UpdateStudentBelongingFromModel(item, model);
				await dataService.UpdateStudentBelongingAsync(item);
				model.Merge(await GetStudentBelongingAsync(dataService, item.StudentBelongingID));
			}
			return 0;
		}

		public async Task<int> DeleteStudentBelongingAsync(StudentBelongingModel model)
		{
			var item = new StudentBelonging() { StudentBelongingID = model.StudentBelongingID };
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.DeleteStudentBelongingsAsync(item);
		}

		public async Task<int> DeleteStudentBelongingRangeAsync(int index, int length, DataRequest<StudentBelonging> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentBelongingKeysAsync(index, length, request);
			return await dataService.DeleteStudentBelongingsAsync([.. items]);
		}

		public static async Task<StudentBelongingModel> CreateStudentBelongingModelAsync(StudentBelonging source, bool includeAllFields)
		{
			var model = new StudentBelongingModel()
			{
				StudentBelongingID = source.StudentBelongingID,
				StudentID = source.StudentID,
				DisplayName = source.DisplayName,
				Thumbnail = source.Thumbnail,
				ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn,
				Student = source.Student is not null
					? await StudentService.CreateStudentModelAsync(source.Student, includeAllFields)
					: new StudentModel(),
			};
			if (includeAllFields)
			{
				model.Description = source.Description;
				model.Picture = source.Picture;
				model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
			}
			return model;
		}

		private static void UpdateStudentBelongingFromModel(StudentBelonging target, StudentBelongingModel source)
		{
			target.StudentID = source.StudentID;
			target.DisplayName = source.DisplayName;
			target.Description = source.Description;
			target.Picture = source.Picture;
			target.Thumbnail = source.Thumbnail;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
