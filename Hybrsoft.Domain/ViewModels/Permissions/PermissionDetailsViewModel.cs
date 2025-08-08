using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class PermissionDetailsViewModel(IPermissionService permissionService, ICommonServices commonServices) : GenericDetailsViewModel<PermissionDto>(commonServices)
	{
		public IPermissionService PermissionService { get; } = permissionService;

		public override string Title
		{
			get
			{
				if (Item?.IsNew ?? true)
				{
					string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_Title");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return TitleEdit;
			}
		}
		public string TitleEdit
		{
			get
			{
				if (Item == null)
				{
					string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_TitleEdit");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return $"{Item.FullName}";
			}
		}

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
					Item = item ?? new PermissionDto { PermissionID = ViewModelArgs.PermissionID, IsEmpty = true };
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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(PermissionDetailsViewModel), "_SavingPermission"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await PermissionService.UpdatePermissionAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(PermissionDetailsViewModel), "_PermissionSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Permission", "Save", "Permission saved successfully", $"Permission {model.PermissionID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_ErrorSavingPermission0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Permission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(PermissionDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(PermissionDetailsViewModel), "_DeletingPermission"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await PermissionService.DeletePermissionAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(PermissionDetailsViewModel), "_PermissionDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("Permission", "Delete", "Permission deleted", $"Permission {model.PermissionID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_ErrorDeletingPermission0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Permission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(PermissionDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentPermission"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<PermissionDto>> GetValidationConstraints(PermissionDto model)
		{
			string resourceKeyForName = string.Concat(nameof(PermissionDetailsViewModel), "_PropertyName");
			string propertyName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForName);
			var requiredName = new RequiredConstraint<PermissionDto>(propertyName, m => m.Name);
			requiredName.SetResourceService(ResourceService);

			var nameIsAlphanumeric = new AlphanumericValidationConstraint<PermissionDto>(propertyName, m => m.Name);
			nameIsAlphanumeric.SetResourceService(ResourceService);

			string resourceKeyForDisplayName = string.Concat(nameof(PermissionDetailsViewModel), "_PropertyDisplayName");
			string propertyDisplayName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForDisplayName);
			var requiredDisplayName = new RequiredConstraint<PermissionDto>(propertyDisplayName, m => m.DisplayName);
			requiredDisplayName.SetResourceService(ResourceService);

			string resourceKeyForDescription = string.Concat(nameof(PermissionDetailsViewModel), "_PropertyDescription");
			string propertyDescription = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForDescription);
			var requiredDescription = new RequiredConstraint<PermissionDto>(propertyDescription, m => m.Description);
			requiredDescription.SetResourceService(ResourceService);

			yield return requiredName;
			yield return nameIsAlphanumeric;
			yield return requiredDisplayName;
			yield return requiredDescription;
		}

		#region Handle external messages
		private async void OnDetailsMessage(PermissionDetailsViewModel sender, string message, PermissionDto changed)
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
									var item = await PermissionService.GetPermissionAsync(current.PermissionID);
									item ??= new PermissionDto { PermissionID = current.PermissionID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_ThisPermissionHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
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
							if (deletedModels.Any(r => r.PermissionID == current.PermissionID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await PermissionService.GetPermissionAsync(current.PermissionID);
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
				string resourceKey = string.Concat(nameof(PermissionDetailsViewModel), "_ThisPermissionHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
