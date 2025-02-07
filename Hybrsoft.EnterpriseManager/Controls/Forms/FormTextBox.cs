﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public partial class FormTextBox : TextBox, IFormControl
	{
		public event EventHandler<FormVisualState> VisualStateChanged;

		private Border _borderElement = null;

		private bool _isInitialized = false;

		public FormTextBox()
		{
			DefaultStyleKey = typeof(FormTextBox);
			RegisterPropertyChangedCallback(TextProperty, OnTextChanged);
			BeforeTextChanging += OnBeforeTextChanging;
		}

		public FormVisualState VisualState { get; private set; }

		#region DataType
		public TextDataType DataType
		{
			get { return (TextDataType)GetValue(DataTypeProperty); }
			set { SetValue(DataTypeProperty, value); }
		}

		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(nameof(DataType), typeof(TextDataType), typeof(FormTextBox), new PropertyMetadata(TextDataType.String, OnPropertyChanged));
		#endregion

		#region Format
		public string Format
		{
			get { return (string)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}

		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof(Format), typeof(string), typeof(FormTextBox), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region FormattedText
		public string FormattedText
		{
			get { return (string)GetValue(FormattedTextProperty); }
			set { SetValue(FormattedTextProperty, value); }
		}

		public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register(nameof(FormattedText), typeof(string), typeof(FormTextBox), new PropertyMetadata(null, OnPropertyChanged));
		#endregion

		#region Mode*
		public FormEditMode Mode
		{
			get { return (FormEditMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as FormTextBox;
			control.UpdateMode();
			control.UpdateVisualState();
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormTextBox), new PropertyMetadata(FormEditMode.Auto, ModeChanged));
		#endregion

		protected override void OnApplyTemplate()
		{
			_borderElement = base.GetTemplateChild("BorderElement") as Border;

			_isInitialized = true;

			UpdateMode();
			UpdateVisualState();

			base.OnApplyTemplate();
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as FormTextBox;
			control.ApplyTextFormat();
		}

		private void OnTextChanged(DependencyObject sender, DependencyProperty dp)
		{
			ApplyTextFormat();
		}

		private void ApplyTextFormat()
		{
			switch (DataType)
			{
				case TextDataType.Integer:
					_ = Int64.TryParse(Text, out Int64 n);
					FormattedText = n.ToString(Format);
					break;
				case TextDataType.Decimal:
					_ = Decimal.TryParse(Text, out decimal m);
					FormattedText = m.ToString(Format);
					break;
				case TextDataType.Double:
					_ = Double.TryParse(Text, out double d);
					FormattedText = d.ToString(Format);
					break;
				case TextDataType.String:
				default:
					FormattedText = Text;
					break;
			}
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			switch (DataType)
			{
				case TextDataType.Integer:
					_ = Int64.TryParse(Text, out Int64 n);
					Text = n == 0 ? "" : n.ToString();
					break;
				case TextDataType.Decimal:
					_ = Decimal.TryParse(Text, out decimal m);
					Text = m == 0 ? "" : m.ToString();
					break;
				case TextDataType.Double:
					_ = Double.TryParse(Text, out double d);
					Text = d == 0 ? "" : d.ToString();
					break;
				case TextDataType.String:
				default:
					break;
			}

			if (Mode == FormEditMode.Auto)
			{
				SetVisualState(FormVisualState.Focused);
			}

			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (VisualState == FormVisualState.Focused)
			{
				SetVisualState(FormVisualState.Ready);
			}

			switch (DataType)
			{
				case TextDataType.Integer:
					if (!Int64.TryParse(Text, out _))
					{
						Text = "0";
					}
					break;
				case TextDataType.Decimal:
					if (!Decimal.TryParse(Text, out _))
					{
						Text = "0";
					}
					break;
				case TextDataType.Double:
					if (!Double.TryParse(Text, out _))
					{
						Text = "0";
					}
					break;
				case TextDataType.String:
				default:
					break;
			}

			base.OnLostFocus(e);
		}

		private void OnBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
		{
			string str = args.NewText;
			if (String.IsNullOrEmpty(str) || str == "-")
			{
				return;
			}

			switch (DataType)
			{
				case TextDataType.Integer:
					args.Cancel = !Int64.TryParse(str, out _);
					break;
				case TextDataType.Decimal:
					args.Cancel = !Decimal.TryParse(str, out _);
					break;
				case TextDataType.Double:
					args.Cancel = !Double.TryParse(str, out _);
					break;
			}
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
						break;
					case FormVisualState.Ready:
						_borderElement.Opacity = 1.0;
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
	}
}
