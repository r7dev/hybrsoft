using Hybrsoft.Domain.Interfaces.Infrastructure;

namespace Hybrsoft.EnterpriseManager.Services.Infrastructure
{
	public class CommonServices(IContextService contextService, INavigationService navigationService, IMessageService messageService, IDialogService dialogService, ILogService logService) : ICommonServices
	{
		public IContextService ContextService { get; } = contextService;

		public INavigationService NavigationService { get; } = navigationService;

		public IMessageService MessageService { get; } = messageService;

		public IDialogService DialogService { get; } = dialogService;

		public ILogService LogService { get; } = logService;
	}
}
