using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class RolePermissionDetailsViewModel(IRolePermissionService rolePermissionService, ICommonServices commonServices) : GenericDetailsViewModel<RolePermissionDto>(commonServices)
	{
		public IRolePermissionService RolePermissionService { get; } = rolePermissionService;

		override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => $"New Role Permission, Role #{RoleId}";
		public string TitleEdit => $"Permission {Item?.PermissionId}, #{Item?.RoleId}" ?? String.Empty;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RolePermissionDetailsArgs ViewModelArgs { get; private set; }

		public long RolePermissionId { get; set; }
		public long RoleId { get; set; }

		public ICommand PermissionSelectedCommand => new RelayCommand<PermissionDto>(PermissionSelected);
		private void PermissionSelected(PermissionDto permission)
		{
			EditableItem.PermissionId = permission?.PermissionId ?? 0;
			EditableItem.Permission = permission;

			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(RolePermissionDetailsArgs args)
		{
			ViewModelArgs = args ?? RolePermissionDetailsArgs.CreateDefault();
			RolePermissionId = ViewModelArgs.RolePermissionId;
			RoleId = ViewModelArgs.RoleId;

			if (ViewModelArgs.IsNew)
			{
				Item = new RolePermissionDto { RoleId = RoleId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await RolePermissionService.GetRolePermissionAsync(RolePermissionId);
					Item = item ?? new RolePermissionDto { RolePermissionId = RolePermissionId, RoleId = RoleId, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("RolePermission", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.RoleId = Item?.RoleId ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RolePermissionDetailsViewModel, RolePermissionDto>(this, OnDetailsMessage);
			MessageService.Subscribe<RolePermissionListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public RolePermissionDetailsArgs CreateArgs()
		{
			return new RolePermissionDetailsArgs
			{
				RolePermissionId = Item?.RolePermissionId ?? 0,
				RoleId = Item?.RoleId ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(RolePermissionDto model)
		{
			try
			{
				StartStatusMessage("Saving role permission...");
				await Task.Delay(100);
				await RolePermissionService.UpdateRolePermissionAsync(model);
				EndStatusMessage("Role permission saved", LogType.Success);
				LogSuccess("RolePermission", "Save", "Role permission saved successfully", $"Role permission #{model.RoleId}, {model.Permission.DisplayName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error saving Role permission: {ex.Message}");
				LogException("RolePermission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RolePermissionDto model)
		{
			try
			{
				StartStatusMessage("Deleting role permission...");
				await Task.Delay(100);
				await RolePermissionService.DeleteRolePermissionAsync(model);
				EndStatusMessage("Role permission deleted", LogType.Warning);
				LogWarning("RolePermission", "Delete", "Role permission deleted", $"Role permission #{model.RoleId}, {model.Permission.DisplayName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting Role permission: {ex.Message}");
				LogException("RolePermission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current role permission?", "Ok", "Cancel");
		}

		override protected IEnumerable<IValidationConstraint<RolePermissionDto>> GetValidationConstraints(RolePermissionDto model)
		{
			yield return new RequiredGreaterThanZeroConstraint<RolePermissionDto>("Permission", m => m.PermissionId);
		}

		#region Handle external messages
		private async void OnDetailsMessage(RolePermissionDetailsViewModel sender, string message, RolePermissionDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.RolePermissionId == current?.RolePermissionId)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await RolePermissionService.GetRolePermissionAsync(current.RolePermissionId);
									item ??= new RolePermissionDto { RolePermissionId = RolePermissionId, RoleId = RoleId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										StatusMessage("WARNING: This RolePermission has been modified externally");
									}
								}
								catch (Exception ex)
								{
									LogException("RolePermission", "Handle Changes", ex);
								}
							});
							break;
						case "ItemDeleted":
							await OnItemDeletedExternally();
							break;
					}
				}
			}
		}

		private async void OnListMessage(RolePermissionListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<RolePermissionDto> deletedModels)
						{
							if (deletedModels.Any(r => r.RolePermissionId == current.RolePermissionId))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await RolePermissionService.GetRolePermissionAsync(current.RolePermissionId);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("RolePermission", "Handle Ranges Deleted", ex);
						}
						break;
				}
			}
		}

		private async Task OnItemDeletedExternally()
		{
			await ContextService.RunAsync(() =>
			{
				CancelEdit();
				IsEnabled = false;
				StatusMessage("WARNING: This RolePermission has been deleted externally");
			});
		}
		#endregion
	}
}
