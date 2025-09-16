using Hybrsoft.EnterpriseManager.Common;
using Hybrsoft.EnterpriseManager.Configuration;
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
			ModernTitleBar.InitializeTitleBar(WindowTracker.GetCurrentView().Window);
			InitializeNavigation();
		}

		public ShellViewModel ViewModel { get; private set; }

		private void InitializeContext()
		{
			var context = ServiceLocator.Current.GetService<IContextService>();
			var currentView = WindowTracker.GetCurrentView();
			context.Initialize(DispatcherQueue, (int)currentView.ID, currentView.IsMain);
		}

		private void InitializeNavigation()
		{
			var navigationService = ServiceLocator.Current.GetService<INavigationService>();
			navigationService.Initialize(frame);
			var currentView = WindowTracker.GetCurrentView()?.Window;
			if (currentView != null)
			{
				currentView.Closed += OnWindowClosed;
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync(e.Parameter as ShellArgs);
			ViewModel.Subscribe();
		}

		private void OnWindowClosed(object sender, WindowEventArgs e)
		{
			ViewModel?.Unsubscribe();
			ViewModel = null;
			Bindings.StopTracking();

			var currentWindow = sender as Window;
			if (currentWindow != null)
			{
				currentWindow.Closed -= OnWindowClosed;
			}

			ServiceLocator.DisposeCurrent();
		}

	}
}
