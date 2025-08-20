namespace Hybrsoft.Domain.Services
{
	public interface ISecurityService
	{
		string HashPassword(string password);
		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
