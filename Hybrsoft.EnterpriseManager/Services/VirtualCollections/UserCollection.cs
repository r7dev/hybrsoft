﻿using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class UserCollection(IUserService userService, ILogService logService) : VirtualCollection<UserDto>(logService)
	{
		private DataRequest<User> _dataRequest = null;

		public IUserService UserService { get; } = userService;

		private readonly UserDto _defaultItem = UserDto.CreateEmpty();
		protected override UserDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<User> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await UserService.GetUsersCountAsync(_dataRequest);
				Ranges[0] = await UserService.GetUsersAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<UserDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await UserService.GetUsersAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("UserCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
