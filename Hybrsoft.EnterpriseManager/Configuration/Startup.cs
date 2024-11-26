using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Domain.ViewModels;
using Hybrsoft.EnterpriseManager.Services;
using Hybrsoft.EnterpriseManager.Views;
using Hybrsoft.Infrastructure.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Hybrsoft.EnterpriseManager.Configuration
{
	static public class Startup
	{
		static private readonly ServiceCollection _serviceCollection = new ServiceCollection();

		static public async Task ConfigureAsync()
		{
			ServiceLocator.Configure(_serviceCollection);

			ConfigureNavigation();

			var logService = ServiceLocator.Current.GetService<ILogService>();
			await logService.WriteAsync(LogType.Information, "Startup", "Configuration", "Application Start", $"Application started.");
		}

		private static void ConfigureNavigation()
		{
			NavigationService.Register<ShellViewModel, ShellView>();
			NavigationService.Register<MainShellViewModel, MainShellView>();

			NavigationService.Register<DashboardViewModel, DashboardView>();

			NavigationService.Register<UsersViewModel, UsersView>();
			NavigationService.Register<UserDetailsViewModel, UserView>();
		}
	}
}
