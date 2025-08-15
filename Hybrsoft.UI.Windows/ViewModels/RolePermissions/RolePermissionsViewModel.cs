using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RolePermissionsViewModel(IRolePermissionService rolePermissionService, ICommonServices commonServices) : ViewModelBase(commonServices)
	{
		public IRolePermissionService RolePermissionService { get; } = rolePermissionService;

		public RolePermissionListViewModel RolePermissionList { get; set; } = new RolePermissionListViewModel(rolePermissionService, commonServices);
		public RolePermissionDetailsViewModel RolePermissionDetails { get; set; } = new RolePermissionDetailsViewModel(rolePermissionService, commonServices);

		public async Task LoadAsync(RolePermissionListArgs args)
		{
			await RolePermissionList.LoadAsync(args);
		}
		public void Unload()
		{
			RolePermissionDetails.CancelEdit();
			RolePermissionList.Unload();
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RolePermissionListViewModel>(this, OnMessage);
			RolePermissionList.Subscribe();
			RolePermissionDetails.Subscribe();
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
			RolePermissionList.Unsubscribe();
			RolePermissionDetails.Unsubscribe();
		}

		private async void OnMessage(RolePermissionListViewModel viewModel, string message, object args)
		{
			if (viewModel == RolePermissionList && message == "ItemSelected")
			{
				await ContextService.RunAsync(() =>
				{
					OnItemSelected();
				});
			}
		}

		private async void OnItemSelected()
		{
			if (RolePermissionDetails.IsEditMode)
			{
				StatusReady();
				RolePermissionDetails.CancelEdit();
			}
			var selected = RolePermissionList.SelectedItem;
			if (!RolePermissionList.IsMultipleSelection)
			{
				if (selected != null && !selected.IsEmpty)
				{
					await PopulateDetails(selected);
				}
			}
			RolePermissionDetails.Item = selected;
		}

		private async Task PopulateDetails(RolePermissionModel selected)
		{
			try
			{
				var model = await RolePermissionService.GetRolePermissionAsync(selected.RolePermissionID);
				selected.Merge(model);
			}
			catch (Exception ex)
			{
				LogException("RolePermissions", "Load Details", ex);
			}
		}
	}
}
