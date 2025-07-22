using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class SubscriptionService(IDataServiceFactory dataServiceFactory, ILogService logService) : ISubscriptionService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;

		public async Task<SubscriptionDto> GetSubscriptionAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetSubscriptionAsync(dataService, id);
		}

		private static async Task<SubscriptionDto> GetSubscriptionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetSubscriptionAsync(id);
			if (item != null)
			{
				return await CreateSubscriptionDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<SubscriptionDto>> GetSubscriptionsAsync(DataRequest<Subscription> request)
		{
			var collection = new SubscriptionCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<SubscriptionDto>> GetSubscriptionsAsync(int skip, int take, DataRequest<Subscription> request)
		{
			var models = new List<SubscriptionDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetSubscriptionsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateSubscriptionDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetSubscriptionsCountAsync(DataRequest<Subscription> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetSubscriptionsCountAsync(request);
		}

		public async Task<int> UpdateSubscriptionAsync(SubscriptionDto model)
		{
			long id = model.SubscriptionID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetSubscriptionAsync(model.SubscriptionID) : new Subscription();
			if (item != null)
			{
				UpdateSubscriptionFromDto(item, model);
				await dataService.UpdateSubscriptionAsync(item);
				model.Merge(await GetSubscriptionAsync(dataService, item.SubscriptionID));
			}
			return 0;
		}

		public async Task<int> DeleteSubscriptionAsync(SubscriptionDto model)
		{
			var item = new Subscription { SubscriptionID = model.SubscriptionID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteSubscriptionsAsync(item);
		}

		public async Task<int> DeleteSubscriptionRangeAsync(int index, int length, DataRequest<Subscription> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetSubscriptionKeysAsync(index, length, request);
			return await dataService.DeleteSubscriptionsAsync([.. items]);
		}

		public static async Task<SubscriptionDto> CreateSubscriptionDtoAsync(Subscription source, bool includeAllFields)
		{
			var model = new SubscriptionDto()
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
					model.Company = await CompanyService.CreateCompanyDtoAsync(source.Company, includeAllFields);
				}
				else if (model.Type == SubscriptionType.Individual)
				{
					model.UserID = source.UserID ?? 0;
					model.User = await UserService.CreateUserDtoAsync(source.User, includeAllFields);
				}
				model.CancelledOn = source.CancelledOn;
				model.LastValidatedOn = source.LastValidatedOn;
				model.CreatedOn = source.CreatedOn;
				model.LastModifiedOn = source.LastModifiedOn;
			}
			return model;
		}

		private static void UpdateSubscriptionFromDto(Subscription target, SubscriptionDto source)
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
