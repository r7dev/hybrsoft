using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.UI.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

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

		private void LicenseKey_TextChanged(object sender, TextChangedEventArgs e)
		{
			var tb = (TextBox)sender;
			var upper = tb.Text.ToUpperInvariant();
			var masked = ApplyMask(upper);
			if (tb.Text != masked)
			{
				tb.Text = masked;
				tb.SelectionStart = tb.Text.Length;
			}
			ViewModel.LicenseKey = tb.Text;
		}

		private static string ApplyMask(string input)
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

			var clean = new string([.. input.Where(char.IsLetterOrDigit)]);

			if (clean.Length > 25)
				clean = clean[..25];

			return string.Join("-", Enumerable.Range(0, clean.Length / 5 + (clean.Length % 5 == 0 ? 0 : 1))
													   .Select(i => clean.Substring(i * 5, Math.Min(5, clean.Length - i * 5))));
		}
	}
}
