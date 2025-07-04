﻿using Hybrsoft.Domain.Dtos;
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

		public async Task<StudentRelativeDto> GetStudentRelativeAsync(long id)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetStudentRelativeAsync(dataService, id);
		}
		private static async Task<StudentRelativeDto> GetStudentRelativeAsync(IDataService dataService, long id)
		{
			var item = await dataService.GetStudentRelativeAsync(id);
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

		public async Task<IList<long>> GetAddedRelativeKeysInStudentAsync(long parentID)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetAddedRelativeKeysInStudentAsync(parentID);
		}

		public async Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetStudentRelativesCountAsync(request);
		}

		public async Task<int> UpdateStudentRelativeAsync(StudentRelativeDto model)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var item = model.StudentRelativeID > 0
				? await dataService.GetStudentRelativeAsync(model.StudentRelativeID)
				: new StudentRelative() { Relative = new Relative() };
			if (item != null)
			{
				UpdateStudentRelativeFromDto(item, model);
				await dataService.UpdateStudentRelativeAsync(item);
				model.Merge(await GetStudentRelativeAsync(dataService, item.StudentRelativeID));
			}
			return 0;
		}

		public async Task<int> DeleteStudentRelativeAsync(StudentRelativeDto model)
		{
			var item = new StudentRelative() { StudentRelativeID = model.StudentRelativeID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteStudentRelativesAsync(item);
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
				StudentRelativeID = source.StudentRelativeID,
				StudentID = source.StudentID,
				RelativeID = source.RelativeID,
				Relative = await RelativeService.CreateRelativeDtoAsync(source.Relative, includeAllFields),
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields) { }
			return model;
		}

		private static void UpdateStudentRelativeFromDto(StudentRelative target, StudentRelativeDto source)
		{
			target.StudentID = source.StudentID;
			target.RelativeID = source.RelativeID;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
			target.SearchTerms = source.Relative?.FullName;
		}
	}
}
