using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
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

		public async Task<UserRoleDto> GetUserRoleAsync(long userRoleId)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetUserRoleAsync(dataService, userRoleId);
		}
		static private async Task<UserRoleDto> GetUserRoleAsync(IDataService dataService, long userRoleId)
		{
			var item = await dataService.GetUserRoleAsync(userRoleId);
			if (item != null)
			{
				return CreateUserRoleDto(item, includeAllFields: true);
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
				models.Add(CreateUserRoleDto(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedRoleKeysAsync(long userId)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedRoleKeysAsync(userId);
		}

		public async Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetUserRolesCountAsync(request);
		}

		public async Task<int> UpdateUserRoleAsync(UserRoleDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var userRole = model.UserRoleID > 0
				? await dataService.GetUserRoleAsync(model.UserRoleID)
				: new UserRole() { Role = new Role() };
			if (userRole != null)
			{
				UpdateUserRoleFromDto(userRole, model);
				await dataService.UpdateUserRoleAsync(userRole);
				model.Merge(await GetUserRoleAsync(dataService, userRole.UserRoleId));
			}
			return 0;
		}

		public async Task<int> DeleteUserRoleAsync(UserRoleDto model)
		{
			var userRole = new UserRole() { UserRoleId = model.UserRoleID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteUserRolesAsync(userRole);
		}

		public async Task<int> DeleteUserRoleRangeAsync(int index, int length, DataRequest<UserRole> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetUserRolesAsync(index, length, request);
			return await dataService.DeleteUserRolesAsync([..items]);
		}

		static public UserRoleDto CreateUserRoleDto(UserRole source, bool includeAllFields)
		{
			var model = new UserRoleDto()
			{
				UserRoleID = source.UserRoleId,
				UserID = source.UserId,
				RoleID = source.RoleId,
				Role = RoleService.CreateRoleDto(source.Role, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			return model;
		}

		private static void UpdateUserRoleFromDto(UserRole target, UserRoleDto source)
		{
			target.UserId = source.UserID;
			target.RoleId = source.RoleID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Role?.Name;
		}
	}
}
