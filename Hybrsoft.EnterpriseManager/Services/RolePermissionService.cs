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

		public async Task<RolePermissionDto> GetRolePermissionAsync(long rolePermissionId)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetRolePermissionAsync(dataService, rolePermissionId);
		}
		static private async Task<RolePermissionDto> GetRolePermissionAsync(IDataService dataService, long rolePermissionId)
		{
			var item = await dataService.GetRolePermissionAsync(rolePermissionId);
			if (item != null)
			{
				return CreateRolePermissionDto(item, includeAllFields: true);
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
				models.Add(CreateRolePermissionDto(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetRolePermissionsCountAsync(request);
		}

		public async Task<int> UpdateRolePermissionAsync(RolePermissionDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var rolePermission = model.RolePermissionID > 0
				? await dataService.GetRolePermissionAsync(model.RolePermissionID)
				: new RolePermission() { Permission = new Permission() };
			if (rolePermission != null)
			{
				UpdateRolePermissionFromDto(rolePermission, model);
				await dataService.UpdateRolePermissionAsync(rolePermission);
				model.Merge(await GetRolePermissionAsync(dataService, rolePermission.RolePermissionId));
			}
			return 0;
		}

		public async Task<int> DeleteRolePermissionAsync(RolePermissionDto model)
		{
			var rolePermission = new RolePermission { RolePermissionId = model.RolePermissionID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteRolePermissionsAsync(rolePermission);
		}

		public async Task<int> DeleteRolePermissionRangeAsync(int index, int length, DataRequest<RolePermission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRolePermissionKeysAsync(index, length, request);
			return await dataService.DeleteRolePermissionsAsync([..items]);
		}

		static public RolePermissionDto CreateRolePermissionDto(RolePermission source, bool includeAllFields)
		{
			var model = new RolePermissionDto()
			{
				RolePermissionID = source.RolePermissionId,
				RoleID = source.RoleId,
				PermissionID = source.PermissionId,
				Permission = PermissionService.CreatePermissionDto(source.Permission, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			return model;
		}

		private static void UpdateRolePermissionFromDto(RolePermission target, RolePermissionDto source)
		{
			target.RoleId = source.RoleID;
			target.PermissionId = source.PermissionID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Permission?.DisplayName;
		}
	}
}
