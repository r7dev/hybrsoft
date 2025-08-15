using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ICompanyUserService
	{
		Task<CompanyUserModel> GetCompanyUserAsync(long id);
		Task<IList<CompanyUserModel>> GetCompanyUsersAsync(DataRequest<CompanyUser> request);
		Task<IList<CompanyUserModel>> GetCompanyUsersAsync(int skip, int take, DataRequest<CompanyUser> request);
		Task<IList<long>> GetAddedUserKeysInCompanyAsync(long parentID);
		Task<int> GetCompanyUsersCountAsync(DataRequest<CompanyUser> request);

		Task<int> UpdateCompanyUserAsync(CompanyUserModel model);

		Task<int> DeleteCompanyUserAsync(CompanyUserModel model);
		Task<int> DeleteCompanyUserRangeAsync(int index, int length, DataRequest<CompanyUser> request);
	}
}
