using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
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
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;

		public async Task<RoleDto> GetRoleAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetRoleAsync(dataService, id);
		}

		private static async Task<RoleDto> GetRoleAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetRoleAsync(id);
			if (item != null)
			{
				return await CreateRoleDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<RoleDto>> GetRolesAsync(DataRequest<Role> request)
		{
			var collection = new RoleCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<RoleDto>> GetRolesAsync(int skip, int take, DataRequest<Role> request)
		{
			var models = new List<RoleDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRolesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateRoleDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetRolesCountAsync(DataRequest<Role> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetRolesCountAsync(request);
		}

		public async Task<int> UpdateRoleAsync(RoleDto model)
		{
			long id = model.RoleID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetRoleAsync(model.RoleID) : new Role();
			if (item != null)
			{
				UpdateRoleFromDto(item, model);
				await dataService.UpdateRoleAsync(item);
				model.Merge(await GetRoleAsync(dataService, item.RoleID));
			}
			return 0;
		}

		public async Task<int> DeleteRoleAsync(RoleDto model)
		{
			var item = new Role { RoleID = model.RoleID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteRolesAsync(item);
		}

		public async Task<int> DeleteRoleRangeAsync(int index, int length, DataRequest<Role> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetRoleKeysAsync(index, length, request);
			return await dataService.DeleteRolesAsync([.. items]);
		}

		public static async Task<RoleDto> CreateRoleDtoAsync(Role source, bool includeAllFields)
		{
			var model = new RoleDto()
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

		private static void UpdateRoleFromDto(Role target, RoleDto source)
		{
			target.Name = source.Name;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
