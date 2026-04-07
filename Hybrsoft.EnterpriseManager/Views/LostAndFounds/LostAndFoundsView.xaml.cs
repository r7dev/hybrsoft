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
	public sealed partial class LostAndFoundsView : Page
	{
		public LostAndFoundsView()
		{
			ViewModel = ServiceLocator.Current.GetService<LostAndFoundsViewModel>();
			NavigationService = ServiceLocator.Current.GetService<INavigationService>();
			InitializeComponent();
		}

		public LostAndFoundsViewModel ViewModel { get; }
		public INavigationService NavigationService { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Subscribe();
			await ViewModel.LoadAsync(e.Parameter as LostAndFoundListArgs);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		private async void OpenInNewView(object sender, RoutedEventArgs e)
		{
			await NavigationService.CreateNewViewAsync<LostAndFoundsViewModel>(ViewModel.LostAndFoundList.CreateArgs());
		}

		public int GetRowSpan(bool isMultipleSelection)
		{
			return isMultipleSelection ? 2 : 1;
		}
	}
}
