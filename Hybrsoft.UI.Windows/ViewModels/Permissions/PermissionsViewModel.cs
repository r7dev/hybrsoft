using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class PermissionsViewModel : ViewModelBase
	{
		public PermissionsViewModel(IPermissionService permissionService, ICommonServices commonServices) : base(commonServices)
		{
			_permissionService = permissionService;

			PermissionList = new PermissionListViewModel(_permissionService, commonServices);
		}

		private readonly IPermissionService _permissionService;

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
