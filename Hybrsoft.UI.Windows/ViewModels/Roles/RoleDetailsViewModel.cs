using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RoleDetailsViewModel(IRoleService roleService, ICommonServices commonServices) : GenericDetailsViewModel<RoleModel>(commonServices)
	{
		public IRoleService RoleService { get; } = roleService;

		public override string Title
		{
			get
			{
				string suffix = ItemIsNew ? "New" : "Edit";
				string resourceKey = $"{nameof(RoleDetailsViewModel)}_Title{suffix}";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				return ItemIsNew ? resourceValue : $"{resourceValue} #{Item.RoleID}";
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RoleDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(RoleDetailsArgs args)
		{
			ViewModelArgs = args ?? RoleDetailsArgs.CreateDefault();
			if (ViewModelArgs.IsNew)
			{
				Item = new RoleModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await RoleService.GetRoleAsync(ViewModelArgs.RoleID);
					Item = item ?? new RoleModel { RoleID = ViewModelArgs.RoleID, IsEmpty = true };
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
			ViewModelArgs.RoleID = Item?.RoleID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RoleDetailsViewModel, RoleModel>(this, OnDetailsMessage);
			MessageService.Subscribe<RoleListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(RoleModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RoleDetailsViewModel)}_SavingRole");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RoleService.UpdateRoleAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RoleDetailsViewModel)}_RoleSaved");
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Role", "Save", "Role saved successfully", $"Role {model.RoleID} '{model.Name}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(RoleDetailsViewModel)}_ErrorSavingRole0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Role", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RoleModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RoleDetailsViewModel)}_DeletingRole");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RoleService.DeleteRoleAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RoleDetailsViewModel)}_RoleDeleted");
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("Role", "Delete", "Role deleted", $"Role {model.RoleID} '{model.Name}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(RoleDetailsViewModel)}_ErrorDeletingRole0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Role", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), $"{nameof(RoleDetailsViewModel)}_AreYouSureYouWantToDeleteCurrentRole");
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<RoleModel>> GetValidationConstraints(RoleModel model)
		{
			string resourceKeyForName = $"{nameof(RoleDetailsViewModel)}_PropertyName";
			string propertyName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForName);
			var requiredName = new RequiredConstraint<RoleModel>(propertyName, m => m.Name);
			requiredName.SetResourceService(ResourceService);

			var nameIsAlphanumeric = new AlphanumericValidationConstraint<RoleModel>(propertyName, m => m.Name);
			nameIsAlphanumeric.SetResourceService(ResourceService);

			yield return requiredName;
			yield return nameIsAlphanumeric;
		}

		#region Handle external messages
		private async void OnDetailsMessage(RoleDetailsViewModel sender, string message, RoleModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.RoleID == current?.RoleID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await RoleService.GetRoleAsync(current.RoleID);
									item ??= new RoleModel { RoleID = current.RoleID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = $"{nameof(RoleDetailsViewModel)}_ThisRoleHasBeenModifiedExternally";
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
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
						if (args is IList<RoleModel> deletedModels)
						{
							if (deletedModels.Any(r => r.RoleID == current.RoleID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await RoleService.GetRoleAsync(current.RoleID);
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
				string resourceKey = $"{nameof(RoleDetailsViewModel)}_ThisRoleHasBeenDeletedExternally";
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
