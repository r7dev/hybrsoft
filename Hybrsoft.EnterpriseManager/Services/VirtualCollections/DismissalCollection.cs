using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Common.VirtualCollection;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.VirtualCollections
{
	public partial class DismissalCollection(IDismissalService dismissalService, ILogService logService) : VirtualCollection<DismissalDto>(logService)
	{
		private DataRequest<Dismissal> _dataRequest = null;

		public IDismissalService DismissalService { get; } = dismissalService;

		private readonly DismissalDto _defaultItem = DismissalDto.CreateEmpty();
		protected override DismissalDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Dismissal> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await DismissalService.GetDismissalsCountAsync(_dataRequest);
				Ranges[0] = await DismissalService.GetDismissalsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<DismissalDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await DismissalService.GetDismissalsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("DismissalCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
