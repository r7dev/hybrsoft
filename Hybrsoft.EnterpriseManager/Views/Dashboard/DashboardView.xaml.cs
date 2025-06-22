using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Configuration;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DashboardView : Page
	{
		public DashboardView()
		{
			ViewModel = ServiceLocator.Current.GetService<DashboardViewModel>();
			this.InitializeComponent();
		}

		public DashboardViewModel ViewModel { get; }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.LoadAsync();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.Unload();
		}

		private void OnItemClick(object sender, ItemClickEventArgs e)
		{
			if (e.ClickedItem is Control control)
			{
				ViewModel.ItemSelected(control.Tag as String);
			}
		}
	}
}
