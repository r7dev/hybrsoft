using Hybrsoft.EnterpriseManager.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public class WindowTitle : Control
	{
		public string Prefix
		{
			get { return (string)GetValue(PrefixProperty); }
			set { SetValue(PrefixProperty, value); }
		}
		public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(nameof(Prefix), typeof(string), typeof(WindowTitle), new PropertyMetadata(null, TitleChanged));

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(WindowTitle), new PropertyMetadata(null, TitleChanged));

		private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = ((App)Application.Current).CurrentView;
			var appWindow = AppWindowExtensions.GetAppWindow(window);
			var control = d as WindowTitle;
			appWindow.Title = $"{control.Prefix} {control.Title}".Trim();
		}
	}
}
