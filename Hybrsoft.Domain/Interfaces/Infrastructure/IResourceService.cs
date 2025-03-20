using Hybrsoft.Domain.Dtos;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.Interfaces.Infrastructure
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
