using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class StudentRelativeService(IDataServiceFactory dataServiceFactory) : IStudentRelativeService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<StudentRelativeDto> GetStudentRelativeAsync(long studentRelativeId)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetStudentRelativeAsync(dataService, studentRelativeId);
		}
		static private async Task<StudentRelativeDto> GetStudentRelativeAsync(IDataService dataService, long studentRelativeId)
		{
			var item = await dataService.GetStudentRelativeAsync(studentRelativeId);
			if (item != null)
			{
				return await CreateStudentRelativeDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<StudentRelativeDto>> GetStudentRelativesAsync(DataRequest<StudentRelative> request)
		{
			// StudentRelatives are not virtualized
			return GetStudentRelativesAsync(0, 100, request);
		}

		public async Task<IList<StudentRelativeDto>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request)
		{
			var models = new List<StudentRelativeDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentRelativesAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateStudentRelativeDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedRelativeKeysAsync(long studentId)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedRelativeKeysAsync(studentId);
		}

		public async Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetStudentRelativesCountAsync(request);
		}

		public async Task<int> UpdateStudentRelativeAsync(StudentRelativeDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var studentRelative = model.StudentRelativeID > 0
				? await dataService.GetStudentRelativeAsync(model.StudentRelativeID)
				: new StudentRelative() { Relative = new Relative() };
			if (studentRelative != null)
			{
				UpdateStudentRelativeFromDto(studentRelative, model);
				await dataService.UpdateStudentRelativeAsync(studentRelative);
				model.Merge(await GetStudentRelativeAsync(dataService, studentRelative.StudentRelativeId));
			}
			return 0;
		}

		public async Task<int> DeleteStudentRelativeAsync(StudentRelativeDto model)
		{
			var studentRelative = new StudentRelative() { StudentRelativeId = model.StudentRelativeID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteStudentRelativesAsync(studentRelative);
		}

		public async Task<int> DeleteStudentRelativeRangeAsync(int index, int length, DataRequest<StudentRelative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetStudentRelativesAsync(index, length, request);
			return await dataService.DeleteStudentRelativesAsync([.. items]);
		}

		public static async Task<StudentRelativeDto> CreateStudentRelativeDtoAsync(StudentRelative source, bool includeAllFields)
		{
			var model = new StudentRelativeDto()
			{
				StudentRelativeID = source.StudentRelativeId,
				StudentID = source.StudentId,
				RelativeID = source.RelativeId,
				Relative = await RelativeService.CreateRelativeDtoAsync(source.Relative, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
			}
			return model;
		}

		private static void UpdateStudentRelativeFromDto(StudentRelative target, StudentRelativeDto source)
		{
			target.StudentId = source.StudentID;
			target.RelativeId = source.RelativeID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Relative?.FullName;
		}
	}
}
