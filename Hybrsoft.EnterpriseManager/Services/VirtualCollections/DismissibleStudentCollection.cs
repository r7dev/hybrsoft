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
	public partial class DismissibleStudentCollection(IDismissalService dismissalService, ILogService logService) : VirtualCollection<DismissibleStudentModel>(logService)
	{
		private DataRequest<ClassroomStudent> _dataRequest = null;

		public IDismissalService DismissalService { get; } = dismissalService;

		private readonly DismissibleStudentModel _defaultItem = DismissibleStudentModel.CreateEmpty();
		protected override DismissibleStudentModel DefaultItem => _defaultItem;

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

		protected override async Task<IList<DismissibleStudentModel>> FetchDataAsync(int rangeIndex, int rangeSize)
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
