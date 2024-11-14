using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;

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

			AppWindow appWindow = AppWindowExtensions.GetAppWindow(this);
			OverlappedPresenter presenter = appWindow.Presenter as OverlappedPresenter;
			presenter.IsAlwaysOnTop = true;
			presenter.IsMaximizable = false;
			presenter.IsMinimizable = false;
			presenter.IsResizable = false;
			appWindow.IsShownInSwitchers = false;
			this.Title = "";
			appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 620, Height = 300 });
			presenter.SetBorderAndTitleBar(false, false);

			LoadDataAsync();
		}

		public async Task SetWindowPositionToCenter()
		{
			AppWindow appWindow = AppWindowExtensions.GetAppWindow(this);

			var displayList = await DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());

			if (!displayList.Any())
			{
				return;
			}

			var monitorInfo = await DisplayMonitor.FromInterfaceIdAsync(displayList[0].Id);

			var Height = monitorInfo.NativeResolutionInRawPixels.Height;
			var Width = monitorInfo.NativeResolutionInRawPixels.Width;
			var CenterPosition = appWindow.Position;
			CenterPosition.X = (Width - appWindow.Size.Width) / 2;
			CenterPosition.Y = (Height - appWindow.Size.Height) / 2;
			appWindow.Move(CenterPosition);
		}

		private async void LoadDataAsync()
		{
			await Startup.ConfigureAsync();
		}
	}
}
