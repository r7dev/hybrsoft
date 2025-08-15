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
	public partial class RelativeCollection(IRelativeService relativeService, ILogService logService) : VirtualCollection<RelativeDto>(logService)
	{
		private DataRequest<Relative> _dataRequest = null;

		public IRelativeService RelativeService { get; } = relativeService;

		private readonly RelativeDto _defaultItem = RelativeDto.CreateEmpty();
		protected override RelativeDto DefaultItem => _defaultItem;

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

		protected override async Task<IList<RelativeDto>> FetchDataAsync(int rangeIndex, int rangeSize)
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
