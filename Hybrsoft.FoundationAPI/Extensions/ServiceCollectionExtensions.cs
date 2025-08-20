using Hybrsoft.Domain.Services;
using Hybrsoft.FoundationAPI.Services;
using Hybrsoft.FoundationAPI.Services.DataServiceFactory;

namespace Hybrsoft.FoundationAPI.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection Configure(this IServiceCollection services)
		{
			services.AddSingleton<ISettingsService, SettingsService>();

			services.AddScoped<IDataServiceFactory, DataServiceFactory>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ISecurityService, SecurityService>();
			services.AddScoped<ISubscriptionService, SubscriptionService>();

			return services;
		}
	}
}
