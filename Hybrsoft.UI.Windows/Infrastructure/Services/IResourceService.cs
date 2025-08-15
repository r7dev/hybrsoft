using Hybrsoft.UI.Windows.Models;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Services
{
	public interface IResourceService
	{
		List<LanguageModel> LanguageOptions { get; }
		LanguageModel GetCurrentLanguageItem();

		string GetString(string resourceFile, string key);

		Task InitializeAsync();
		Task SetLanguageAsync(LanguageModel language);

		ResourceContext GetContext();
	}
}
