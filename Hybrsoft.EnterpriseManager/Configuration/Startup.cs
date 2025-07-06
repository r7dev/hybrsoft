using Hybrsoft.Domain.Interfaces;
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
		static private readonly ServiceCollection _serviceCollection = new();

		static public async Task ConfigureAsync()
		{
			ServiceLocator.Configure(_serviceCollection);

			ConfigureNavigation();

			var logService = ServiceLocator.Current.GetService<ILogService>();
			await logService.WriteAsync(LogType.Information, "Startup", "Configuration", "Application Start", $"Application started.");

			var resourceService = ServiceLocator.Current.GetService<IResourceService>();
			await resourceService.InitializeAsync();

			await ConfigureLookupTables();
		}

		private static void ConfigureNavigation()
		{
			NavigationService.Register<ShellViewModel, ShellView>();
			NavigationService.Register<MainShellViewModel, MainShellView>();

			NavigationService.Register<DashboardViewModel, DashboardView>();

			NavigationService.Register<RelativesViewModel, RelativesView>();
			NavigationService.Register<RelativeDetailsViewModel, RelativeView>();

			NavigationService.Register<StudentsViewModel, StudentsView>();
			NavigationService.Register<StudentDetailsViewModel, StudentView>();

			NavigationService.Register<StudentRelativeDetailsViewModel, StudentRelativeView>();

			NavigationService.Register<ClassroomsViewModel, ClassroomsView>();
			NavigationService.Register<ClassroomDetailsViewModel, ClassroomView>();

			NavigationService.Register<ClassroomStudentDetailsViewModel, ClassroomStudentView>();

			NavigationService.Register<DismissibleStudentsViewModel, DismissibleStudentsView>();
			NavigationService.Register<DismissalsViewModel, DismissalsView>();
			NavigationService.Register<DismissalDetailsViewModel, DismissalView>();

			NavigationService.Register<CompaniesViewModel, CompaniesView>();
			NavigationService.Register<CompanyDetailsViewModel, CompanyView>();

			NavigationService.Register<CompanyUserDetailsViewModel, CompanyUserView>();

			NavigationService.Register<PermissionsViewModel, PermissionsView>();
			NavigationService.Register<PermissionDetailsViewModel, PermissionView>();

			NavigationService.Register<RolesViewModel, RolesView>();
			NavigationService.Register<RoleDetailsViewModel, RoleView>();

			NavigationService.Register<RolePermissionDetailsViewModel, RolePermissionView>();

			NavigationService.Register<UsersViewModel, UsersView>();
			NavigationService.Register<UserDetailsViewModel, UserView>();

			NavigationService.Register<UserRoleDetailsViewModel, UserRoleView>();

			NavigationService.Register<AppLogsViewModel, AppLogsView>();

			NavigationService.Register<SettingsViewModel, SettingsView>();
		}

		static private async Task ConfigureLookupTables()
		{
			var lookupTables = ServiceLocator.Current.GetService<ILookupTables>();
			await lookupTables.InitializeAsync();
			LookupTablesProxy.Instance = lookupTables;
		}
	}
}
