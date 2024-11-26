using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;

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

		public static async Task SetWindowPositionToCenterAsync(this Window window)
		{
			var displayList = await DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());
			if (!displayList.Any())
			{
				return;
			}

			var monitorInfo = await DisplayMonitor.FromInterfaceIdAsync(displayList[0].Id);
			var Height = monitorInfo.NativeResolutionInRawPixels.Height;
			var Width = monitorInfo.NativeResolutionInRawPixels.Width;

			AppWindow appWindow = GetAppWindow(window);
			var CenterPosition = appWindow.Position;
			CenterPosition.X = (Width - appWindow.Size.Width) / 2;
			CenterPosition.Y = (Height - appWindow.Size.Height) / 2;
			appWindow.Move(CenterPosition);
		}
	}
}
