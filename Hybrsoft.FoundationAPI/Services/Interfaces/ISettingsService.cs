using Hybrsoft.Enums;

namespace Hybrsoft.FoundationAPI.Services
{
	public interface ISettingsService
	{
		DataProviderType DataProvider { get; set; }
		string SQLServerConnectionString { get; }
	}
}
