using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Services;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

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
			frame.Navigated += OnFrameNavigated;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync(e.Parameter as ShellArgs);
			ViewModel.Subscribe();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (args.SelectedItem is NavigationItemDto item)
			{
				if (item.ViewModel != null)
				{
					ViewModel.NavigateTo(item.ViewModel);
				}
			}
			else if (args.IsSettingsSelected)
			{
				ViewModel.NavigateTo(typeof(SettingsViewModel));
			}
			UpdateBackButton();
		}

		private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
		{
			if (_navigationService.CanGoBack)
			{
				_navigationService.GoBack();
			}
		}

		private void OnFrameNavigated(object sender, NavigationEventArgs e)
		{
			var targetType = NavigationService.GetViewModel(e.SourcePageType);
			ViewModel.SelectedItem = targetType.Name switch
			{
				"SettingsViewModel" => NavigationView.SettingsItem,
				_ => ViewModel.NavigationItems.Where(r => r.ViewModel == targetType).FirstOrDefault(),
			};
			UpdateBackButton();
		}

		private void UpdateBackButton()
		{
			NavigationView.IsBackEnabled = _navigationService.CanGoBack;
		}

		private async void OnLogoff(object sender, RoutedEventArgs e)
		{
			string title = ViewModel.ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmLogoff");
			string content = ViewModel.ResourceService.GetString(nameof(ResourceFiles.Questions), "AreYouSureYouWantToLogoff");
			string cancel = ViewModel.ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			var dialogService = ServiceLocator.Current.GetService<IDialogService>();
			if (await dialogService.ShowAsync(title, content, "Ok", cancel))
			{
				var loginService = ServiceLocator.Current.GetService<ILoginService>();
				loginService.Logoff();
				if (Frame.CanGoBack)
				{
					Frame.GoBack();
				}
			}
		}
	}
}
