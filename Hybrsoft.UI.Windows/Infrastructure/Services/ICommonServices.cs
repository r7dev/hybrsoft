namespace Hybrsoft.UI.Windows.Services
{
	public interface ICommonServices
	{
		IContextService ContextService { get; }
		INavigationService NavigationService { get; }
		IMessageService MessageService { get; }
		IDialogService DialogService { get; }
		ILogService LogService { get; }
		IUserPermissionService UserPermissionService { get; }
		IResourceService ResourceService { get; }
	}
}
