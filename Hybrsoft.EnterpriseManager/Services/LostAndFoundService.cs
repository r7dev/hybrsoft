using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.EnterpriseManager.Tools;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class LostAndFoundService(IDataServiceFactory dataServiceFactory, ILogService logService) : ILostAndFoundService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly ILogService _logService = logService;

		public async Task<LostAndFoundModel> GetLostAndFoundAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await GetLostAndFoundAsync(dataService, id);
		}

		private static async Task<LostAndFoundModel> GetLostAndFoundAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetLostAndFoundAsync(id);
			if (item != null)
			{
				return await CreateLostAndFoundModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<LostAndFoundModel>> GetLostAndFoundAsync(DataRequest<LostAndFound> request)
		{
			var collection = new LostAndFoundCollection(this, _logService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<LostAndFoundModel>> GetLostAndFoundAsync(int skip, int take, DataRequest<LostAndFound> request)
		{
			var models = new List<LostAndFoundModel>();
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetLostAndFoundAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateLostAndFoundModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetLostAndFoundCountAsync(DataRequest<LostAndFound> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.GetLostAndFoundCountAsync(request);
		}

		public async Task<int> UpdateLostAndFoundAsync(LostAndFoundModel model)
		{
			long id = model.LostAndFoundID;
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetLostAndFoundAsync(model.LostAndFoundID) : new LostAndFound();
			if (item != null)
			{
				UpdateLostAndFoundFromModel(item, model);
				await dataService.UpdateLostAndFoundAsync(item);
				model.Merge(await GetLostAndFoundAsync(dataService, item.LostAndFoundID));
			}
			return 0;
		}

		public async Task<int> DeleteLostAndFoundAsync(LostAndFoundModel model)
		{
			var item = new LostAndFound { LostAndFoundID = model.LostAndFoundID };
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.DeleteLostAndFoundAsync(item);
		}

		public async Task<int> DeleteLostAndFoundRangeAsync(int index, int length, DataRequest<LostAndFound> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetLostAndFoundKeysAsync(index, length, request);
			return await dataService.DeleteLostAndFoundAsync([.. items]);
		}

		public static async Task<LostAndFoundModel> CreateLostAndFoundModelAsync(LostAndFound source, bool includeAllFields)
		{
			var model = new LostAndFoundModel()
			{
				LostAndFoundID = source.LostAndFoundID,
				DisplayName = source.DisplayName,
				Status = source.Status,
				Thumbnail = source.Thumbnail,
				ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.Description = source.Description;
				if (model.Status == LostAndFoundStatus.Claimed)
				{
					model.StudentBelongingID = source.StudentBelongingID ?? 0;
					model.StudentBelonging = await StudentBelongingService.CreateStudentBelongingModelAsync(source.StudentBelonging, includeAllFields);
				}
				model.DonationDate = source.DonationDate;
				model.Picture = source.Picture;
				model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
			}
			return model;
		}

		private static void UpdateLostAndFoundFromModel(LostAndFound target, LostAndFoundModel source)
		{
			target.DisplayName = source.DisplayName;
			target.Description = source.Description;
			target.Status = source.Status;
			target.StudentBelongingID = source.StudentBelongingID == 0 ? null : source.StudentBelongingID;
			target.DonationDate = source.Status == LostAndFoundStatus.Donated ? source.DonationDate : null;
			target.Picture = source.Picture;
			target.Thumbnail = source.Thumbnail;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = LookupTablesProxy.Instance?.GetLostAndFoundStatus((short)source.Status);
		}
	}
}
