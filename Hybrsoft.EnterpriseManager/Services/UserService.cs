using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class UserService(IDataServiceFactory dataServiceFactory, ILogService logService, IPasswordHasher passwordHasher) : IUserService
	{
		public IDataServiceFactory DataServiceFactory { get; } = dataServiceFactory;
		public ILogService LogService { get; } = logService;
		public IPasswordHasher PasswordHasher { get; } = passwordHasher;

		public async Task<UserDto> GetUserAsync(long id, bool includePassword = false)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetUserAsync(dataService, id, includePassword);
		}

		private static async Task<UserDto> GetUserAsync(IDataService dataService, long id, bool includePassword)
		{
			var item = await dataService.GetUserAsync(id);
			if (item != null)
			{
				return await CreateUserDtoAsync(item, includeAllFields: true, includePassword);
			}
			return null;
		}

		public async Task<UserDto> GetUserByEmailAsync(string email, bool includePassword)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await GetUserByEmailAsync(dataService, email, includePassword);
		}

		private static async Task<UserDto> GetUserByEmailAsync(IDataService dataService, string email, bool includePassword)
		{
			var item = await dataService.GetUserByEmailAsync(email);
			if (item != null)
			{
				return await CreateUserDtoAsync(item, includeAllFields: true, includePassword);
			}
			return null;
		}

		public async Task<IList<UserDto>> GetUsersAsync(DataRequest<User> request)
		{
			var collection = new UserCollection(this, LogService);
			await collection.LoadAsync(request);
			return collection;
		}

		public async Task<IList<UserDto>> GetUsersAsync(int skip, int take, DataRequest<User> request)
		{
			var models = new List<UserDto>();
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetUsersAsync(skip, take, request);
			foreach (var item in items)
			{
				models.Add(await CreateUserDtoAsync(item, includeAllFields: false));
			}
			return models;
		}

		public async Task<int> GetUsersCountAsync(DataRequest<User> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.GetUsersCountAsync(request);
		}

		public async Task<int> UpdateUserAsync(UserDto model)
		{
			long id = model.UserID;
			using var dataService = DataServiceFactory.CreateDataService();
			var item = id > 0 ? await dataService.GetUserAsync(model.UserID) : new User();
			if (item != null)
			{
				model.PasswordLength = model.Password?.Length ?? 0;
				model.Password = model.PasswordChanged
					? PasswordHasher.HashPassword(model.Password)
					: item.Password;
				UpdateUserFromDto(item, model);
				await dataService.UpdateUserAsync(item);
				model.Merge(await GetUserAsync(dataService, item.UserID, false));
			}
			return 0;
		}

		public async Task<int> DeleteUserAsync(UserDto model)
		{
			var item = new User { UserID = model.UserID };
			using var dataService = DataServiceFactory.CreateDataService();
			return await dataService.DeleteUsersAsync(item);
		}

		public async Task<int> DeleteUserRangeAsync(int index, int length, DataRequest<User> request)
		{
			using var dataService = DataServiceFactory.CreateDataService();
			var items = await dataService.GetUserKeysAsync(index, length, request);
			return await dataService.DeleteUsersAsync([.. items]);
		}

		public static async Task<UserDto> CreateUserDtoAsync(User source, bool includeAllFields, bool includePassword = false)
		{
			var model = new UserDto()
			{
				UserID = source.UserID,
				FirstName = source.FirstName,
				LastName = source.LastName,
				Email = source.Email,
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				model.MiddleName = source.MiddleName;
				model.Password = includePassword
					? source.Password
					: new string(AppSettings.Current.PasswordChar, source.PasswordLength);
				model.PasswordLength = source.PasswordLength;
			}
			await Task.CompletedTask;
			return model;
		}

		private static void UpdateUserFromDto(User target, UserDto source)
		{
			target.FirstName = source.FirstName;
			target.MiddleName = source.MiddleName;
			target.LastName = source.LastName;
			target.Email = source.Email;
			target.Password = source.Password;
			target.PasswordLength = source.PasswordLength;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
