using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class LostAndFoundCollection(ILostAndFoundService lostAndFoundService, ILogService logService) : VirtualCollection<LostAndFoundModel>(logService)
	{
		private DataRequest<LostAndFound> _dataRequest = null;

		public ILostAndFoundService LostAndFoundService { get; } = lostAndFoundService;

		private readonly LostAndFoundModel _defaultItem = LostAndFoundModel.CreateEmpty();
		protected override LostAndFoundModel DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<LostAndFound> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await LostAndFoundService.GetLostAndFoundCountAsync(_dataRequest);
				Ranges[0] = await LostAndFoundService.GetLostAndFoundAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<LostAndFoundModel>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await LostAndFoundService.GetLostAndFoundAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("LostAndFoundCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
