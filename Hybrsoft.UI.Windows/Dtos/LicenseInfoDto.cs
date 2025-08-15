using System;

namespace Hybrsoft.UI.Windows.Dtos
{
	public class LicenseInfoDto
	{
		public bool IsActivated { get; set; }
		public DateTimeOffset? StartDate { get; set; }
		public DateTimeOffset? ExpirationDate { get; set; }
		public string LicenseData { get; set; }
		public string Message { get; set; }
	}
}
