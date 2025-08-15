using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IPermissionService
	{
		Task<PermissionModel> GetPermissionAsync(long id);
		Task<IList<PermissionModel>> GetPermissionsAsync(DataRequest<Permission> request);
		Task<IList<PermissionModel>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request);
		Task<int> GetPermissionsCountAsync(DataRequest<Permission> request);

		Task<int> UpdatePermissionAsync(PermissionModel model);

		Task<int> DeletePermissionAsync(PermissionModel model);
		Task<int> DeletePermissionRangeAsync(int index, int length, DataRequest<Permission> request);
	}
}
