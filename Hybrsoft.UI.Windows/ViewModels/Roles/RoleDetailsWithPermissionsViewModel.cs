using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RoleDetailsWithPermissionsViewModel(IRoleService roleService, IRolePermissionService rolePermissionService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public RoleDetailsViewModel RoleDetails { get; set; } = new RoleDetailsViewModel(roleService, commonServices);
		public RolePermissionListViewModel RolePermissionList { get; set; } = new RolePermissionListViewModel(rolePermissionService, commonServices);

		public async Task LoadAsync(RoleDetailsArgs args)
		{
			await RoleDetails.LoadAsync(args);

			long roleId = args?.RoleID ?? 0;
			if (roleId > 0)
			{
				await RolePermissionList.LoadAsync(new RolePermissionListArgs { RoleId = args.RoleID });
			}
			else
			{
				await RolePermissionList.LoadAsync(new RolePermissionListArgs { IsEmpty = true }, silent: true);
			}
		}
		public void Unload()
		{
			RoleDetails.CancelEdit();
			RoleDetails.Unload();
			RolePermissionList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RoleDetailsViewModel, RoleModel>(this, OnMessage);
			RoleDetails.Subscribe();
			RolePermissionList.Subscribe();
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			RoleDetails.Unsubscribe();
			RolePermissionList.Unsubscribe();
		}

		private async void OnMessage(RoleDetailsViewModel viewModel, string message, RoleModel role)
		{
			if (viewModel == RoleDetails && (message == "NewItemSaved" || message == "ItemChanged"))
			{
				await RolePermissionList.LoadAsync(new RolePermissionListArgs { RoleId = role.RoleID });
			}
		}
	}
}
