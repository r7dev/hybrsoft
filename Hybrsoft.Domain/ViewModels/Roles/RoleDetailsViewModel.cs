using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class RoleDetailsViewModel(IRoleService roleService, ICommonServices commonServices) : GenericDetailsViewModel<RoleDto>(commonServices)
	{
		public IRoleService RoleService { get; } = roleService;

		override public string Title => (Item?.IsNew ?? true) ? "New Role" : TitleEdit;
		public string TitleEdit => Item == null ? "Role" : $"Role #{Item?.RoleId}";

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RoleDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RoleDetailsArgs args)
		{
			ViewModelArgs = args ?? RoleDetailsArgs.CreateDefault();
			if (ViewModelArgs.IsNew)
			{
				Item = new RoleDto();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await RoleService.GetRoleAsync(ViewModelArgs.RoleID);
					Item = item ?? new RoleDto { RoleId = ViewModelArgs.RoleID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("Role", "Load", ex);
				}
			}
			NotifyPropertyChanged(nameof(ItemIsNew));
		}

		public void Unload()
		{
			ViewModelArgs.RoleID = Item?.RoleId ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RoleDetailsViewModel, RoleDto>(this, OnDetailsMessage);
			MessageService.Subscribe<RoleListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(RoleDto model)
		{
			try
			{
				StartStatusMessage("Saving role...");
				await Task.Delay(100);
				await RoleService.UpdateRoleAsync(model);
				EndStatusMessage("Role saved", LogType.Success);
				LogSuccess("Role", "Save", "Role saved successfully", $"Role {model.RoleId} '{model.Name}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error saving Role: {ex.Message}");
				LogException("Role", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RoleDto model)
		{
			try
			{
				StartStatusMessage("Deleting role...");
				await Task.Delay(100);
				await RoleService.DeleteRoleAsync(model);
				EndStatusMessage("Role deleted", LogType.Warning);
				LogWarning("Role", "Delete", "Role deleted", $"Role {model.RoleId} '{model.Name}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting Role: {ex.Message}");
				LogException("Role", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current role?", "Delete", "Cancel");
		}

		override protected IEnumerable<IValidationConstraint<RoleDto>> GetValidationConstraints(RoleDto model)
		{
			yield return new RequiredConstraint<RoleDto>("Name", m => m.Name);
			yield return new AlphanumericValidationConstraint<RoleDto>("Name", m => m.Name);
		}

		#region Handle external messages
		private async void OnDetailsMessage(RoleDetailsViewModel sender, string message, RoleDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.RoleId == current?.RoleId)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await RoleService.GetRoleAsync(current.RoleId);
									item ??= new RoleDto { RoleId = current.RoleId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										StatusMessage("WARNING: This role has been modified externally");
									}
								}
								catch (Exception ex)
								{
									LogException("Role", "Handle Changes", ex);
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

		private async void OnListMessage(RoleListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<RoleDto> deletedModels)
						{
							if (deletedModels.Any(r => r.RoleId == current.RoleId))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await RoleService.GetRoleAsync(current.RoleId);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Role", "Handle Ranges Deleted", ex);
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
				StatusMessage("WARNING: This role has been deleted externally");
			});
		}
		#endregion
	}
}
