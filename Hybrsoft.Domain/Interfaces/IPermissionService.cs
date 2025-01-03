using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IPermissionService
	{
		Task<PermissionDto> GetPermissionAsync(long id);
		Task<IList<PermissionDto>> GetPermissionsAsync(DataRequest<Permission> request);
		Task<IList<PermissionDto>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request);
		Task<int> GetPermissionsCountAsync(DataRequest<Permission> request);

		Task<int> UpdatePermissionAsync(PermissionDto model);

		Task<int> DeletePermissionAsync(PermissionDto model);
		Task<int> DeletePermissionRangeAsync(int index, int length, DataRequest<Permission> request);
	}
}
