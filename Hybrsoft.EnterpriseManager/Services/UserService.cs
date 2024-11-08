using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Services.DataServiceFactory;
using Hybrsoft.EnterpriseManager.Services.VirtualCollections;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.DataServices;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services
{
	public class UserService : IUserService
	{
		public UserService(IDataServiceFactory dataServiceFactory, ILogService logService)
		{
			DataServiceFactory = dataServiceFactory;
			LogService = logService;
		}

		public IDataServiceFactory DataServiceFactory { get; }
		public ILogService LogService { get; }

		public async Task<UserDto> GetUserAsync(Guid id)
		{
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				return await GetUserAsync(dataService, id);
			}
		}

		static private async Task<UserDto> GetUserAsync(IDataService dataService, Guid id)
		{
			var item = await dataService.GetUserAsync(id);
			if (item != null)
			{
				return CreateUserDtoAsync(item, includeAllFields: true);
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
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				var items = await dataService.GetUsersAsync(skip, take, request);
				foreach (var item in items)
				{
					models.Add(CreateUserDtoAsync(item, includeAllFields: false));
				}
				return models;
			}
		}

		public async Task<int> GetUsersCountAsync(DataRequest<User> request)
		{
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				return await dataService.GetUsersCountAsync(request);
			}
		}

		public async Task<int> UpdateUserAsync(UserDto model)
		{
			Guid id = model.UserID;
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				var user = id != Guid.Empty ? await dataService.GetUserAsync(model.UserID) : new User();
				if (user != null)
				{
					UpdateUserFromDto(user, model);
					await dataService.UpdateUserAsync(user);
					model.Merge(await GetUserAsync(dataService, user.UserId));
				}
				return 0;
			}
		}

		public async Task<int> DeleteUserAsync(UserDto model)
		{
			var customer = new User { UserId = model.UserID };
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				return await dataService.DeleteUsersAsync(customer);
			}
		}

		public async Task<int> DeleteUserRangeAsync(int index, int length, DataRequest<User> request)
		{
			using (var dataService = DataServiceFactory.CreateDataService())
			{
				var items = await dataService.GetUserKeysAsync(index, length, request);
				return await dataService.DeleteUsersAsync(items.ToArray());
			}
		}

		static public UserDto CreateUserDtoAsync(User source, bool includeAllFields)
		{
			var model = new UserDto()
			{
				UserID = source.UserId,
				FirstName = source.FirstName,
				LastName = source.LastName,
				Email = source.Email,
				CreatedOn = source.CreatedOn,
				LastModifiedOn = source.LastModifiedOn
			};
			if (includeAllFields)
			{
				//model.BirthDate = source.BirthDate;
			}
			return model;
		}

		private void UpdateUserFromDto(User target, UserDto source)
		{
			target.FirstName = source.FirstName;
			target.LastName = source.LastName;
			target.Email = source.Email;
			target.CreatedOn = source.CreatedOn;
			target.LastModifiedOn = source.LastModifiedOn;
		}
	}
}
