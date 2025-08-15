using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class CompanyUserService(IDataServiceFactory dataServiceFactory) : ICompanyUserService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<CompanyUserDto> GetCompanyUserAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetCompanyUserAsync(dataService, id);
		}
		private static async Task<CompanyUserDto> GetCompanyUserAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetCompanyUserAsync(id);
			if (item != null)
			{
				return await CreateCompanyUserDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<CompanyUserDto>> GetCompanyUsersAsync(DataRequest<CompanyUser> request)
		{
			// CompanyUsers are not virtualized
			return GetCompanyUsersAsync(0, 100, request);
		}

		public async Task<IList<CompanyUserDto>> GetCompanyUsersAsync(int skip, int take, DataRequest<CompanyUser> request)
		{
			var models = new List<CompanyUserDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetCompanyUsersAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateCompanyUserDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedUserKeysInCompanyAsync(long parentID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedUserKeysInCompanyAsync(parentID);
		}

		public async Task<int> GetCompanyUsersCountAsync(DataRequest<CompanyUser> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetCompanyUsersCountAsync(request);
		}

		public async Task<int> UpdateCompanyUserAsync(CompanyUserDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.CompanyUserID > 0
				? await dataService.GetCompanyUserAsync(model.CompanyUserID)
				: new CompanyUser() { User = new User() };
			if (item != null)
			{
				UpdateCompanyUserFromDto(item, model);
				await dataService.UpdateCompanyUserAsync(item);
				model.Merge(await GetCompanyUserAsync(dataService, item.CompanyUserID));
			}
			return 0;
		}

		public async Task<int> DeleteCompanyUserAsync(CompanyUserDto model)
		{
			var item = new CompanyUser() { CompanyUserID = model.CompanyUserID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteCompanyUsersAsync(item);
		}

		public async Task<int> DeleteCompanyUserRangeAsync(int index, int length, DataRequest<CompanyUser> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetCompanyUsersAsync(index, length, request);
			return await dataService.DeleteCompanyUsersAsync([.. items]);
		}

		public static async Task<CompanyUserDto> CreateCompanyUserDtoAsync(CompanyUser source, bool includeAllFields)
		{
			var model = new CompanyUserDto
			{
				CompanyUserID = source.CompanyUserID,
				CompanyID = source.CompanyID,
				UserID = source.UserID,
				User = await UserService.CreateUserDtoAsync(source.User, includeAllFields)
			};
			if (includeAllFields)
			{
				model.CreatedOn = source.CreatedOn;
				model.LastModifiedOn = source.LastModifiedOn;
			}
			return model;
		}

		private static void UpdateCompanyUserFromDto(CompanyUser target, CompanyUserDto source)
		{
			target.CompanyID = source.CompanyID;
			target.UserID = source.UserID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.User?.FullName;
		}
	}
}
