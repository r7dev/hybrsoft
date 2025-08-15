using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure.LogService
{
	public partial class LogCollection(ILogService logService) : VirtualCollection<AppLogDto>(logService)
	{
		private DataRequest<AppLog> _dataRequest = null;

		private readonly AppLogDto _defaultItem = AppLogDto.CreateEmpty();
		protected override AppLogDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<AppLog> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await LogService.GetLogsCountAsync(_dataRequest);
				Ranges[0] = await FetchDataAsync(0, RangeSize);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<AppLogDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await LogService.GetLogsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("LogCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
