using Hybrsoft.Domain.Dtos;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces
{
	public interface IStudentRelativeService
	{
		Task<StudentRelativeDto> GetStudentRelativeAsync(long id);
		Task<IList<StudentRelativeDto>> GetStudentRelativesAsync(DataRequest<StudentRelative> request);
		Task<IList<StudentRelativeDto>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request);
		Task<IList<long>> GetAddedRelativeKeysInStudentAsync(long parentID);
		Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request);

		Task<int> UpdateStudentRelativeAsync(StudentRelativeDto model);

		Task<int> DeleteStudentRelativeAsync(StudentRelativeDto model);
		Task<int> DeleteStudentRelativeRangeAsync(int index, int length, DataRequest<StudentRelative> request);
	}
}
