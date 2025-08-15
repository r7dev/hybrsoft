using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.UI.Windows.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PermissionView : Page
	{
		public PermissionView()
		{
			ViewModel = ServiceLocator.Current.GetService<PermissionDetailsViewModel>();
			NavigationService = ServiceLocator.Current.GetService<INavigationService>();
			this.InitializeComponent();
		}

		public PermissionDetailsViewModel ViewModel { get; }
		public INavigationService NavigationService { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Subscribe();
			await ViewModel.LoadAsync(e.Parameter as PermissionDetailsArgs);

			if (ViewModel.IsEditMode)
			{
				await Task.Delay(100);
				details.SetFocus();
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.Unload();
			ViewModel.Unsubscribe();
		}
	}
}
