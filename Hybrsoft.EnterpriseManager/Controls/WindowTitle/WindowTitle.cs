using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Infrastructure.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public partial class WindowTitle : Control
	{
		private readonly ITitleService _titleService;

		public WindowTitle()
		{
			_titleService = ServiceLocator.Current.GetService<ITitleService>();
			this.Loaded += WindowTitle_Loaded;
		}

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
			var control = d as WindowTitle;
			control._titleService.Title = $"{control.Prefix}{control.Title}";
		}

		private void WindowTitle_Loaded(object sender, RoutedEventArgs e)
		{
			_titleService.Title = Title;
		}
	}
}
