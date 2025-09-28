using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RolePermissionDetailsViewModel(IRolePermissionService rolePermissionService, ICommonServices commonServices) : GenericDetailsViewModel<RolePermissionModel>(commonServices)
	{
		public IRolePermissionService RolePermissionService { get; } = rolePermissionService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew
		{
			get
			{
				string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_TitleNew";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, RoleId);
				return message;
			}
		}
		public string TitleEdit
		{
			get
			{
				string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_TitleEdit";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, Item?.PermissionID, Item?.RoleID);
				return message ?? String.Empty;
			}
		}

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
			AddedPermissionKeys = await RolePermissionService.GetAddedPermissionKeysInRoleAsync(RoleId);

			if (ViewModelArgs.IsNew)
			{
				Item = new RolePermissionModel { RoleID = RoleId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await RolePermissionService.GetRolePermissionAsync(RolePermissionId);
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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RolePermissionDetailsViewModel)}_SavingRolePermission");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RolePermissionService.UpdateRolePermissionAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RolePermissionDetailsViewModel)}_RolePermissionSaved");
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("RolePermission", "Save", "Role permission saved successfully", $"Role permission #{model.RoleID}, {model.Permission.DisplayName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_ErrorSavingRolePermission0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("RolePermission", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RolePermissionModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RolePermissionDetailsViewModel)}_DeletingRolePermission");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RolePermissionService.DeleteRolePermissionAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(RolePermissionDetailsViewModel)}_RolePermissionDeleted");
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("RolePermission", "Delete", "Role permission deleted", $"Role permission #{model.RoleID}, {model.Permission.DisplayName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_ErrorDeletingRolePermission0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("RolePermission", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), $"{nameof(RolePermissionDetailsViewModel)}_AreYouSureYouWantToDeleteCurrentRolePermission");
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<RolePermissionModel>> GetValidationConstraints(RolePermissionModel model)
		{
			string resourceKeyForPermission = $"{nameof(RolePermissionDetailsViewModel)}_PropertyPermission";
			string propertyPermission = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForPermission);
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
									var item = await RolePermissionService.GetRolePermissionAsync(current.RolePermissionID);
									item ??= new RolePermissionModel { RolePermissionID = RolePermissionId, RoleID = RoleId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_ThisRolePermissionHasBeenModifiedExternally";
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
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
							var model = await RolePermissionService.GetRolePermissionAsync(current.RolePermissionID);
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
				string resourceKey = $"{nameof(RolePermissionDetailsViewModel)}_ThisRolePermissionHasBeenDeletedExternally";
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
