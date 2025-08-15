using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces
{
	public interface ICompanyService
	{
		Task<CompanyDto> GetCompanyAsync(long id);
		Task<IList<CompanyDto>> GetCompaniesAsync(DataRequest<Company> request);
		Task<IList<CompanyDto>> GetCompaniesAsync(int skip, int take, DataRequest<Company> request);
		Task<int> GetCompaniesCountAsync(DataRequest<Company> request);

		Task<int> UpdateCompanyAsync(CompanyDto model);

		Task<int> DeleteCompanyAsync(CompanyDto model);
		Task<int> DeleteCompanyRangeAsync(int index, int length, DataRequest<Company> request);
	}
}
