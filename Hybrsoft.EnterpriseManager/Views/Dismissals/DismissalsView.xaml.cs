using Hybrsoft.UI.Windows.Services;
using Hybrsoft.UI.Windows.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DismissalsView : Page
	{
		public DismissalsView()
		{
			ViewModel = ServiceLocator.Current.GetService<DismissalsViewModel>();
			NavigationService = ServiceLocator.Current.GetService<INavigationService>();
			InitializeComponent();
		}

		public DismissalsViewModel ViewModel { get; }
		public INavigationService NavigationService { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Subscribe();
			await ViewModel.LoadAsync(e.Parameter as DismissalListArgs);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}

		public int GetRowSpan(bool isMultipleSelection)
		{
			return isMultipleSelection ? 2 : 1;
		}
	}
}
