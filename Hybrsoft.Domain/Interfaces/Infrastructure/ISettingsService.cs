namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ISettingsService
	{
		string AppName { get; }
		string Version { get; }
		long UserID { get; set; }
		string UserName { get; set; }
		char PasswordChar { get; }
	}
}
