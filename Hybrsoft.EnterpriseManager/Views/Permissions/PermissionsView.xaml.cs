using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
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
	public sealed partial class PermissionsView : Page
	{
		public PermissionsView()
		{
			ViewModel = ServiceLocator.Current.GetService<PermissionsViewModel>();
			NavigationService = ServiceLocator.Current.GetService<INavigationService>();
			this.InitializeComponent();
		}

		public PermissionsViewModel ViewModel { get; }
		public INavigationService NavigationService { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Subscribe();
			await ViewModel.LoadAsync(e.Parameter as PermissionListArgs);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		private async void OpenInNewView(object sender, RoutedEventArgs e)
		{
			await NavigationService.CreateNewViewAsync<UsersViewModel>(ViewModel.PermissionList.CreateArgs());
		}

		public int GetRowSpan(bool isMultipleSelection)
		{
			return isMultipleSelection ? 2 : 1;
		}
	}
}
