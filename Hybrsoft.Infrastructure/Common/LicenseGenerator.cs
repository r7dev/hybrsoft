using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hybrsoft.Infrastructure.Common
{
	static class LicenseGenerator
	{
		public static string GenerateLicenseKey(long subscriptionID, int durationDays, DateTimeOffset createdOn, long targetID)
		{
			string rawKey = $"{subscriptionID}-{durationDays}-{createdOn:yyyyMMddHHmmsszzz}-{targetID}";
			byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
			// Convert to Base64 and remove unwanted characters
			string base64 = Convert.ToBase64String(hash);
			// Use only alphanumeric characters (A-Z, 0-9)
			string alphanumeric = string.Concat(base64.Where(c => char.IsLetterOrDigit(c)));
			// Ensure 25 alphanumeric characters
			if (alphanumeric.Length < 25)
			{
				// Complete with random characters, if necessary
				string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				Random random = new();
				while (alphanumeric.Length < 25)
				{
					alphanumeric += chars[random.Next(chars.Length)];
				}
			}
			else
			{
				alphanumeric = alphanumeric[..25];
			}
			// Format as XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
			return $"{alphanumeric[..5]}-{alphanumeric[5..10]}-{alphanumeric[10..15]}-{alphanumeric[15..20]}-{alphanumeric[20..25]}".ToUpper();
		}
	}
}
