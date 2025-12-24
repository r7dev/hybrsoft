using Hybrsoft.EnterpriseManager.Common;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Infrastructure.Services;
using Hybrsoft.UI.Windows.Services;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class ModernTitle : UserControl
	{
		private readonly INavigationService _navigationService;
		private readonly ITitleService _titleService;
		private static string AppDisplayName => AppSettings.Current.AppName;
		private Window m_Window;

		public ModernTitle()
		{
			InitializeComponent();
			_navigationService = ServiceLocator.Current.GetService<INavigationService>();
			_titleService = ServiceLocator.Current.GetService<ITitleService>();
			_titleService.TitleChanged += OnTitleChanged;
		}

		private void OnTitleChanged(object sender, string newTitle)
		{
			string appDisplayName = AppDisplayName;
			Title = !string.IsNullOrEmpty(newTitle)
				? $"{appDisplayName} - {newTitle}"
				: appDisplayName;
		}

		public void InitializeTitleBar(Window window)
		{
			if (AppWindowTitleBar.IsCustomizationSupported())
			{
				m_Window = window;
				m_Window.ExtendsContentIntoTitleBar = true;
				if (m_Window.ExtendsContentIntoTitleBar == true)
				{
					m_Window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
				}
			}
		}

		private void TitleBar_BackRequested(TitleBar sender, object args)
		{
			if (_navigationService.CanLogoff)
			{
				var loginService = ServiceLocator.Current.GetService<ILoginService>();
				loginService.Logoff();
			}
			if (_navigationService.CanGoBack)
			{
				_navigationService.GoBack();
			}
		}

		private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
		{
			IsPaneOpen = !IsPaneOpen;
		}

		public event RoutedEventHandler LogoffClicked;
		private void OnLogoff(object sender, RoutedEventArgs e)
			=> LogoffClicked?.Invoke(this, e);

		#region Properties

		public bool IsBackButtonEnabled
		{
			get => (bool)GetValue(IsBackButtonEnabledProperty);
			set => SetValue(IsBackButtonEnabledProperty, value);
		}
		public static readonly DependencyProperty IsBackButtonEnabledProperty =
			DependencyProperty.Register(nameof(IsBackButtonEnabled), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		public bool IsBackButtonVisible
		{
			get => (bool)GetValue(IsBackButtonVisibleProperty);
			set => SetValue(IsBackButtonVisibleProperty, value);
		}
		public static readonly DependencyProperty IsBackButtonVisibleProperty =
			DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		public bool IsPaneToggleButtonVisible
		{
			get => (bool)GetValue(IsPaneToggleButtonVisibleProperty);
			set => SetValue(IsPaneToggleButtonVisibleProperty, value);
		}
		public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty =
			DependencyProperty.Register(nameof(IsPaneToggleButtonVisible), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		public bool IsPaneOpen
		{
			get => (bool)GetValue(IsPaneOpenProperty);
			set => SetValue(IsPaneOpenProperty, value);
		}
		public static readonly DependencyProperty IsPaneOpenProperty =
			DependencyProperty.Register(nameof(IsPaneOpen), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register(nameof(Title), typeof(string), typeof(ModernTitle),
				new PropertyMetadata(AppDisplayName, OnTitleChanged));
		private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ModernTitle)
			{
				var newTitle = e.NewValue as string;
				WindowTracker.SetCurrentWindowTitle(newTitle ?? AppDisplayName);
			}
		}

		public bool IsSearchBoxVisible
		{
			get { return (bool)GetValue(IsSearchBoxVisibleProperty); }
			set { SetValue(IsSearchBoxVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsSearchBoxVisibleProperty =
			DependencyProperty.Register(nameof(IsSearchBoxVisible), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		public string DisplayName
		{
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		public static readonly DependencyProperty DisplayNameProperty =
			DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ModernTitle),
				new PropertyMetadata(null));

		public object PictureSource
		{
			get { return (object)GetValue(PictureSourceProperty); }
			set { SetValue(PictureSourceProperty, value); }
		}
		public static readonly DependencyProperty PictureSourceProperty =
			DependencyProperty.Register(nameof(PictureSource), typeof(object), typeof(ModernTitle),
				new PropertyMetadata(null));

		public string AccountName
		{
			get { return (string)GetValue(AccountNameProperty); }
			set { SetValue(AccountNameProperty, value); }
		}
		public static readonly DependencyProperty AccountNameProperty =
			DependencyProperty.Register(nameof(AccountName), typeof(string), typeof(ModernTitle),
				new PropertyMetadata(null));

		public bool IsUserInfoVisible
		{
			get { return (bool)GetValue(IsUserInfoVisibleProperty); }
			set { SetValue(IsUserInfoVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsUserInfoVisibleProperty =
			DependencyProperty.Register(nameof(IsUserInfoVisible), typeof(bool), typeof(ModernTitle),
				new PropertyMetadata(false));

		#endregion
	}
}
