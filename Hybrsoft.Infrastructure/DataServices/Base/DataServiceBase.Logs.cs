using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	partial class DataServiceBase
	{
		public async Task<int> CreateLogAsync(AppLog appLog)
		{
			appLog.DateTime = DateTime.UtcNow;
			_dataSource.Entry(appLog).State = EntityState.Added;
			return await _dataSource.SaveChangesAsync();
		}
	}
}
