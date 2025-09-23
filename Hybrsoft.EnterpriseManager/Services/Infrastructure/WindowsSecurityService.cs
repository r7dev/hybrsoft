using Hybrsoft.UI.Windows.Services;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class WindowsSecurityService : IWindowsSecurityService
	{
		public string EncryptData(object value)
		{
			byte[] encryptedData = ProtectedData.Protect(
				Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)),
				null,
				DataProtectionScope.CurrentUser);

			return Convert.ToBase64String(encryptedData);
		}

		public TResult DecryptData<TResult>(string encryptedData)
		{
			if (string.IsNullOrEmpty(encryptedData))
			{
				return default;
			}
			// Decrypt the data using ProtectedData
			// Note: Ensure that the encryptedData is in Base64 format before calling this method
			byte[] decryptedData = ProtectedData.Unprotect(
				Convert.FromBase64String(encryptedData),
				null,
				DataProtectionScope.CurrentUser);
			return JsonSerializer.Deserialize<TResult>(Encoding.UTF8.GetString(decryptedData));
		}
	}
}
