using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class UserRoleDetailsViewModel(IUserRoleService userRoleService,
		ICommonServices commonServices) : GenericDetailsViewModel<UserRoleModel>(commonServices)
	{
		private readonly IUserRoleService _userRoleService = userRoleService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => string.Format(ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.UI, "TitleNew"), UserId);
		public string TitleEdit => string.Format(ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.UI, "TitleEdit"), Item?.RoleID, Item?.UserID);

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public UserRoleDetailsArgs ViewModelArgs { get; private set; }

		public long UserRoleId { get; set; }
		public long UserId { get; set; }
		private IList<long> _addedRoleKeys;
		public IList<long> AddedRoleKeys
		{
			get => _addedRoleKeys;
			set
			{
				_addedRoleKeys = value;
				NotifyPropertyChanged(nameof(AddedRoleKeys));
			}
		}

		public ICommand RoleSelectedCommand => new RelayCommand<RoleModel>(RoleSelected);
		private void RoleSelected(RoleModel role)
		{
			EditableItem.RoleID = role?.RoleID ?? 0;
			EditableItem.Role = role;

			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(UserRoleDetailsArgs args)
		{
			ViewModelArgs = args ?? UserRoleDetailsArgs.CreateDefault();
			UserRoleId = ViewModelArgs.UserRoleId;
			UserId = ViewModelArgs.UserId;
			AddedRoleKeys = await _userRoleService.GetAddedRoleKeysInUserAsync(UserId);

			if (ViewModelArgs.IsNew)
			{
				Item = new UserRoleModel { UserID = UserId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _userRoleService.GetUserRoleAsync(UserRoleId);
					Item = item ?? new UserRoleModel { UserRoleID = UserRoleId, UserID = UserId, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("UserRole", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.UserId = UserId;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<UserRoleDetailsViewModel, UserRoleModel>(this, OnDetailsMessage);
			MessageService.Subscribe<UserRoleListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public UserRoleDetailsArgs CreateArgs()
		{
			return new UserRoleDetailsArgs
			{
				UserRoleId = Item?.UserRoleID ?? 0,
				UserId = Item?.UserID ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(UserRoleModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.InfoMessages, "SavingUserRole");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _userRoleService.UpdateUserRoleAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.InfoMessages, "UserRoleSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("UserRole", "Save", "User role saved successfully", $"User role #{model.UserID}, {model.Role.Name} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingUserRole0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("UserRole", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserRoleModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingUserRole");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _userRoleService.DeleteUserRoleAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.InfoMessages, "UserRoleDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("UserRole", "Delete", "User role deleted", $"User role #{model.UserID}, {model.Role.Name} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingUserRole0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("UserRole", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentUserRole");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<UserRoleModel>> GetValidationConstraints(UserRoleModel model)
		{
			string propertyRole = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyRole");
			var requiredRole = new RequiredGreaterThanZeroConstraint<UserRoleModel>(propertyRole, m => m.RoleID);
			requiredRole.SetResourceService(ResourceService);

			yield return requiredRole;
		}

		#region Handle external messages
		private async void OnDetailsMessage(UserRoleDetailsViewModel sender, string message, UserRoleModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.UserRoleID == current?.UserRoleID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _userRoleService.GetUserRoleAsync(current.UserRoleID);
									item ??= new UserRoleModel { UserRoleID = UserRoleId, UserID = UserId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.Warnings, "ThisUserRoleHasBeenModifiedExternally");
										StatusMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("UserRole", "Handle Changes", ex);
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

		private async void OnListMessage(UserRoleListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<UserRoleModel> deletedModels)
						{
							if (deletedModels.Any(r => r.UserRoleID == current.UserRoleID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _userRoleService.GetUserRoleAsync(current.UserRoleID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("UserRole", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<UserRoleDetailsViewModel>(ResourceFiles.Warnings, "ThisUserRoleHasBeenDeletedExternally");
				StatusMessage(title, message);
			});
		}
		#endregion
	}
}
