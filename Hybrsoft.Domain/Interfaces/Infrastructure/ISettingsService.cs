namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ISettingsService
	{
		long UserID { get; set; }
		string UserName { get; set; }
		char PasswordChar { get; }
	}
}
