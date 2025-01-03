using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class PermissionCollection(IPermissionService permissionService, ILogService logService) : VirtualCollection<PermissionDto>(logService)
	{
		private DataRequest<Permission> _dataRequest = null;

		public IPermissionService PermissionService { get; } = permissionService;

		private readonly PermissionDto _defaultItem = PermissionDto.CreateEmpty();
		protected override PermissionDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Permission> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await PermissionService.GetPermissionsCountAsync(_dataRequest);
				Ranges[0] = await PermissionService.GetPermissionsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<PermissionDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await PermissionService.GetPermissionsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("PermissionCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
