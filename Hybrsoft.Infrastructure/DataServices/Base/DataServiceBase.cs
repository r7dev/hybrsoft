using Hybrsoft.Infrastructure.DataContexts;
using System;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase : IDataService, IDisposable
	{
		private IDataSource _dataSource = null;

		public DataServiceBase(IDataSource dataSource)
		{
			_dataSource = dataSource;
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
