using Hybrsoft.Enums;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
{
	public interface ISettingsService
	{
		string AppName { get; }
		string Version { get; }
		long UserID { get; set; }
		string UserName { get; set; }
		char PasswordChar { get; }
		EnvironmentType Environment { get; set; }

		Task<T> ReadSettingAsync<T>(string key);
		Task SaveSettingAsync<T>(string key, T value);
	}
}
