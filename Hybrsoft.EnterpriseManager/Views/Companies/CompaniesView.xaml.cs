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
	public sealed partial class CompaniesView : Page
	{
		public CompaniesView()
		{
			ViewModel = ServiceLocator.Current.GetService<CompaniesViewModel>();
			NavigationService = ServiceLocator.Current.GetService<INavigationService>();
			InitializeComponent();
		}

		public CompaniesViewModel ViewModel { get; }
		public INavigationService NavigationService { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Subscribe();
			await ViewModel.LoadAsync(e.Parameter as CompanyListArgs);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		private async void OpenInNewView(object sender, RoutedEventArgs e)
		{
			await NavigationService.CreateNewViewAsync<CompaniesViewModel>(ViewModel.CompanyList.CreateArgs());
		}

		public int GetRowSpan(bool isMultipleSelection)
		{
			return isMultipleSelection ? 2 : 1;
		}
	}
}
