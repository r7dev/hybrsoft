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
	public class PermissionService(IDataServiceFactory dataServiceFactory, ILogService logService) : IPermissionService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;

		public async Task<PermissionDto> GetPermissionAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetPermissionAsync(dataService, id);
		}

		private static async Task<PermissionDto> GetPermissionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetPermissionAsync(id);
			if (item != null)
			{
				return await CreatePermissionDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public async Task<IList<PermissionDto>> GetPermissionsAsync(DataRequest<Permission> request)
		{
			var collection = new PermissionCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<PermissionDto>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request)
		{
			var models = new List<PermissionDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetPermissionsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreatePermissionDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetPermissionsCountAsync(DataRequest<Permission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetPermissionsCountAsync(request);
		}

		public async Task<int> UpdatePermissionAsync(PermissionDto model)
		{
			long id = model.PermissionID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetPermissionAsync(model.PermissionID) : new Permission();
			if (item != null)
			{
				UpdatePermissionFromDto(item, model);
				await dataService.UpdatePermissionAsync(item);
				model.Merge(await GetPermissionAsync(dataService, item.PermissionID));
			}
			return 0;
		}

		public async Task<int> DeletePermissionAsync(PermissionDto model)
		{
			var item = new Permission { PermissionID = model.PermissionID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeletePermissionsAsync(item);
		}

		public async Task<int> DeletePermissionRangeAsync(int index, int length, DataRequest<Permission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetPermissionKeysAsync(index, length, request);
			return await dataService.DeletePermissionsAsync([.. items]);
		}

		public static async Task<PermissionDto> CreatePermissionDtoAsync(Permission source, bool includeAllFields)
		{
			var model = new PermissionDto()
			{
				PermissionID = source.PermissionID,
				Name = source.Name,
				DisplayName = source.DisplayName,
				Description = source.Description,
				IsEnabled = source.IsEnabled,
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) {}
			await Task.CompletedTask;
			return model;
		}

		private static void UpdatePermissionFromDto(Permission target, PermissionDto source)
		{
			target.Name = source.Name;
			target.DisplayName = source.DisplayName;
			target.Description = source.Description;
			target.IsEnabled = source.IsEnabled;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
