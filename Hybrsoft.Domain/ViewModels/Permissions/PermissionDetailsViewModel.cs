﻿using Hybrsoft.Domain.Dtos;
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
	public partial class PermissionDetailsViewModel(IPermissionService permissionService, ICommonServices commonServices) : GenericDetailsViewModel<PermissionDto>(commonServices)
	{
		public IPermissionService PermissionService { get; } = permissionService;

		override public string Title => (Item?.IsNew ?? true) ? "New Permission" : TitleEdit;
		public string TitleEdit => Item == null ? "Permission" : $"{Item.FullName}";

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public PermissionDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(PermissionDetailsArgs args)
		{
			ViewModelArgs = args ?? PermissionDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new PermissionDto { IsEnabled = true };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await PermissionService.GetPermissionAsync(ViewModelArgs.PermissionID);
					Item = item ?? new PermissionDto { PermissionId = ViewModelArgs.PermissionID, IsEmpty = true };
					IsEnabled = item.IsEnabled;
				}
				catch (Exception ex)
				{
					LogException("Permission", "Load", ex);
				}
			}
		}

		public void Unload()
		{
			ViewModelArgs.PermissionID = Item?.PermissionId ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<PermissionDetailsViewModel, PermissionDto>(this, OnDetailsMessage);
			MessageService.Subscribe<PermissionListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(PermissionDto model)
		{
			try
			{
				StartStatusMessage("Saving permission...");
				await Task.Delay(100);
				await PermissionService.UpdatePermissionAsync(model);
				EndStatusMessage("Permission saved", LogType.Success);
				LogSuccess("Permission", "Save", "Permission saved successfully", $"Permission {model.PermissionId} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error saving Permission: {ex.Message}");
				LogException("Permission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(PermissionDto model)
		{
			try
			{
				StartStatusMessage("Deleting permission...");
				await Task.Delay(100);
				await PermissionService.DeletePermissionAsync(model);
				EndStatusMessage("Permission deleted", LogType.Warning);
				LogWarning("Permission", "Delete", "Permission deleted", $"Permission {model.PermissionId} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting Permission: {ex.Message}");
				LogException("Permission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current permission?", "Delete", "Cancel");
		}

		override protected IEnumerable<IValidationConstraint<PermissionDto>> GetValidationConstraints(PermissionDto model)
		{
			yield return new RequiredConstraint<PermissionDto>("Name", m => m.Name);
			yield return new AlphanumericValidationConstraint<PermissionDto>("Name", m => m.Name);
			yield return new RequiredConstraint<PermissionDto>("Display Name", m => m.DisplayName);
			yield return new RequiredConstraint<PermissionDto>("Description", m => m.Description);
		}

		#region Handle external messages
		private async void OnDetailsMessage(PermissionDetailsViewModel sender, string message, PermissionDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.PermissionId == current?.PermissionId)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await PermissionService.GetPermissionAsync(current.PermissionId);
									item ??= new PermissionDto { PermissionId = current.PermissionId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										StatusMessage("WARNING: This permission has been modified externally");
									}
								}
								catch (Exception ex)
								{
									LogException("Permission", "Handle Changes", ex);
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

		private async void OnListMessage(PermissionListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<PermissionDto> deletedModels)
						{
							if (deletedModels.Any(r => r.PermissionId == current.PermissionId))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await PermissionService.GetPermissionAsync(current.PermissionId);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Permission", "Handle Ranges Deleted", ex);
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
				StatusMessage("WARNING: This permission has been deleted externally");
			});
		}
		#endregion
	}
}