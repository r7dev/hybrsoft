using Hybrsoft.UI.Windows.Models;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ICompanyService
	{
		Task<CompanyModel> GetCompanyAsync(long id);
		Task<IList<CompanyModel>> GetCompaniesAsync(DataRequest<Company> request);
		Task<IList<CompanyModel>> GetCompaniesAsync(int skip, int take, DataRequest<Company> request);
		Task<int> GetCompaniesCountAsync(DataRequest<Company> request);

		Task<int> UpdateCompanyAsync(CompanyModel model);

		Task<int> DeleteCompanyAsync(CompanyModel model);
		Task<int> DeleteCompanyRangeAsync(int index, int length, DataRequest<Company> request);
	}
}
