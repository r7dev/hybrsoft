using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class DialogService : IDialogService
	{
		public async Task ShowAsync(string title, Exception ex, string ok = "Ok")
		{
			await ShowAsync(title, ex.Message, ok);
		}

		public async Task ShowAsync(Result result, string ok = "Ok")
		{
			await ShowAsync(result.Message, result.Description, ok);
		}

		public async Task<bool> ShowAsync(string title, string content, string ok = "Ok", string cancel = null)
		{
			var dialog = new ContentDialog
			{
				Title = title,
				Content = content,
				PrimaryButtonText = ok,
				DefaultButton = ContentDialogButton.Primary,
				Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
			};
			if (cancel != null)
			{
				dialog.CloseButtonText = cancel;
			}
			Window currentView = ((App)Application.Current).CurrentView;
			dialog.XamlRoot = currentView.Content.XamlRoot;
			var result = await dialog.ShowAsync();
			return result == ContentDialogResult.Primary;
		}
	}
}
