using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class ContextService : IContextService
	{
		static private int _mainViewID = -1;

		private CoreDispatcher _dispatcher = null;

		public int MainViewID => _mainViewID;

		public int ContextID { get; private set; }

		public bool IsMainView { get; private set; }

		public void Initialize(object dispatcher, int contextID, bool isMainView)
		{
			_dispatcher = dispatcher as CoreDispatcher;
			ContextID = contextID;
			IsMainView = isMainView;
			if (IsMainView)
			{
				_mainViewID = ContextID;
			}
		}

		public async Task RunAsync(Action action)
		{
			await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
		}
	}
}
