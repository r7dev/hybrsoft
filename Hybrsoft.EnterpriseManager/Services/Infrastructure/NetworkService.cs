using Hybrsoft.UI.Windows.Services;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.Enums;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class NetworkService(ILogService logService) : INetworkService
	{
		public ILogService LogService { get; } = logService;

		private static readonly SocketsHttpHandler _handler = new()
		{
			PooledConnectionLifetime = TimeSpan.FromMinutes(5),
		};
		private static readonly HttpClient _httpClient = new(_handler);

		public async Task<bool> IsInternetAvailableAsync()
		{
			try
			{
				var profile = NetworkInformation.GetInternetConnectionProfile();
				return profile != null && profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
			}
			catch (Exception ex)
			{
				await LogService.WriteAsync(LogType.Error, "Network", "IsInternetAvailable", ex.Message, ex.ToString());
				return false;
			}
		}

		public HttpClient GetHttpClient()
		{
			try
			{
				if (_httpClient.BaseAddress == null)
				{
					_httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
				}
				return _httpClient;
			}
			catch (Exception ex)
			{
				LogService.WriteAsync(LogType.Error, "Network", "GetHttpClient", ex.Message, ex.ToString()).Wait();
				throw;
			}
		}
	}
}
