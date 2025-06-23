using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	public class AppMutex
	{
		private static readonly string MutexName = "Hybrsoft.EnterpriseManager.SingleInstanceMutex";
		private static Mutex _mutex;

		/// <summary>
		/// Initializes the application mutex to ensure that only a single instance of the application runs.
		/// </summary>
		public static void Initialize()
		{
			if (!IsSingleInstance())
			{
				// If another instance is running, bring it to the front.
				BringExistingInstanceToFront();
				_mutex.Dispose();
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// Checks if the application is a single instance.
		/// </summary>
		private static bool IsSingleInstance()
		{
			_mutex = new Mutex(true, MutexName, out bool createdNew);
			return createdNew;
		}

		/// <summary>
		/// Find window existing and bring instance to front.
		/// </summary>
		private static void BringExistingInstanceToFront()
		{
			string windowTitle = AppInfo.Current.DisplayInfo.DisplayName;
			IntPtr hWnd = FindWindow(null, windowTitle);
			if (hWnd != IntPtr.Zero)
			{
				ShowWindow(hWnd, SW_RESTORE);
				SetForegroundWindow(hWnd);
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		/// <summary>
		/// Finds a window by its class name and window name.
		/// </summary>
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		/// <summary>
		/// Sets the specified window to the foreground.
		/// </summary>
		private static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		/// <summary>
		/// Shows the specified window.
		/// </summary>
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_RESTORE = 9;
	}
}
