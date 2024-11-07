using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace Hybrsoft.EnterpriseManager.Extensions
{
	public static class AppWindowExtensions
	{
		public static AppWindow GetAppWindow(this Window window)
		{
			IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
			return GetAppWindowFromWindowHandle(windowHandle);
		}

		private static AppWindow GetAppWindowFromWindowHandle(IntPtr windowHandle)
		{
			WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
			return AppWindow.GetFromWindowId(windowId);
		}

		public static void SetDefaultIcon(AppWindow appWindow)
		{
			appWindow.SetIcon("Assets/default.ico");
		}

		public static bool IsMain(this Window window)
		{
			WindowId mainWindowId = ((App)Application.Current).MainWindow.AppWindow.Id;
			return window.AppWindow.Id == mainWindowId;
		}
	}
}
