﻿using Hybrsoft.EnterpriseManager.Common;
using Hybrsoft.EnterpriseManager.Tools;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Services;
using Microsoft.Windows.Storage.Pickers;
using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class FilePickerService : IFilePickerService
	{
		public async Task<ImagePickerResult> OpenImagePickerAsync()
		{
			var windowId = WindowTracker.GetCurrentView().Window.AppWindow.Id;
			var picker = new FileOpenPicker(windowId)
			{
				ViewMode = PickerViewMode.Thumbnail,
				SuggestedStartLocation = PickerLocationId.PicturesLibrary
			};
			picker.FileTypeFilter.Add(".jpg");
			picker.FileTypeFilter.Add(".jpeg");
			picker.FileTypeFilter.Add(".png");
			picker.FileTypeFilter.Add(".bmp");
			picker.FileTypeFilter.Add(".gif");

			var result = await picker.PickSingleFileAsync();
			if (result is null)
				return null;

			var file = await StorageFile.GetFileFromPathAsync(result.Path);
			if (file is not null)
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
