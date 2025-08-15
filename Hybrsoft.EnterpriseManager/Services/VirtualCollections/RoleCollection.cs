using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class RoleCollection(IRoleService roleService, ILogService logService) : VirtualCollection<RoleModel>(logService)
	{
		private DataRequest<Role> _dataRequest = null;

		public IRoleService RoleService { get; } = roleService;

		private readonly RoleModel _defaultItem = RoleModel.CreateEmpty();
		protected override RoleModel DefaultItem => _defaultItem;

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

		protected override async Task<IList<RoleModel>> FetchDataAsync(int rangeIndex, int rangeSize)
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
