using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace Hybrsoft.EnterpriseManager.Extensions
{
	public static class ThemeExtensions
	{
		public static bool TrySetMicaBackdrop(Window window, bool useMicaAlt)
		{
			if (MicaController.IsSupported())
			{
				Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new();
				micaBackdrop.Kind = useMicaAlt ? MicaKind.BaseAlt : MicaKind.Base;
				window.SystemBackdrop = micaBackdrop;

				return true; // Succeeded.
			}

			return false; // Mica is not supported on this system.
		}

		public static bool TrySetDesktopAcrylicBackdrop(Window window)
		{
			if (DesktopAcrylicController.IsSupported())
			{
				DesktopAcrylicBackdrop DesktopAcrylicBackdrop = new();
				window.SystemBackdrop = DesktopAcrylicBackdrop;

				return true; // Succeeded.
			}

			return false; // DesktopAcrylic is not supported on this system.
		}
	}
}
