using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class PermissionDetailsViewModel(IPermissionService permissionService,
		ICommonServices commonServices) : GenericDetailsViewModel<PermissionModel>(commonServices)
	{
		private readonly IPermissionService _permissionService = permissionService;

		public override string Title => ItemIsNew
				? ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.UI, "TitleNew")
				: Item.FullName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public PermissionDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(PermissionDetailsArgs args)
		{
			ViewModelArgs = args ?? PermissionDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new PermissionModel { IsEnabled = true };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _permissionService.GetPermissionAsync(ViewModelArgs.PermissionID);
					Item = item ?? new PermissionModel { PermissionID = ViewModelArgs.PermissionID, IsEmpty = true };
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
			ViewModelArgs.PermissionID = Item?.PermissionID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<PermissionDetailsViewModel, PermissionModel>(this, OnDetailsMessage);
			MessageService.Subscribe<PermissionListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(PermissionModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.InfoMessages, "SavingPermission");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _permissionService.UpdatePermissionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.InfoMessages, "PermissionSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("Permission", "Save", "Permission saved successfully", $"Permission {model.PermissionID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingPermission0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Permission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(PermissionModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingPermission");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _permissionService.DeletePermissionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.InfoMessages, "PermissionDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("Permission", "Delete", "Permission deleted", $"Permission {model.PermissionID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingPermission0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Permission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentPermission");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<PermissionModel>> GetValidationConstraints(PermissionModel model)
		{
			string propertyName = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyName");
			var requiredName = new RequiredConstraint<PermissionModel>(propertyName, m => m.Name);
			requiredName.SetResourceService(ResourceService);

			var nameIsAlphanumeric = new AlphanumericValidationConstraint<PermissionModel>(propertyName, m => m.Name);
			nameIsAlphanumeric.SetResourceService(ResourceService);

			string propertyDisplayName = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDisplayName");
			var requiredDisplayName = new RequiredConstraint<PermissionModel>(propertyDisplayName, m => m.DisplayName);
			requiredDisplayName.SetResourceService(ResourceService);

			string propertyDescription = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDescription");
			var requiredDescription = new RequiredConstraint<PermissionModel>(propertyDescription, m => m.Description);
			requiredDescription.SetResourceService(ResourceService);

			yield return requiredName;
			yield return nameIsAlphanumeric;
			yield return requiredDisplayName;
			yield return requiredDescription;
		}

		#region Handle external messages
		private async void OnDetailsMessage(PermissionDetailsViewModel sender, string message, PermissionModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.PermissionID == current?.PermissionID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _permissionService.GetPermissionAsync(current.PermissionID);
									item ??= new PermissionModel { PermissionID = current.PermissionID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.Warnings, "ThisPermissionHasBeenModifiedExternally");
										WarningMessage(title, message);
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
						if (args is IList<PermissionModel> deletedModels)
						{
							if (deletedModels.Any(r => r.PermissionID == current.PermissionID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _permissionService.GetPermissionAsync(current.PermissionID);
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
				string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalDeletion");
				string message = ResourceService.GetString<PermissionDetailsViewModel>(ResourceFiles.Warnings, "ThisPermissionHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
