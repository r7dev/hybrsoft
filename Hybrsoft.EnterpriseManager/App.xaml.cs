using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Views.SplashScreen;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application, IDisposable
	{
		private Window m_window;
		private ExtendedSplash splash_Screen;
		private static Mutex _mutex;
		private bool disposedValue;

		public Window MainWindow { get; set; }
		public Window CurrentView { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

			_mutex = new Mutex(true, "SingleInstanceAppMutex", out bool createdNew);
			if (!createdNew)
			{
				BringExistingInstanceToFront();
				_mutex.Dispose();
				Environment.Exit(0);
			}

			this.UnhandledException += OnUnhandledException;
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
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_RESTORE = 9;

		/// <summary>
		/// Invoked when the application is launched.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			AppActivationArguments activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
			await ActivateAsync(activatedArgs.Data as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs);
		}

		private async Task ActivateAsync(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
		{
			var windowSize = new Windows.Graphics.SizeInt32 { Width = 1280, Height = 840 };
			splash_Screen = new ExtendedSplash();
			splash_Screen.AppWindow.Resize(windowSize);
			await splash_Screen.SetWindowPositionToCenterAsync();
			splash_Screen.Activate();
			await Task.Delay(2000);

			m_window = new MainWindow(args);
			AppWindowExtensions.SetDefaultIcon(m_window.AppWindow);
			m_window.AppWindow.Resize(windowSize);
			await m_window.SetWindowPositionToCenterAsync();
			m_window.Activate();

			await Task.Delay(200);
			splash_Screen.Close();
		}

		private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			var logService = ServiceLocator.Current.GetService<ILogService>();
			logService.WriteAsync(LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
		}

		#region Dispose
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_mutex?.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
