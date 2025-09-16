using Hybrsoft.EnterpriseManager.Common;
using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using WinRT.Interop;

namespace Hybrsoft.EnterpriseManager.Extensions
{
	public static class AppWindowExtensions
	{
		public static AppWindow GetAppWindow(this Window window)
		{
			IntPtr windowHandle = WindowNative.GetWindowHandle(window);
			return GetAppWindowFromWindowHandle(windowHandle);
		}

		private static AppWindow GetAppWindowFromWindowHandle(IntPtr windowHandle)
		{
			WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
			return AppWindow.GetFromWindowId(windowId);
		}

		public static void SetDefaultIcon(this Window window)
		{
			window.AppWindow.SetIcon("Assets/default.ico");
		}

		public static async Task SetWindowPositionToCenterAsync(this Window window)
		{
			var displayList = await DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());
			if (displayList.Count == 0)
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

		public static void SetDefaultWindowSize(this Window window)
		{
			AppWindow appWindow = GetAppWindow(window);
			appWindow.Resize(AppSettings.Current.WindowSizeDefault);
		}

		public static void InitializeWithObject(this Window window, object obj)
		{
			IntPtr windowHandle = WindowNative.GetWindowHandle(window);
			InitializeWithWindow.Initialize(obj, windowHandle);
		}
	}
}
