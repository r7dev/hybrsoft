using Hybrsoft.UI.Windows.Services;
using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class ContextService : IContextService
	{
		static private int _mainViewID = -1;

		private DispatcherQueue _dispatcher = null;

		public int MainViewID => _mainViewID;

		public int ContextID { get; private set; }

		public bool IsMainView { get; private set; }

		public void Initialize(object dispatcher, int contextID, bool isMainView)
		{
			_dispatcher = dispatcher as DispatcherQueue;
			ContextID = contextID;
			IsMainView = isMainView;
			if (IsMainView)
			{
				_mainViewID = ContextID;
			}
		}

		public async Task RunAsync(Action action)
		{
			var taskCompletionSource = new TaskCompletionSource<object>();
			_dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () => {
				try
				{
					action();
					taskCompletionSource.SetResult(null);
				}
				catch (Exception ex)
				{
					taskCompletionSource.SetException(ex);
				}
			});
			await taskCompletionSource.Task;
		}
	}
}
