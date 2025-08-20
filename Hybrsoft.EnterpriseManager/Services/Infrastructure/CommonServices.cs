using Hybrsoft.UI.Windows.Services;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class CommonServices(IContextService contextService,
		INavigationService navigationService,
		IMessageService messageService,
		IDialogService dialogService,
		ILogService logService,
		IAuthorizationService userPermissionService,
		IResourceService resourceService) : ICommonServices
	{
		public IContextService ContextService { get; } = contextService;

		public INavigationService NavigationService { get; } = navigationService;

		public IMessageService MessageService { get; } = messageService;

		public IDialogService DialogService { get; } = dialogService;

		public ILogService LogService { get; } = logService;

		public IAuthorizationService AuthorizationService { get; } = userPermissionService;

		public IResourceService ResourceService { get; } = resourceService;
	}
}
