using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views.SplashScreen
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ExtendedSplash : Window
	{
		public ExtendedSplash()
		{
			this.InitializeComponent();
			((App)Application.Current).CurrentView = this;

			OverlappedPresenter presenter = this.AppWindow.Presenter as OverlappedPresenter;
			presenter.IsAlwaysOnTop = true;
			presenter.IsMaximizable = false;
			presenter.IsMinimizable = false;
			presenter.IsResizable = false;
			this.AppWindow.IsShownInSwitchers = false;
			this.Title = "";
			presenter.SetBorderAndTitleBar(false, false);

			LoadDataAsync();
		}

		private static async void LoadDataAsync()
		{
			await Startup.ConfigureAsync();
		}
	}
}
