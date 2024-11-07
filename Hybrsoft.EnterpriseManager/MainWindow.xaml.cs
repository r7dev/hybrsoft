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
			this.Title = AppInfo.Current.DisplayInfo.DisplayName;
			TrySetMicaBackdrop(true);
			//TrySetDesktopAcrylicBackdrop();
			rootFrame.Navigate(typeof(MainShellView));
		}

		bool TrySetMicaBackdrop(bool useMicaAlt)
		{
			if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
			{
				Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
				micaBackdrop.Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
				this.SystemBackdrop = micaBackdrop;

				return true; // Succeeded.
			}

			return false; // Mica is not supported on this system.
		}

		bool TrySetDesktopAcrylicBackdrop()
		{
			if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
			{
				Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop DesktopAcrylicBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
				this.SystemBackdrop = DesktopAcrylicBackdrop;

				return true; // Succeeded.
			}

			return false; // DesktopAcrylic is not supported on this system.
		}
	}
}
