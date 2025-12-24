using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LicenseActivationView : Page
	{
		public LicenseActivationView()
		{
			ViewModel = ServiceLocator.Current.GetService<LicenseActivationViewModel>();
			InitializeComponent();
		}

		public LicenseActivationViewModel ViewModel { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync(e.Parameter as ShellArgs);
			ViewModel.Subscribe();
			InitializeNavigation();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		private void InitializeNavigation()
		{
			var navigationService = ServiceLocator.Current.GetService<INavigationService>();
			navigationService.Initialize(Frame);
			ViewModel.IsBackButtonEnabled = navigationService.CanGoBack;
		}
	}
}
