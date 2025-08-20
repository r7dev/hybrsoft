namespace Hybrsoft.FoundationAPI.Services
{
	public interface IAuthService
	{
		Task<string> AuthenticateAsync(string username, string password, bool isEncrypted);
	}
}
