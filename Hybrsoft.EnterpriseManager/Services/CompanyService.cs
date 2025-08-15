using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
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
	public class CompanyService(IDataServiceFactory dataServiceFactory, ILogService logService) : ICompanyService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;
		static public ILookupTables LookupTables => LookupTablesProxy.Instance;

		public async Task<CompanyModel> GetCompanyAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetCompanyAsync(dataService, id);
		}

		private static async Task<CompanyModel> GetCompanyAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetCompanyAsync(id);
			if (item != null)
			{
				return await CreateCompanyModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<CompanyModel>> GetCompaniesAsync(DataRequest<Company> request)
		{
			var collection = new CompanyCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<CompanyModel>> GetCompaniesAsync(int skip, int take, DataRequest<Company> request)
		{
			var models = new List<CompanyModel>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetCompaniesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateCompanyModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetCompaniesCountAsync(DataRequest<Company> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetCompaniesCountAsync(request);
		}

		public async Task<int> UpdateCompanyAsync(CompanyModel model)
		{
			long id = model.CompanyID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0
				? await dataService.GetCompanyAsync(model.CompanyID)
				: new Company() { Country = new Country() };
			if (item != null)
			{
				UpdateCompanyFromModel(item, model);
				await dataService.UpdateCompanyAsync(item);
				model.Merge(await GetCompanyAsync(dataService, item.CompanyID));
			}
			return 0;
		}

		public async Task<int> DeleteCompanyAsync(CompanyModel model)
		{
			var item = new Company { CompanyID = model.CompanyID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteCompaniesAsync(item);
		}

		public async Task<int> DeleteCompanyRangeAsync(int index, int length, DataRequest<Company> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetCompanyKeysAsync(index, length, request);
			return await dataService.DeleteCompaniesAsync([.. items]);
		}

		public static async Task<CompanyModel> CreateCompanyModelAsync(Company source, bool includeAllFields)
		{
			var model = new CompanyModel
			{
				CompanyID = source.CompanyID,
				LegalName = source.LegalName,
				TradeName = source.TradeName,
				FederalRegistration = source.FederalRegistration,
				Phone = source.Phone
			};
			if (includeAllFields)
			{
				model.StateRegistration = source.StateRegistration;
				model.CityLicense = source.CityLicense;
				model.CountryID = source.CountryID;
				model.Country = await CreateCompanyModelAsync(source.Country, includeAllFields);
				model.Email = source.Email;
			}
			return model;
		}

		private static void UpdateCompanyFromModel(Company target, CompanyModel source)
		{
			target.LegalName = source.LegalName;
			target.TradeName = source.TradeName;
			target.FederalRegistration = source.FederalRegistration;
			target.StateRegistration = source.StateRegistration;
			target.CityLicense = source.CityLicense;
			target.CountryID = source.CountryID;
			target.Phone = source.Phone;
			target.Email = source.Email;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}

		private static async Task<CountryModel> CreateCompanyModelAsync(Country source, bool includeAllFields)
		{
			var model = new CountryModel
			{
				CountryID = source.CountryID,
				Name = string.IsNullOrEmpty(source.Uid)
					? source.Name
					: LookupTables.Countries.FirstOrDefault(r => r.CountryID == source.CountryID).Name,
			};
			if (includeAllFields) { }
			await Task.CompletedTask;
			return model;
		}
	}
}
