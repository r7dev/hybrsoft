using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Views;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			((App)Application.Current).MainWindow = this;
			((App)Application.Current).CurrentView = this;
			this.Title = AppInfo.Current.DisplayInfo.DisplayName;
			ThemeExtensions.TrySetMicaBackdrop(this, true);
			rootFrame.Navigate(typeof(MainShellView));
		}
	}
}
