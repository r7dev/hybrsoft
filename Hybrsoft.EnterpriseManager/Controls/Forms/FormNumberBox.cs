using Hybrsoft.EnterpriseManager.Tools;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class FormNumberBox : NumberBox, IFormControl
	{
		public event EventHandler<FormVisualState> VisualStateChanged;

		private TextBox _inputElement = null;

		private bool _isInitialized = false;

		public FormNumberBox()
		{
			this.DefaultStyleKey = typeof(NumberBox);
		}

		public FormVisualState VisualState { get; private set; }

		#region Mode*
		public FormEditMode Mode
		{
			get { return (FormEditMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as FormNumberBox;
			control.UpdateMode();
			control.UpdateVisualState();
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormNumberBox), new PropertyMetadata(FormEditMode.Auto, ModeChanged));
		#endregion

		protected override void OnApplyTemplate()
		{
			_inputElement = base.GetTemplateChild("InputBox") as TextBox;

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
						_inputElement.Background = TertiaryBrush;
						break;
					case FormVisualState.Ready:
						_inputElement.Background = DefaultBrush;
						break;
					case FormVisualState.Focused:
						_inputElement.Background = DefaultBrush;
						break;
					case FormVisualState.Disabled:
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
