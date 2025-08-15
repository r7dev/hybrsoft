using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class UserCollection(IUserService userService, ILogService logService) : VirtualCollection<UserModel>(logService)
	{
		private DataRequest<User> _dataRequest = null;

		public IUserService UserService { get; } = userService;

		private readonly UserModel _defaultItem = UserModel.CreateEmpty();
		protected override UserModel DefaultItem => _defaultItem;

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

		protected override async Task<IList<UserModel>> FetchDataAsync(int rangeIndex, int rangeSize)
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
