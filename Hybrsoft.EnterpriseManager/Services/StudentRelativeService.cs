using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class StudentRelativeService(IDataServiceFactory dataServiceFactory) : IStudentRelativeService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<StudentRelativeModel> GetStudentRelativeAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetStudentRelativeAsync(dataService, id);
		}
		private static async Task<StudentRelativeModel> GetStudentRelativeAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetStudentRelativeAsync(id);
			if (item != null)
			{
				return await CreateStudentRelativeModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<StudentRelativeModel>> GetStudentRelativesAsync(DataRequest<StudentRelative> request)
		{
			// StudentRelatives are not virtualized
			return GetStudentRelativesAsync(0, 100, request);
		}

		public async Task<IList<StudentRelativeModel>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request)
		{
			var models = new List<StudentRelativeModel>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentRelativesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateStudentRelativeModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedRelativeKeysInStudentAsync(long parentID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedRelativeKeysInStudentAsync(parentID);
		}

		public async Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetStudentRelativesCountAsync(request);
		}

		public async Task<int> UpdateStudentRelativeAsync(StudentRelativeModel model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.StudentRelativeID > 0
				? await dataService.GetStudentRelativeAsync(model.StudentRelativeID)
				: new StudentRelative() { Relative = new Relative() };
			if (item != null)
			{
				UpdateStudentRelativeFromModel(item, model);
				await dataService.UpdateStudentRelativeAsync(item);
				model.Merge(await GetStudentRelativeAsync(dataService, item.StudentRelativeID));
			}
			return 0;
		}

		public async Task<int> DeleteStudentRelativeAsync(StudentRelativeModel model)
		{
			var item = new StudentRelative() { StudentRelativeID = model.StudentRelativeID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteStudentRelativesAsync(item);
		}

		public async Task<int> DeleteStudentRelativeRangeAsync(int index, int length, DataRequest<StudentRelative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentRelativesAsync(index, length, request);
			return await dataService.DeleteStudentRelativesAsync([.. items]);
		}

		public static async Task<StudentRelativeModel> CreateStudentRelativeModelAsync(StudentRelative source, bool includeAllFields)
		{
			var model = new StudentRelativeModel()
			{
				StudentRelativeID = source.StudentRelativeID,
				StudentID = source.StudentID,
				RelativeID = source.RelativeID,
				Relative = await RelativeService.CreateRelativeModelAsync(source.Relative, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) { }
			return model;
		}

		private static void UpdateStudentRelativeFromModel(StudentRelative target, StudentRelativeModel source)
		{
			target.StudentID = source.StudentID;
			target.RelativeID = source.RelativeID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Relative?.FullName;
		}
	}
}
