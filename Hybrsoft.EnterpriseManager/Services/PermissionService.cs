using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
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

		static private async Task<PermissionDto> GetPermissionAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetPermissionAsync(id);
			if (item != null)
			{
				return CreatePermissionDtoAsync(item, includeAllFields: true);
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
				models.Add(CreatePermissionDtoAsync(item, includeAllFields: false));
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
			long id = model.PermissionId;
			using var dataService = DataServiceFactory.CreateDataService();
			var permission = id > 0 ? await dataService.GetPermissionAsync(model.PermissionId) : new Permission();
			if (permission != null)
			{
				UpdatePermissionFromDto(permission, model);
				await dataService.UpdatePermissionAsync(permission);
				model.Merge(await GetPermissionAsync(dataService, permission.PermissionId));
			}
			return 0;
		}

		public async Task<int> DeletePermissionAsync(PermissionDto model)
		{
			var permission = new Permission { PermissionId = model.PermissionId };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeletePermissionsAsync(permission);
		}

		public async Task<int> DeletePermissionRangeAsync(int index, int length, DataRequest<Permission> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetPermissionKeysAsync(index, length, request);
			return await dataService.DeletePermissionsAsync([.. items]);
		}

		static public PermissionDto CreatePermissionDtoAsync(Permission source, bool includeAllFields)
		{
			var model = new PermissionDto()
			{
				PermissionId = source.PermissionId,
				Name = source.Name,
				DisplayName = source.DisplayName,
				Description = source.Description,
				IsEnabled = source.IsEnabled,
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) {}
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
