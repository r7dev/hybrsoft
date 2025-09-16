using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Hybrsoft.EnterpriseManager.Common
{
	public static class WindowTracker
	{
		private static readonly Dictionary<ulong, Window> _windows = [];
		private static CurrentWindow _current;
		private static ulong _mainId;

		public static ulong Register(Window window)
		{
			ulong id = window.AppWindow.Id.Value;
			_windows[id] = window;

			bool isMain = window is MainWindow;
			_mainId = isMain ? id : ulong.MinValue;

			var current = new CurrentWindow
			{
				ID = id,
				Window = window,
				IsMain = isMain
			};
			_current = current;

			window.Activated += (_, e) =>
			{
				if (e.WindowActivationState == WindowActivationState.CodeActivated ||
					e.WindowActivationState == WindowActivationState.PointerActivated)
				{
					_current = current;
				}
			};

			window.Closed += (_, _) =>
			{
				_windows.Remove(id);
			};

			return id;
		}

		public static ulong? MainID => _mainId;

		public static CurrentWindow GetCurrentView() => _current;

		public static void SetCurrentWindowTitle(string title)
		{
			if (_current?.Window != null && _current.Window.Title != title)
			{
				_current.Window.Title = title;
			}
		}
	}

	public class CurrentWindow
	{
		public ulong ID { get; set; }
		public Window Window { get; set; }
		public bool IsMain { get; set; }
	}
}
