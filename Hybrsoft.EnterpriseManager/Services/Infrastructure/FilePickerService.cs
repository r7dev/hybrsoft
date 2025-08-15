using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.EnterpriseManager.Tools;
using Microsoft.UI.Xaml;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class FilePickerService : IFilePickerService
	{
		public async Task<ImagePickerResult> OpenImagePickerAsync()
		{
			var picker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.PicturesLibrary
			};
			picker.FileTypeFilter.Add(".jpg");
			picker.FileTypeFilter.Add(".jpeg");
			picker.FileTypeFilter.Add(".png");
			picker.FileTypeFilter.Add(".bmp");
			picker.FileTypeFilter.Add(".gif");

			var window = ((App)Application.Current).CurrentView;
			window.InitializeWithObject(picker);

			var file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				var bytes = await GetImageBytesAsync(file);
				var bytesResized = await GetResizedImageBytesAsync(file, 200, 200);

				return new ImagePickerResult
				{
					FileName = file.Name,
					ContentType = file.ContentType,
					ImageBytes = bytes,
					ImageSource = await BitmapTools.LoadBitmapAsync(bytes),
					ThumbnailBytes = bytesResized,
					ThumbnailSource = await BitmapTools.LoadBitmapAsync(bytesResized)
				};
			}
			return null;
		}

		static private async Task<byte[]> GetImageBytesAsync(StorageFile file)
		{
			using var randomStream = await file.OpenReadAsync();
			using var stream = randomStream.AsStream();
			byte[] buffer = new byte[randomStream.Size];
			await stream.ReadExactlyAsync(buffer);
			return buffer;
		}

		static private async Task<byte[]> GetResizedImageBytesAsync(StorageFile file, int newWidth, int newHeight)
		{
			using var randomStream = await file.OpenReadAsync();
			using var stream = randomStream.AsStream();
			byte[] buffer = new byte[randomStream.Size];
			await stream.ReadExactlyAsync(buffer);
			using var inputStream = new MemoryStream(buffer);
			using var original = SKBitmap.Decode(inputStream);
			using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);
			using var image = SKImage.FromBitmap(resized);
			using var outputStream = new MemoryStream();
			image.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(outputStream);
			return outputStream.ToArray();
		}
	}
}
