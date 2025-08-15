using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RolesViewModel : ViewModelBase
	{
		public RolesViewModel(IRoleService roleService, IRolePermissionService rolePermissionService, ICommonServices commonServices) : base(commonServices)
		{
			RoleService = roleService;
			RoleList = new RoleListViewModel(RoleService, commonServices);
			RoleDetails = new RoleDetailsViewModel(RoleService, commonServices);
			RolePermissionList = new RolePermissionListViewModel(rolePermissionService, commonServices);
		}

		public IRoleService RoleService { get; }

		public RoleListViewModel RoleList { get; set; }

		public RoleDetailsViewModel RoleDetails { get; set; }

		public RolePermissionListViewModel RolePermissionList { get; set; }

		public async Task LoadAsync(RoleListArgs args)
		{
			await RoleList.LoadAsync(args);
		}

		public void Unload()
		{
			RoleDetails.CancelEdit();
			RoleList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RoleListViewModel>(this, OnMessage);
			RoleList.Subscribe();
			RoleDetails.Subscribe();
			RolePermissionList.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			RoleList.Unsubscribe();
			RoleDetails.Unsubscribe();
			RolePermissionList.Unsubscribe();
		}

		private async void OnMessage(RoleListViewModel viewModel, string message, object args)
		{
			if (viewModel == RoleList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (RoleDetails.IsEditMode)
			{
				StatusReady();
				RoleDetails.CancelEdit();
			}
			RolePermissionList.IsMultipleSelection = false;
			var selected = RoleList.SelectedItem;
			if (!RoleList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
					await PopulateRolePermissions(selected);
				}
			}
			RoleDetails.Item = selected;
		}

		private async Task PopulateDetails(RoleDto selected)
		{
			try
			{
				var model = await RoleService.GetRoleAsync(selected.RoleID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Roles", "Load Details", ex);
			}
		}

		private async Task PopulateRolePermissions(RoleDto selectedItem)
		{
			try
			{
				if (selectedItem != null)
				{
					await RolePermissionList.LoadAsync(new RolePermissionListArgs { RoleId = selectedItem.RoleID }, silent: true);
				}
			}
			catch (Exception ex)
			{
				LogException("Roles", "Load RolePermissions", ex);
			}
		}
	}
}
