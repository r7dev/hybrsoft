using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ICompanyUserService
	{
		Task<CompanyUserDto> GetCompanyUserAsync(long id);
		Task<IList<CompanyUserDto>> GetCompanyUsersAsync(DataRequest<CompanyUser> request);
		Task<IList<CompanyUserDto>> GetCompanyUsersAsync(int skip, int take, DataRequest<CompanyUser> request);
		Task<IList<long>> GetAddedUserKeysInCompanyAsync(long parentID);
		Task<int> GetCompanyUsersCountAsync(DataRequest<CompanyUser> request);

		Task<int> UpdateCompanyUserAsync(CompanyUserDto model);

		Task<int> DeleteCompanyUserAsync(CompanyUserDto model);
		Task<int> DeleteCompanyUserRangeAsync(int index, int length, DataRequest<CompanyUser> request);
	}
}
