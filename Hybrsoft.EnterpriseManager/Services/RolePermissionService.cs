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
	public class RolePermissionService(IDataServiceFactory dataServiceFactory) : IRolePermissionService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<RolePermissionDto> GetRolePermissionAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetRolePermissionAsync(dataService, id);
		}
		private static async Task<RolePermissionDto> GetRolePermissionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetRolePermissionAsync(id);
			if (item != null)
			{
				return await CreateRolePermissionDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<RolePermissionDto>> GetRolePermissionsAsync(DataRequest<RolePermission> request)
		{
			// RolePermissions are not virtualized
			return GetRolePermissionsAsync(0, 100, request);
		}

		public async Task<IList<RolePermissionDto>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request)
		{
			var models = new List<RolePermissionDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRolePermissionsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateRolePermissionDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedPermissionKeysInRoleAsync(long roleID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedPermissionKeysInRoleAsync(roleID);
		}

		public async Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetRolePermissionsCountAsync(request);
		}

		public async Task<int> UpdateRolePermissionAsync(RolePermissionDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.RolePermissionID > 0
				? await dataService.GetRolePermissionAsync(model.RolePermissionID)
				: new RolePermission() { Permission = new Permission() };
			if (item != null)
			{
				UpdateRolePermissionFromDto(item, model);
				await dataService.UpdateRolePermissionAsync(item);
				model.Merge(await GetRolePermissionAsync(dataService, item.RolePermissionID));
			}
			return 0;
		}

		public async Task<int> DeleteRolePermissionAsync(RolePermissionDto model)
		{
			var item = new RolePermission { RolePermissionID = model.RolePermissionID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteRolePermissionsAsync(item);
		}

		public async Task<int> DeleteRolePermissionRangeAsync(int index, int length, DataRequest<RolePermission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRolePermissionKeysAsync(index, length, request);
			return await dataService.DeleteRolePermissionsAsync([..items]);
		}

		public static async Task<RolePermissionDto> CreateRolePermissionDtoAsync(RolePermission source, bool includeAllFields)
		{
			var model = new RolePermissionDto()
			{
				RolePermissionID = source.RolePermissionID,
				RoleID = source.RoleID,
				PermissionID = source.PermissionID,
				Permission = await PermissionService.CreatePermissionDtoAsync(source.Permission, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			return model;
		}

		private static void UpdateRolePermissionFromDto(RolePermission target, RolePermissionDto source)
		{
			target.RoleID = source.RoleID;
			target.PermissionID = source.PermissionID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Permission?.DisplayName;
		}
	}
}
