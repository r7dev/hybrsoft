namespace Hybrsoft.Domain.Infrastructure.Commom
{
	public class ImagePickerResult
	{
		public string FileName { get; set; }
		public string ContentType { get; set; }
		public byte[] ImageBytes { get; set; }
		public object ImageSource { get; set; }
		public byte[] ThumbnailBytes { get; set; }
		public object ThumbnailSource { get; set; }
	}
}
