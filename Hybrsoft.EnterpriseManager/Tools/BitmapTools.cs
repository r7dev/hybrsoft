using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Hybrsoft.EnterpriseManager.Tools
{
	static public class BitmapTools
	{
		static public async Task<BitmapImage> LoadBitmapAsync(IRandomAccessStreamReference randomStreamReference)
		{
			var bitmap = new BitmapImage();
			try
			{
				using var stream = await randomStreamReference.OpenReadAsync();
				await bitmap.SetSourceAsync(stream);
			}
			catch { }
			return bitmap;
		}
	}
}
