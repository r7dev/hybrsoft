using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SubscriptionView : Page
{
	public SubscriptionView()
	{
		ViewModel = ServiceLocator.Current.GetService<SubscriptionDetailsViewModel>();
		NavigationService = ServiceLocator.Current.GetService<INavigationService>();
		InitializeComponent();
	}

	public SubscriptionDetailsViewModel ViewModel { get; }
	public INavigationService NavigationService { get; }

	protected override async void OnNavigatedTo(NavigationEventArgs e)
	{
		ViewModel.Subscribe();
		await ViewModel.LoadAsync(e.Parameter as SubscriptionDetailsArgs);

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
