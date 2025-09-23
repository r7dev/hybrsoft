using Hybrsoft.Enums;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface ISettingsService
	{
		string AppName { get; }
		string Version { get; }
		string UserName { get; set; }
		char PasswordChar { get; }
		EnvironmentType Environment { get; set; }

		Task<string> GetLicensedToAsync();

		Task<T> ReadSettingAsync<T>(string key);
		Task SaveSettingAsync<T>(string key, T value);
	}
}
