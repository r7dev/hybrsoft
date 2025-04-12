using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class RoundButton : Button
	{
		public RoundButton()
		{
			this.DefaultStyleKey = typeof(RoundButton);
		}

		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set
			{
				SetValue(CornerRadiusProperty, new CornerRadius(value));
				SetValue(RadiusProperty, value);

			}
		}
		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof(Radius), typeof(double), typeof(RoundButton), new PropertyMetadata(0));

		#region CornerRadius
		public new CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public static new readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RoundButton), new PropertyMetadata(new CornerRadius(0)));
		#endregion
	}
}
