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
	public partial class StudentCollection(IStudentService studentService, ILogService logService) : VirtualCollection<StudentModel>(logService)
	{
		private DataRequest<Student> _dataRequest = null;

		public IStudentService StudentService { get; } = studentService;

		private readonly StudentModel _defaultItem = StudentModel.CreateEmpty();
		protected override StudentModel DefaultItem => _defaultItem;

		public async Task LoadAsync(DataRequest<Student> dataRequest)
		{
			try
			{
				_dataRequest = dataRequest;
				Count = await StudentService.GetStudentsCountAsync(_dataRequest);
				Ranges[0] = await StudentService.GetStudentsAsync(0, RangeSize, _dataRequest);
			}
			catch (Exception)
			{
				Count = 0;
				throw;
			}
		}

		protected override async Task<IList<StudentModel>> FetchDataAsync(int rangeIndex, int rangeSize)
		{
			try
			{
				return await StudentService.GetStudentsAsync(rangeIndex * rangeSize, rangeSize, _dataRequest);
			}
			catch (Exception ex)
			{
				LogException("StudentCollection", "Fetch", ex);
			}
			return null;
		}
	}
}
