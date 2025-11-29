using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RolesViewModel : ViewModelBase
	{
		public RolesViewModel(IRoleService roleService,
			IRolePermissionService rolePermissionService,
			ICommonServices commonServices) : base(commonServices)
		{
			_roleService = roleService;
			RoleList = new RoleListViewModel(_roleService, commonServices);
			RoleDetails = new RoleDetailsViewModel(_roleService, commonServices);
			RolePermissionList = new RolePermissionListViewModel(rolePermissionService, commonServices);
		}

		private readonly IRoleService _roleService;

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

		private async Task PopulateDetails(RoleModel selected)
		{
			try
			{
				var model = await _roleService.GetRoleAsync(selected.RoleID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("Roles", "Load Details", ex);
			}
		}

		private async Task PopulateRolePermissions(RoleModel selectedItem)
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
