namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface IPasswordHasher
	{
		string HashPassword(string password);

		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
