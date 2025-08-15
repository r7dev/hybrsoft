using Hybrsoft.UI.Windows.Dtos;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces.Infrastructure
{
	public interface IResourceService
	{
		List<LanguageDto> LanguageOptions { get; }
		LanguageDto GetCurrentLanguageItem();

		string GetString(string resourceFile, string key);

		Task InitializeAsync();
		Task SetLanguageAsync(LanguageDto language);

		ResourceContext GetContext();
	}
}
