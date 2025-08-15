﻿namespace Hybrsoft.UI.Windows.Services
{
	public interface IPasswordHasher
	{
		string HashPassword(string password);

		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
