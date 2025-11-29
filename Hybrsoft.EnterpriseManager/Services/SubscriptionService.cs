using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class SubscriptionService(IDataServiceFactory dataServiceFactory, ILogService logService) : ISubscriptionService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly ILogService _logService = logService;

		public async Task<SubscriptionModel> GetSubscriptionAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await GetSubscriptionAsync(dataService, id);
		}

		private static async Task<SubscriptionModel> GetSubscriptionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetSubscriptionAsync(id);
			if (item != null)
			{
				return await CreateSubscriptionModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<SubscriptionModel>> GetSubscriptionsAsync(DataRequest<Subscription> request)
		{
			var collection = new SubscriptionCollection(this, _logService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<SubscriptionModel>> GetSubscriptionsAsync(int skip, int take, DataRequest<Subscription> request)
		{
			var models = new List<SubscriptionModel>();
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetSubscriptionsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateSubscriptionModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetSubscriptionsCountAsync(DataRequest<Subscription> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.GetSubscriptionsCountAsync(request);
		}

		public async Task<int> UpdateSubscriptionAsync(SubscriptionModel model)
		{
			long id = model.SubscriptionID;
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetSubscriptionAsync(model.SubscriptionID) : new Subscription();
			if (item != null)
			{
				UpdateSubscriptionFromModel(item, model);
				await dataService.UpdateSubscriptionAsync(item);
				model.Merge(await GetSubscriptionAsync(dataService, item.SubscriptionID));
			}
			return 0;
		}

		public async Task<int> DeleteSubscriptionAsync(SubscriptionModel model)
		{
			var item = new Subscription { SubscriptionID = model.SubscriptionID };
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.DeleteSubscriptionsAsync(item);
		}

		public async Task<int> DeleteSubscriptionRangeAsync(int index, int length, DataRequest<Subscription> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetSubscriptionKeysAsync(index, length, request);
			return await dataService.DeleteSubscriptionsAsync([.. items]);
		}

		public static async Task<SubscriptionModel> CreateSubscriptionModelAsync(Subscription source, bool includeAllFields)
		{
			var model = new SubscriptionModel()
			{
				SubscriptionID = source.SubscriptionID,
				LicenseKey = source.LicenseKey,
				Status = source.Status,
				ExpirationDate = source.ExpirationDate
			};
			if (includeAllFields)
			{
				model.SubscriptionPlanID = source.SubscriptionPlanID;
				model.DurationDays = source.DurationDays;
				model.StartDate = source.StartDate;
				model.Type = source.Type;
				if (model.Type == SubscriptionType.Enterprise)
				{
					model.CompanyID = source.CompanyID ?? 0;
					model.Company = await CompanyService.CreateCompanyModelAsync(source.Company, includeAllFields);
				}
				else if (model.Type == SubscriptionType.Individual)
				{
					model.UserID = source.UserID ?? 0;
					model.User = await UserService.CreateUserModelAsync(source.User, includeAllFields);
				}
				model.CancelledOn = source.CancelledOn;
				model.LastValidatedOn = source.LastValidatedOn;
				model.CreatedOn = source.CreatedOn;
				model.LastModifiedOn = source.LastModifiedOn;
			}
			return model;
		}

		private static void UpdateSubscriptionFromModel(Subscription target, SubscriptionModel source)
		{
			target.SubscriptionPlanID = source.SubscriptionPlanID;
			target.DurationDays = source.DurationDays;
			target.StartDate = source.StartDate;
			target.ExpirationDate = source.ExpirationDate;
			target.Type = source.Type;
			target.CompanyID = source.CompanyID == 0 ? null : source.CompanyID;
			target.UserID = source.UserID == 0 ? null : source.UserID;
			target.LicenseKey = source.LicenseKey;
			target.Status = source.Status;
			target.CancelledOn = source.CancelledOn;
			target.LastValidatedOn = source.LastValidatedOn;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.LicencedTo;
		}
	}
}
