using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShellView : Page
	{
		public ShellView()
		{
			ViewModel = ServiceLocator.Current.GetService<ShellViewModel>();
			InitializeContext();
			this.InitializeComponent();
			ModernTitleBar.InitializeTitleBar(((App)Application.Current).CurrentView);
			InitializeNavigation();
		}

		public ShellViewModel ViewModel { get; private set; }

		private void InitializeContext()
		{
			var context = ServiceLocator.Current.GetService<IContextService>();
			Window currentView = ((App)Application.Current).CurrentView;
			var appWindow = AppWindowExtensions.GetAppWindow(currentView);
			context.Initialize(DispatcherQueue, (int)appWindow.Id.Value, currentView.IsMain());
		}

		private void InitializeNavigation()
		{
			var navigationService = ServiceLocator.Current.GetService<INavigationService>();
			navigationService.Initialize(frame);
			//var appView = ApplicationView.GetForCurrentView();
			//appView.Consolidated += OnViewConsolidated;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync(e.Parameter as ShellArgs);
			ViewModel.Subscribe();
		}
	}
}
