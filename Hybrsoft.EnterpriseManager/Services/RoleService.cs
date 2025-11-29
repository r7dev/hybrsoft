using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class RoleService(IDataServiceFactory dataServiceFactory, ILogService logService) : IRoleService
	{
		private readonly IDataServiceFactory _dataServiceFactory = dataServiceFactory;
		private readonly ILogService _logService = logService;

		public async Task<RoleModel> GetRoleAsync(long id)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await GetRoleAsync(dataService, id);
		}

		private static async Task<RoleModel> GetRoleAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetRoleAsync(id);
			if (item != null)
			{
				return await CreateRoleModelAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<RoleModel>> GetRolesAsync(DataRequest<Role> request)
		{
			var collection = new RoleCollection(this, _logService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<RoleModel>> GetRolesAsync(int skip, int take, DataRequest<Role> request)
		{
			var models = new List<RoleModel>();
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetRolesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateRoleModelAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetRolesCountAsync(DataRequest<Role> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.GetRolesCountAsync(request);
		}

		public async Task<int> UpdateRoleAsync(RoleModel model)
		{
			long id = model.RoleID;
			using var dataService = _dataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetRoleAsync(model.RoleID) : new Role();
			if (item != null)
			{
				UpdateRoleFromModel(item, model);
				await dataService.UpdateRoleAsync(item);
				model.Merge(await GetRoleAsync(dataService, item.RoleID));
			}
			return 0;
		}

		public async Task<int> DeleteRoleAsync(RoleModel model)
		{
			var item = new Role { RoleID = model.RoleID };
			using var dataService = _dataServiceFactory.CreateDataService();
			return await dataService.DeleteRolesAsync(item);
		}

		public async Task<int> DeleteRoleRangeAsync(int index, int length, DataRequest<Role> request)
		{
			using var dataService = _dataServiceFactory.CreateDataService();
			var items = await dataService.GetRoleKeysAsync(index, length, request);
			return await dataService.DeleteRolesAsync([.. items]);
		}

		public static async Task<RoleModel> CreateRoleModelAsync(Role source, bool includeAllFields)
		{
			var model = new RoleModel()
			{
				RoleID = source.RoleID,
				Name = source.Name,
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) { }
			await Task.CompletedTask;
			return model;
		}

		private static void UpdateRoleFromModel(Role target, RoleModel source)
		{
			target.Name = source.Name;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
