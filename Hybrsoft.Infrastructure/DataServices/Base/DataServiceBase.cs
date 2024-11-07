using Hybrsoft.Infrastructure.DataContexts;
using Hybrsoft.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase : IDataService, IDisposable
	{
		private IDataSource _dataSource = null;

		public DataServiceBase(IDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public async Task<IList<Menu>> GetMenusAsync()
		{
			return await _dataSource.Menu.ToListAsync();
		}

		public IList<Menu> GetMenus()
		{
			return _dataSource.Menu.AsNoTracking().ToList();
		}

		#region Dispose
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_dataSource != null)
				{
					_dataSource.Dispose();
				}
			}
		}
		#endregion
	}
}
