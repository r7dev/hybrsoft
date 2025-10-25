using Hybrsoft.Enums;
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

		string GetString<TViewModel>(ResourceFiles resource, string sufix);
		string GetString(ResourceFiles resource, string key);

		Task InitializeAsync();
		Task SetLanguageAsync(LanguageModel language);

		ResourceContext GetContext();
	}
}
