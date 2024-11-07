using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Views.SplashScreen;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		private Window m_window;
		private ExtendedSplash splash_Screen;

		public Window MainWindow { get; private set; }
		public Window CurrentView { get; set; }

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

			this.UnhandledException += OnUnhandledException;
		}

		/// <summary>
		/// Invoked when the application is launched.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			splash_Screen = new ExtendedSplash();
			await splash_Screen.SetWindowPositionToCenter();
			splash_Screen.Activate();
			await Task.Delay(3000);

			m_window = new MainWindow();
			AppWindowExtensions.SetDefaultIcon(m_window.AppWindow);
			MainWindow = m_window;
			m_window.Activate();

			await Task.Delay(1000);
			splash_Screen.Close();
		}

		private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			var logService = ServiceLocator.Current.GetService<ILogService>();
			logService.WriteAsync(LogType.Error, "App", "UnhandledException", e.Message, e.Exception.ToString());
		}
	}
}
