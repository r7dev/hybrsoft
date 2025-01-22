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
	public partial class RoleCollection(IRoleService roleService, ILogService logService) : VirtualCollection<RoleDto>(logService)
	{
		private DataRequest<Role> _dataRequest = null;

		public IRoleService RoleService { get; } = roleService;

		private readonly RoleDto _defaultItem = RoleDto.CreateEmpty();
		protected override RoleDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Role> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await RoleService.GetRolesCountAsync(_dataRequest);
				Ranges[0] = await RoleService.GetRolesAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<RoleDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await RoleService.GetRolesAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("RoleCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
