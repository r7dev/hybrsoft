using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class ClassroomStudentService(IDataServiceFactory dataServiceFactory) : IClassroomStudentService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;

		public async Task<ClassroomStudentDto> GetClassroomStudentAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetClassroomStudentAsync(dataService, id);
		}
		private static async Task<ClassroomStudentDto> GetClassroomStudentAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetClassroomStudentAsync(id);
			if (item != null)
			{
				return await CreateClassroomStudentDtoAsync(item, includeAllFields: true);
			}
			return null;
		}

		public Task<IList<ClassroomStudentDto>> GetClassroomStudentsAsync(DataRequest<ClassroomStudent> request)
		{
			// ClassroomStudents are not virtualized
			return GetClassroomStudentsAsync(0, 100, request);
		}

		public async Task<IList<ClassroomStudentDto>> GetClassroomStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request)
		{
			var models = new List<ClassroomStudentDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomStudentsAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateClassroomStudentDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<IList<long>> GetAddedStudentKeysInClassroomAsync(long parentID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedStudentKeysInClassroomAsync(parentID);
		}

		public async Task<int> GetClassroomStudentsCountAsync(DataRequest<ClassroomStudent> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetClassroomStudentsCountAsync(request);
		}

		public async Task<int> UpdateClassroomStudentAsync(ClassroomStudentDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var classroomStudent = model.ClassroomStudentID > 0
				? await dataService.GetClassroomStudentAsync(model.ClassroomStudentID)
				: new ClassroomStudent() { Student = new Student() };
			if (classroomStudent != null)
			{
				UpdateClassroomStudentFromDto(classroomStudent, model);
				await dataService.UpdateClassroomStudentAsync(classroomStudent);
				model.Merge(await GetClassroomStudentAsync(dataService, classroomStudent.ClassroomStudentID));
			}
			return 0;
		}

		public async Task<int> DeleteClassroomStudentAsync(ClassroomStudentDto model)
		{
			var classroomStudent = new ClassroomStudent() { ClassroomStudentID = model.ClassroomStudentID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteClassroomStudentsAsync(classroomStudent);
		}

		public async Task<int> DeleteClassroomStudentRangeAsync(int index, int length, DataRequest<ClassroomStudent> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetClassroomStudentsAsync(index, length, request);
			return await dataService.DeleteClassroomStudentsAsync([.. items]);
		}

		public static async Task<ClassroomStudentDto> CreateClassroomStudentDtoAsync(ClassroomStudent source, bool includeAllFields)
		{
			var model = new ClassroomStudentDto()
			{
				ClassroomStudentID = source.ClassroomStudentID,
				ClassroomID = source.ClassroomID,
				StudentID = source.StudentID,
				Student = await StudentService.CreateStudentDtoAsync(source.Student, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) { }
			return model;
		}

		private static void UpdateClassroomStudentFromDto(ClassroomStudent target, ClassroomStudentDto source)
		{
			target.ClassroomID = source.ClassroomID;
			target.StudentID = source.StudentID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Student?.FullName;
			target.SearchTermsDismissibleStudent = source.Classroom?.Name;
		}
	}
}
