using Hybrsoft.EnterpriseManager.Tools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public partial class FormPasswordBox : Control, IFormControl
	{
		public event EventHandler<FormVisualState> VisualStateChanged;

		private Border _borderElement;
		private PasswordBox _passwordBox = null;

		private bool _isInitialized = false;

		public event ContextMenuOpeningEventHandler ContextMenuOpening;
		public event RoutedEventHandler PasswordChanged;
		public event TypedEventHandler<PasswordBox, PasswordBoxPasswordChangingEventArgs> PasswordChanging;
		public event TextControlPasteEventHandler Paste;

		public FormPasswordBox()
		{
			DefaultStyleKey = typeof(FormPasswordBox);
		}

		public FormVisualState VisualState { get; private set; }

		#region Header
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(FormPasswordBox), new PropertyMetadata(null));
		#endregion

		#region HeaderTemplate
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(FormPasswordBox), new PropertyMetadata(null));
		#endregion

		#region Password
		public string Password
		{
			get { return (string)GetValue(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}

		public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(FormPasswordBox), new PropertyMetadata(null));
		#endregion

		#region PlaceholderText
		public string PlaceholderText
		{
			get { return (string)GetValue(PlaceholderTextProperty); }
			set { SetValue(PlaceholderTextProperty, value); }
		}

		public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(FormPasswordBox), new PropertyMetadata(null));
		#endregion

		#region Mode*
		public FormEditMode Mode
		{
			get { return (FormEditMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as FormPasswordBox;
			control.UpdateMode();
			control.UpdateVisualState();
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormPasswordBox), new PropertyMetadata(FormEditMode.Auto, ModeChanged));
		#endregion

		protected override void OnApplyTemplate()
		{
			_borderElement = base.GetTemplateChild("BorderElement") as Border;
			_passwordBox = base.GetTemplateChild("PasswordBox") as PasswordBox;

			_passwordBox.ContextMenuOpening += (s, e) => ContextMenuOpening?.Invoke(s, e);
			_passwordBox.PasswordChanged += (s, e) => PasswordChanged?.Invoke(s, e);
			_passwordBox.PasswordChanging += (s, e) => PasswordChanging?.Invoke(s, e);
			_passwordBox.Paste += (s, e) => Paste?.Invoke(s, e);

			_isInitialized = true;

			UpdateMode();
			UpdateVisualState();

			base.OnApplyTemplate();
		}

		protected override void OnPointerEntered(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "PointerOver", false);
			base.OnPointerEntered(e);
		}

		protected override void OnPointerExited(PointerRoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Normal", false);
			base.OnPointerExited(e);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			if (Mode == FormEditMode.Auto)
			{
				SetVisualState(FormVisualState.Focused);
			}

			VisualStateManager.GoToState(this, "Focused", false);
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			Password = _passwordBox.Password;
			if (Mode == FormEditMode.Auto)
			{
				SetVisualState(FormVisualState.Idle);
			}
			VisualStateManager.GoToState(this, "Normal", false);
			base.OnLostFocus(e);
		}

		private void UpdateMode()
		{
			switch (Mode)
			{
				case FormEditMode.Auto:
					VisualState = FormVisualState.Idle;
					break;
				case FormEditMode.ReadWrite:
					VisualState = FormVisualState.Ready;
					break;
				case FormEditMode.ReadOnly:
					VisualState = FormVisualState.Disabled;
					break;
			}
		}

		public void SetVisualState(FormVisualState visualState)
		{
			if (Mode == FormEditMode.ReadOnly)
			{
				visualState = FormVisualState.Disabled;
			}

			if (visualState != VisualState)
			{
				VisualState = visualState;
				UpdateVisualState();
				VisualStateChanged?.Invoke(this, visualState);
			}
		}

		private void UpdateVisualState()
		{
			if (_isInitialized)
			{
				switch (VisualState)
				{
					case FormVisualState.Idle:
						_borderElement.Opacity = 0.40;
						_passwordBox.Background = TertiaryBrush;
						break;
					case FormVisualState.Ready:
						_borderElement.Opacity = 1.00;
						_passwordBox.Background = DefaultBrush;
						break;
					case FormVisualState.Focused:
						_borderElement.Opacity = 1.0;
						break;
					case FormVisualState.Disabled:
						_borderElement.Opacity = 0.40;
						IsEnabled = false;
						Opacity = 0.75;
						break;
				}
			}
		}
		
		private readonly Brush TertiaryBrush = FillColorTools.GetBrush(FillColors.ControlFillColorTertiaryBrush);
		private readonly Brush DefaultBrush = FillColorTools.GetBrush(FillColors.ControlFillColorDefaultBrush);
	}
}
