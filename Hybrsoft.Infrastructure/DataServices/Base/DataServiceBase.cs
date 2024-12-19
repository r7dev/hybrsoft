using Hybrsoft.Infrastructure.DataContexts;
using System;

namespace Hybrsoft.Infrastructure.DataServices.Base
{
	abstract public partial class DataServiceBase(IDataSource dataSource) : IDataService, IDisposable
	{
		private readonly IDataSource _dataSource = dataSource;

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
				_dataSource?.Dispose();
			}
		}
		#endregion
	}
}
