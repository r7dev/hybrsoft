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
	public class UserRoleService(IDataServiceFactory dataServiceFactory) : IUserRoleService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<UserRoleDto> GetUserRoleAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetUserRoleAsync(dataService, id);
		}
		private static async Task<UserRoleDto> GetUserRoleAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetUserRoleAsync(id);
			if (item != null)
			{
				return await CreateUserRoleDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<UserRoleDto>> GetUserRolesAsync(DataRequest<UserRole> request)
		{
			// UserRoles are not virtualized
			return GetUserRolesAsync(0, 100, request);
		}

		public async Task<IList<UserRoleDto>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request)
		{
			var models = new List<UserRoleDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetUserRolesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateUserRoleDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedRoleKeysInUserAsync(long parentID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedRoleKeysInUserAsync(parentID);
		}

		public async Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetUserRolesCountAsync(request);
		}

		public async Task<int> UpdateUserRoleAsync(UserRoleDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.UserRoleID > 0
				? await dataService.GetUserRoleAsync(model.UserRoleID)
				: new UserRole() { Role = new Role() };
			if (item != null)
			{
				UpdateUserRoleFromDto(item, model);
				await dataService.UpdateUserRoleAsync(item);
				model.Merge(await GetUserRoleAsync(dataService, item.UserRoleID));
			}
			return 0;
		}

		public async Task<int> DeleteUserRoleAsync(UserRoleDto model)
		{
			var item = new UserRole() { UserRoleID = model.UserRoleID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteUserRolesAsync(item);
		}

		public async Task<int> DeleteUserRoleRangeAsync(int index, int length, DataRequest<UserRole> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetUserRolesAsync(index, length, request);
			return await dataService.DeleteUserRolesAsync([..items]);
		}

		public static async Task<UserRoleDto> CreateUserRoleDtoAsync(UserRole source, bool includeAllFields)
		{
			var model = new UserRoleDto()
			{
				UserRoleID = source.UserRoleID,
				UserID = source.UserID,
				RoleID = source.RoleID,
				Role = await RoleService.CreateRoleDtoAsync(source.Role, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			return model;
		}

		private static void UpdateUserRoleFromDto(UserRole target, UserRoleDto source)
		{
			target.UserID = source.UserID;
			target.RoleID = source.RoleID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Role?.Name;
		}
	}
}
