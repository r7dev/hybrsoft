using System.Net.Http;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.Interfaces.Infrastructure
{
	public interface INetworkService
	{
		Task<bool> IsInternetAvailableAsync();
		HttpClient GetHttpClient();
	}
}
