using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
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
	public sealed partial class MainShellView : Page
	{
		private INavigationService _navigationService = null;

		public MainShellView()
		{
			ViewModel = ServiceLocator.Current.GetService<MainShellViewModel>();
			InitializeContext();
			this.InitializeComponent();
			InitializeNavigation();
		}

		public MainShellViewModel ViewModel { get; }

		private void InitializeContext()
		{
			var context = ServiceLocator.Current.GetService<IContextService>();
			Window currentView = ((App)Application.Current).CurrentView;
			var appWindow = AppWindowExtensions.GetAppWindow(currentView);
			context.Initialize(DispatcherQueue, (int)appWindow.Id.Value, currentView.IsMain());
		}

		private void InitializeNavigation()
		{
			_navigationService = ServiceLocator.Current.GetService<INavigationService>();
			_navigationService.Initialize(frame);
			//frame.Navigated += OnFrameNavigated;
		}

		private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (args.SelectedItem is NavigationItem item)
			{
				ViewModel.NavigateTo(item.ViewModel);
			}
			else if (args.IsSettingsSelected)
			{
				//ViewModel.NavigateTo(typeof(SettingsViewModel));
			}
			//UpdateBackButton();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync(e.Parameter as ShellArgs);
			ViewModel.Subscribe();
		}
	}
}
