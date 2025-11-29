using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RolePermissionDetailsViewModel(IRolePermissionService rolePermissionService,
		ICommonServices commonServices) : GenericDetailsViewModel<RolePermissionModel>(commonServices)
	{
		private readonly IRolePermissionService _rolePermissionService = rolePermissionService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => string.Format(ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.UI, "TitleNew"), RoleId);
		public string TitleEdit => string.Format(ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.UI, "TitleEdit"), Item?.PermissionID, Item?.RoleID);

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RolePermissionDetailsArgs ViewModelArgs { get; private set; }

		public long RolePermissionId { get; set; }
		public long RoleId { get; set; }
		private IList<long> _addedPermissionKeys;
		public IList<long> AddedPermissionKeys
		{
			get => _addedPermissionKeys;
			set
			{
				_addedPermissionKeys = value;
				NotifyPropertyChanged(nameof(AddedPermissionKeys));
			}
		}

		public ICommand PermissionSelectedCommand => new RelayCommand<PermissionModel>(PermissionSelected);
		private void PermissionSelected(PermissionModel model)
		{
			EditableItem.PermissionID = model?.PermissionID ?? 0;
			EditableItem.Permission = model;

			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(RolePermissionDetailsArgs args)
		{
			ViewModelArgs = args ?? RolePermissionDetailsArgs.CreateDefault();
			RolePermissionId = ViewModelArgs.RolePermissionId;
			RoleId = ViewModelArgs.RoleId;
			AddedPermissionKeys = await _rolePermissionService.GetAddedPermissionKeysInRoleAsync(RoleId);

			if (ViewModelArgs.IsNew)
			{
				Item = new RolePermissionModel { RoleID = RoleId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _rolePermissionService.GetRolePermissionAsync(RolePermissionId);
					Item = item ?? new RolePermissionModel { RolePermissionID = RolePermissionId, RoleID = RoleId, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("RolePermission", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.RoleId = Item?.RoleID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RolePermissionDetailsViewModel, RolePermissionModel>(this, OnDetailsMessage);
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
				RolePermissionId = Item?.RolePermissionID ?? 0,
				RoleId = Item?.RoleID ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(RolePermissionModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.InfoMessages, "SavingRolePermission");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _rolePermissionService.UpdateRolePermissionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.InfoMessages, "RolePermissionSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("RolePermission", "Save", "Role permission saved successfully", $"Role permission #{model.RoleID}, {model.Permission.DisplayName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingRolePermission0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("RolePermission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RolePermissionModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingRolePermission");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _rolePermissionService.DeleteRolePermissionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.InfoMessages, "RolePermissionDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("RolePermission", "Delete", "Role permission deleted", $"Role permission #{model.RoleID}, {model.Permission.DisplayName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingRolePermission0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("RolePermission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentRolePermission");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<RolePermissionModel>> GetValidationConstraints(RolePermissionModel model)
		{
			string propertyPermission = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyPermission");
			var requiredPermission = new RequiredGreaterThanZeroConstraint<RolePermissionModel>(propertyPermission, m => m.PermissionID);
			requiredPermission.SetResourceService(ResourceService);

			yield return requiredPermission;
		}

		#region Handle external messages
		private async void OnDetailsMessage(RolePermissionDetailsViewModel sender, string message, RolePermissionModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.RolePermissionID == current?.RolePermissionID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _rolePermissionService.GetRolePermissionAsync(current.RolePermissionID);
									item ??= new RolePermissionModel { RolePermissionID = RolePermissionId, RoleID = RoleId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.Warnings, "ThisRolePermissionHasBeenModifiedExternally");
										WarningMessage(title, message);
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
						if (args is IList<RolePermissionModel> deletedModels)
						{
							if (deletedModels.Any(r => r.RolePermissionID == current.RolePermissionID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _rolePermissionService.GetRolePermissionAsync(current.RolePermissionID);
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
				string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalDeletion");
				string message = ResourceService.GetString<RolePermissionDetailsViewModel>(ResourceFiles.Warnings, "ThisRolePermissionHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
