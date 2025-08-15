using Hybrsoft.UI.Windows.Services;
using System;
using System.Security.Cryptography;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public sealed class PasswordHasher : IPasswordHasher
	{
		private const char Separator = '-';
		private const int SaltSize = 16;
		private const int HashSize = 32;
		private const int Iterations = 100000;

		private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

		public string HashPassword(string password)
		{
			byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
			byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

			return $"{Convert.ToHexString(hash)}{Separator}{Convert.ToHexString(salt)}";
		}

		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			if (string.IsNullOrEmpty(hashedPassword) || !hashedPassword.Contains(Separator))
			{
				return false;
			}
			var parts = hashedPassword.Split(Separator);
			if (parts.Length != 2)
			{
				return false;
			}
			var hash = Convert.FromHexString(parts[0]);
			var salt = Convert.FromHexString(parts[1]);

			var testHash = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, Iterations, Algorithm, hash.Length);

			return CryptographicOperations.FixedTimeEquals(hash, testHash);
		}
	}
}
