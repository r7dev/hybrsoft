using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class PermissionsViewModel : ViewModelBase
	{
		public PermissionsViewModel(IPermissionService permissionService, ICommonServices commonServices) : base(commonServices)
		{
			PermissionService = permissionService;

			PermissionList = new PermissionListViewModel(PermissionService, commonServices);
		}

		public IPermissionService PermissionService { get; }

		public PermissionListViewModel PermissionList { get; set; }

		public async Task LoadAsync(PermissionListArgs args)
		{
			await PermissionList.LoadAsync(args);
		}

		public void Unload()
		{
			PermissionList.Unload();
		}

		public void Subscribe()
		{
			PermissionList.Subscribe();
		}
		public void Unsubscribe()
		{
			PermissionList.Unsubscribe();
		}
	}
}
