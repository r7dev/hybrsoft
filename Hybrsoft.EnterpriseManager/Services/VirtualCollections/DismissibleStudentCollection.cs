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
	public partial class DismissibleStudentCollection(IDismissalService dismissalService, ILogService logService) : VirtualCollection<DismissibleStudentDto>(logService)
	{
		private DataRequest<ClassroomStudent> _dataRequest = null;

		public IDismissalService DismissalService { get; } = dismissalService;

		private readonly DismissibleStudentDto _defaultItem = DismissibleStudentDto.CreateEmpty();
		protected override DismissibleStudentDto DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<ClassroomStudent> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await DismissalService.GetDismissibleStudentsCountAsync(_dataRequest);
				Ranges[0] = await DismissalService.GetDismissibleStudentsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<DismissibleStudentDto>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await DismissalService.GetDismissibleStudentsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("DismissibleStudentCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
