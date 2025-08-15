using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
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
	public class RelativeService(IDataServiceFactory dataServiceFactory, ILogService logService) : IRelativeService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;
		public static ILookupTables LookupTables => LookupTablesProxy.Instance;

		public async Task<RelativeModel> GetRelativeAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetRelativeAsync(dataService, id);
		}

		private static async Task<RelativeModel> GetRelativeAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetRelativeAsync(id);
			if (item != null)
			{
				return await CreateRelativeModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<RelativeModel>> GetRelativesAsync(DataRequest<Relative> request)
		{
			var collection = new RelativeCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<RelativeModel>> GetRelativesAsync(int skip, int take, DataRequest<Relative> request)
		{
			var models = new List<RelativeModel>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRelativesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateRelativeModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetRelativesCountAsync(DataRequest<Relative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetRelativesCountAsync(request);
		}

		public async Task<int> UpdateRelativeAsync(RelativeModel model)
		{
			long id = model.RelativeID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetRelativeAsync(model.RelativeID) : new Relative();
			if (item != null)
			{
				UpdateRelativeFromModel(item, model);
				await dataService.UpdateRelativeAsync(item);
				model.Merge(await GetRelativeAsync(dataService, item.RelativeID));
			}
			return 0;
		}

		public async Task<int> DeleteRelativeAsync(RelativeModel model)
		{
			var item = new Relative { RelativeID = model.RelativeID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteRelativesAsync(item);
		}

		public async Task<int> DeleteRelativeRangeAsync(int index, int length, DataRequest<Relative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRelativeKeysAsync(index, length, request);
			return await dataService.DeleteRelativesAsync([.. items]);
		}

		public static async Task<RelativeModel> CreateRelativeModelAsync(Relative source, bool includeAllFields)
		{
			var model = new RelativeModel()
			{
				RelativeID = source.RelativeID,
				FirstName = source.FirstName,
				LastName = source.LastName,
				Phone = source.Phone,
				Thumbnail = source.Thumbnail,
				ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.MiddleName = source.MiddleName;
				model.RelativeTypeID = source.RelativeTypeID;
				model.RelativeType = await CreateRelativeTypeModelAsync(source.RelativeType, includeAllFields);
				model.DocumentNumber = source.DocumentNumber;
				model.Email = source.Email;
				model.Picture = source.Picture;
				model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
			}
			return model;
		}

		private static void UpdateRelativeFromModel(Relative target, RelativeModel source)
		{
			target.FirstName = source.FirstName;
			target.MiddleName = source.MiddleName;
			target.LastName = source.LastName;
			target.RelativeTypeID = source.RelativeTypeID;
			target.DocumentNumber = source.DocumentNumber;
			target.Phone = source.Phone;
			target.Email = source.Email;
			target.Picture = source.Picture;
			target.Thumbnail = source.Thumbnail;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}

		public static async Task<RelativeTypeModel> CreateRelativeTypeModelAsync(RelativeType source, bool includeAllFields)
		{
			var model = new RelativeTypeModel()
			{
				RelativeTypeID = source.RelativeTypeID,
				Name = string.IsNullOrEmpty(source.Uid)
					? source.Name
					: LookupTables.GetRelativeType(source.RelativeTypeID),
			};
			if (includeAllFields) { }
			await Task.CompletedTask;
			return model;
		}
	}
}
