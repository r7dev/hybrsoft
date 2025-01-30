using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Hybrsoft.EnterpriseManager.Tools
{
	public static class FillColorTools
	{
		public static Brush GetBrush(FillColors key)
		{
			return (Brush)Application.Current.Resources[key.ToString()];
		}
	}

	public enum FillColors
	{
		ControlFillColorTertiaryBrush,
		ControlFillColorDefaultBrush
	}
}
