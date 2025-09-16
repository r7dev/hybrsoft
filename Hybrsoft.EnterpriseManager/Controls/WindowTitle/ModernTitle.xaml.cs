using Hybrsoft.EnterpriseManager.Common;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.UI.Windows.Infrastructure.Services;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class ModernTitle : UserControl
	{
		private readonly ITitleService _titleService;
		private static string AppDisplayName => AppSettings.Current.AppName;
		private Window m_Window;

		public ModernTitle()
		{
			InitializeComponent();
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
				m_Window.AppWindow.Changed += AppWindow_Changed;
				m_Window.Activated += MainWindow_Activated;

				AppTitleBar.Loaded += AppTitleBar_Loaded;
				AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
				m_Window.ExtendsContentIntoTitleBar = true;
				if (m_Window.ExtendsContentIntoTitleBar == true)
				{
					m_Window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
				}
			}
		}

		private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
		{
			if (args.DidPresenterChange)
			{
				switch (sender.Presenter.Kind)
				{
					case AppWindowPresenterKind.CompactOverlay:
						// Compact overlay - hide custom title bar
						// and use the default system title bar instead.
						AppTitleBar.Visibility = Visibility.Collapsed;
						sender.TitleBar.ResetToDefault();
						break;

					case AppWindowPresenterKind.FullScreen:
						// Full screen - hide the custom title bar
						// and the default system title bar.
						AppTitleBar.Visibility = Visibility.Collapsed;
						sender.TitleBar.ExtendsContentIntoTitleBar = true;
						break;

					case AppWindowPresenterKind.Overlapped:
						// Normal - hide the system title bar
						// and use the custom title bar instead.
						AppTitleBar.Visibility = Visibility.Visible;
						sender.TitleBar.ExtendsContentIntoTitleBar = true;
						break;

					default:
						// Use the default system title bar.
						sender.TitleBar.ResetToDefault();
						break;
				}
			}
		}

		private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
		{
			if (args.WindowActivationState == WindowActivationState.Deactivated)
			{
				TitleBarTextBlock.Foreground =
					(SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
			}
			else
			{
				TitleBarTextBlock.Foreground =
					(SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
			}
		}

		private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (m_Window.ExtendsContentIntoTitleBar == true)
			{
				// Set the initial interactive regions.
				SetRegionsForCustomTitleBar();
			}
			WindowTracker.SetCurrentWindowTitle(AppDisplayName);
		}

		private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (m_Window.ExtendsContentIntoTitleBar == true)
			{
				// Update interactive regions if the size of the window changes.
				SetRegionsForCustomTitleBar();
			}
		}

		private void SetRegionsForCustomTitleBar()
		{
			// Specify the interactive regions of the title bar.
			double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

			RightPaddingColumn.Width = new GridLength(m_Window.AppWindow.TitleBar.RightInset / scaleAdjustment);
			LeftPaddingColumn.Width = new GridLength(m_Window.AppWindow.TitleBar.LeftInset / scaleAdjustment);

			GeneralTransform transform = PersonPic.TransformToVisual(null);
			Rect bounds = transform.TransformBounds(new Rect(0, 0,
													PersonPic.ActualWidth,
													PersonPic.ActualHeight));
			RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

			var rectArray = new RectInt32[] { PersonPicRect };

			InputNonClientPointerSource nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(m_Window.AppWindow.Id);
			nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
		}

		private static RectInt32 GetRect(Rect bounds, double scale)
		{
			return new RectInt32(
				_X: (int)Math.Round(bounds.X * scale),
				_Y: (int)Math.Round(bounds.Y * scale),
				_Width: (int)Math.Round(bounds.Width * scale),
				_Height: (int)Math.Round(bounds.Height * scale)
			);
		}

		public event RoutedEventHandler LogoffClicked;
		private void OnLogoff(object sender, RoutedEventArgs e)
			=> LogoffClicked?.Invoke(this, e);

		#region Properties
		public UIElement DragRegion => AppTitleBar;

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

		public string DisplayName
		{
			get { return (string)GetValue(DisplayNameProperty); }
			set { SetValue(DisplayNameProperty, value); }
		}
		public static readonly DependencyProperty DisplayNameProperty =
			DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ModernTitle), new PropertyMetadata(null));

		public object PictureSource
		{
			get { return (object)GetValue(PictureSourceProperty); }
			set { SetValue(PictureSourceProperty, value); }
		}
		public static readonly DependencyProperty PictureSourceProperty =
			DependencyProperty.Register(nameof(PictureSource), typeof(object), typeof(ModernTitle), new PropertyMetadata(null));

		public string AccountName
		{
			get { return (string)GetValue(AccountNameProperty); }
			set { SetValue(AccountNameProperty, value); }
		}
		public static readonly DependencyProperty AccountNameProperty =
			DependencyProperty.Register(nameof(AccountName), typeof(string), typeof(ModernTitle), new PropertyMetadata(null));

		public bool IsUserInfoHidden
		{
			get { return (bool)GetValue(IsUserInfoHiddenProperty); }
			set { SetValue(IsUserInfoHiddenProperty, value); }
		}
		public static readonly DependencyProperty IsUserInfoHiddenProperty =
			DependencyProperty.Register(nameof(IsUserInfoHidden), typeof(bool), typeof(ModernTitle), new PropertyMetadata(false));

		#endregion
	}
}
