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
	public partial class RelativeCollection(IRelativeService relativeService, ILogService logService) : VirtualCollection<RelativeModel>(logService)
	{
		private DataRequest<Relative> _dataRequest = null;

		public IRelativeService RelativeService { get; } = relativeService;

		private readonly RelativeModel _defaultItem = RelativeModel.CreateEmpty();
		protected override RelativeModel DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Relative> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await RelativeService.GetRelativesCountAsync(_dataRequest);
				Ranges[0] = await RelativeService.GetRelativesAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<RelativeModel>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await RelativeService.GetRelativesAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("RelativeCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
