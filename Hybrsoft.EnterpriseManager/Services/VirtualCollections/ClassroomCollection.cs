using Hybrsoft.UI.Windows.Models;
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
	public partial class ClassroomCollection(IClassroomService classroomService, ILogService logService) : VirtualCollection<ClassroomModel>(logService)
	{
		private DataRequest<Classroom> _dataRequest = null;

		public IClassroomService ClassroomService { get; } = classroomService;

		private readonly ClassroomModel _defaultItem = ClassroomModel.CreateEmpty();
		protected override ClassroomModel DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Classroom> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await ClassroomService.GetClassroomsCountAsync(_dataRequest);
				Ranges[0] = await ClassroomService.GetClassroomsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<ClassroomModel>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await ClassroomService.GetClassroomsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("ClassroomCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
