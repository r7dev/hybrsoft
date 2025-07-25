using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls;

public sealed partial class FormCalendarDatePicker : CalendarDatePicker, IFormControl
{
	public event EventHandler<FormVisualState> VisualStateChanged;

	private Border _backgroundBorder = null;

	private bool _isInitialized = false;

	public FormCalendarDatePicker()
	{
		DefaultStyleKey = typeof(FormCalendarDatePicker);
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
		var control = d as FormCalendarDatePicker;
		control.UpdateMode();
		control.UpdateVisualState();
	}

	public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormCalendarDatePicker), new PropertyMetadata(FormEditMode.Auto, ModeChanged));
	#endregion

	protected override void OnApplyTemplate()
	{
		_backgroundBorder = base.GetTemplateChild("Background") as Border;

		_isInitialized = true;

		UpdateMode();
		UpdateVisualState();

		base.OnApplyTemplate();
	}

	protected override void OnTapped(TappedRoutedEventArgs e)
	{
		if (Mode == FormEditMode.Auto)
		{
			SetVisualState(FormVisualState.Focused);
		}

		base.OnTapped(e);
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
					_backgroundBorder.Opacity = 0.40;
					_backgroundBorder.Background = TransparentBrush;
					break;
				case FormVisualState.Ready:
					_backgroundBorder.Opacity = 1.0;
					_backgroundBorder.Background = OpaqueBrush;
					break;
				case FormVisualState.Focused:
					_backgroundBorder.Opacity = 1.0;
					_backgroundBorder.Background = OpaqueBrush;
					break;
				case FormVisualState.Disabled:
					_backgroundBorder.Opacity = 0.40;
					_backgroundBorder.Background = TransparentBrush;
					IsEnabled = false;
					Opacity = 0.75;
					break;
			}
		}
	}

	private readonly Brush TransparentBrush = new SolidColorBrush(Colors.Transparent);
	private readonly Brush OpaqueBrush = new SolidColorBrush(Colors.White);
}
