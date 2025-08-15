using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Interfaces;
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

		public async Task<RolePermissionModel> GetRolePermissionAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetRolePermissionAsync(dataService, id);
		}
		private static async Task<RolePermissionModel> GetRolePermissionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetRolePermissionAsync(id);
			if (item != null)
			{
				return await CreateRolePermissionModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<RolePermissionModel>> GetRolePermissionsAsync(DataRequest<RolePermission> request)
		{
			// RolePermissions are not virtualized
			return GetRolePermissionsAsync(0, 100, request);
		}

		public async Task<IList<RolePermissionModel>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request)
		{
			var models = new List<RolePermissionModel>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRolePermissionsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateRolePermissionModelAsync(item, includeAllFields: false));
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

		public async Task<int> UpdateRolePermissionAsync(RolePermissionModel model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.RolePermissionID > 0
				? await dataService.GetRolePermissionAsync(model.RolePermissionID)
				: new RolePermission() { Permission = new Permission() };
			if (item != null)
			{
				UpdateRolePermissionFromModel(item, model);
				await dataService.UpdateRolePermissionAsync(item);
				model.Merge(await GetRolePermissionAsync(dataService, item.RolePermissionID));
			}
			return 0;
		}

		public async Task<int> DeleteRolePermissionAsync(RolePermissionModel model)
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

		public static async Task<RolePermissionModel> CreateRolePermissionModelAsync(RolePermission source, bool includeAllFields)
		{
			var model = new RolePermissionModel()
			{
				RolePermissionID = source.RolePermissionID,
				RoleID = source.RoleID,
				PermissionID = source.PermissionID,
				Permission = await PermissionService.CreatePermissionModelAsync(source.Permission, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			return model;
		}

		private static void UpdateRolePermissionFromModel(RolePermission target, RolePermissionModel source)
		{
			target.RoleID = source.RoleID;
			target.PermissionID = source.PermissionID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Permission?.DisplayName;
		}
	}
}
