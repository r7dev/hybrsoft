using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class IconLabel : Control
	{
		private FontIcon _icon = null;
		private TextBlock _text = null;

		public IconLabel()
		{
			this.DefaultStyleKey = typeof(IconLabel);
		}

		#region Orientation
		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as IconLabel;
			control.UpdateControl();
		}

		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IconLabel), new PropertyMetadata(Orientation.Horizontal, OrientationChanged));
		#endregion

		#region Glyph
		public string Glyph
		{
			get { return (string)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}

		public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(IconLabel), new PropertyMetadata(null));
		#endregion

		#region GlyphSize
		public double GlyphSize
		{
			get { return (double)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}

		public static readonly DependencyProperty GlyphSizeProperty = DependencyProperty.Register("GlyphSize", typeof(double), typeof(IconLabel), new PropertyMetadata(0.0));
		#endregion

		#region Label
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		private static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as IconLabel;
			control.UpdateControl();
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(IconLabel), new PropertyMetadata(null, LabelChanged));
		#endregion

		private void UpdateControl()
		{
			if (_text != null)
			{
				switch (Orientation)
				{
					case Orientation.Horizontal:
						_text.Margin = String.IsNullOrEmpty(Label) ? new Thickness(0) : new Thickness(12, 0, 0, 0);
						break;
					case Orientation.Vertical:
						_text.Margin = new Thickness(0);
						break;
				}
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_icon = base.GetTemplateChild("icon") as FontIcon;
			_text = base.GetTemplateChild("text") as TextBlock;

			UpdateControl();
		}
	}
}
