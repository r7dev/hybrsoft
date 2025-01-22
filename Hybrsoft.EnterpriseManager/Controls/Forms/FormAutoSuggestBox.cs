﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.Foundation;

namespace Hybrsoft.EnterpriseManager.Controls
{
	public partial class FormAutoSuggestBox : Control, IFormControl
	{
		public event EventHandler<FormVisualState> VisualStateChanged;

		private Border _borderElement;
		private AutoSuggestBox _autoSuggestBox = null;

		private bool _isInitialized = false;

		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;
		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;
		public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

		public FormAutoSuggestBox()
		{
			DefaultStyleKey = typeof(FormAutoSuggestBox);
		}

		public FormVisualState VisualState { get; private set; }

		#region Header
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region HeaderTemplate
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region Text
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region TextMemberPath
		public string TextMemberPath
		{
			get { return (string)GetValue(TextMemberPathProperty); }
			set { SetValue(TextMemberPathProperty, value); }
		}

		public static readonly DependencyProperty TextMemberPathProperty = DependencyProperty.Register(nameof(TextMemberPath), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region DisplayText
		public string DisplayText
		{
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}

		public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region ItemsSource
		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region ItemTemplate
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region ItemContainerStyle
		public Style ItemContainerStyle
		{
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}

		public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region PlaceholderText
		public string PlaceholderText
		{
			get { return (string)GetValue(PlaceholderTextProperty); }
			set { SetValue(PlaceholderTextProperty, value); }
		}

		public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region AutoMaximizeSuggestionArea
		public bool AutoMaximizeSuggestionArea
		{
			get { return (bool)GetValue(AutoMaximizeSuggestionAreaProperty); }
			set { SetValue(AutoMaximizeSuggestionAreaProperty, value); }
		}

		public static readonly DependencyProperty AutoMaximizeSuggestionAreaProperty = DependencyProperty.Register(nameof(AutoMaximizeSuggestionArea), typeof(bool), typeof(FormAutoSuggestBox), new PropertyMetadata(null));
		#endregion

		#region Mode*
		public FormEditMode Mode
		{
			get { return (FormEditMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as FormAutoSuggestBox;
			control.UpdateMode();
			control.UpdateVisualState();
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormAutoSuggestBox), new PropertyMetadata(FormEditMode.Auto, ModeChanged));
		#endregion

		protected override void OnApplyTemplate()
		{
			_borderElement = base.GetTemplateChild("BorderElement") as Border;
			_autoSuggestBox = base.GetTemplateChild("AutoSuggestBox") as AutoSuggestBox;

			_autoSuggestBox.TextChanged += (s, a) => TextChanged?.Invoke(s, a);
			_autoSuggestBox.SuggestionChosen += (s, a) => SuggestionChosen?.Invoke(s, a);
			_autoSuggestBox.QuerySubmitted += (s, a) => QuerySubmitted?.Invoke(s, a);

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
			_autoSuggestBox.Text = DisplayText;

			if (Mode == FormEditMode.Auto)
			{
				SetVisualState(FormVisualState.Focused);
			}

			VisualStateManager.GoToState(this, "Focused", false);
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (VisualState == FormVisualState.Focused)
			{
				SetVisualState(FormVisualState.Ready);
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
						break;
					case FormVisualState.Ready:
						_borderElement.Opacity = 1.00;
						break;
					case FormVisualState.Focused:
						_autoSuggestBox.Opacity = 1.0;
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
