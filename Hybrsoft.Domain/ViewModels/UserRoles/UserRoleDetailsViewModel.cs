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
	public partial class UserRoleDetailsViewModel(IUserRoleService userRoleService, ICommonServices commonServices) : GenericDetailsViewModel<UserRoleDto>(commonServices)
	{
		public IUserRoleService UserRoleService { get; } = userRoleService;

		override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew
		{
			get
			{
				string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_TitleNew");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, UserId);
				return message;
			}
		}
		public string TitleEdit
		{
			get
			{
				string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_TitleEdit");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, Item?.RoleID, Item?.UserID);
				return message ?? String.Empty;
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public UserRoleDetailsArgs ViewModelArgs { get; private set; }

		public long UserRoleId { get; set; }
		public long UserId { get; set; }

		public ICommand RoleSelectedCommand => new RelayCommand<RoleDto>(RoleSelected);
		private void RoleSelected(RoleDto role)
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

			if (ViewModelArgs.IsNew)
			{
				Item = new UserRoleDto { UserID = UserId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await UserRoleService.GetUserRoleAsync(UserRoleId);
					Item = item ?? new UserRoleDto { UserRoleID = UserRoleId, UserID = UserId, IsEmpty = true };
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
			MessageService.Subscribe<UserRoleDetailsViewModel, UserRoleDto>(this, OnDetailsMessage);
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

		protected override async Task<bool> SaveItemAsync(UserRoleDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserRoleDetailsViewModel), "_SavingUserRole"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await UserRoleService.UpdateUserRoleAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserRoleDetailsViewModel), "_UserRoleSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("UserRole", "Save", "User role saved successfully", $"User role #{model.UserID}, {model.Role.Name} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_ErrorSavingUserRole0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("UserRole", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserRoleDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserRoleDetailsViewModel), "_DeletingUserRole"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await UserRoleService.DeleteUserRoleAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(UserRoleDetailsViewModel), "_UserRoleDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("UserRole", "Delete", "User role deleted", $"User role #{model.UserID}, {model.Role.Name} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_ErrorDeletingUserRole0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("UserRole", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(UserRoleDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentUserRole"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<UserRoleDto>> GetValidationConstraints(UserRoleDto model)
		{
			string resourceKeyForRole = string.Concat(nameof(UserRoleDetailsViewModel), "_PropertyRole");
			string propertyRole = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForRole);
			var requiredRole = new RequiredGreaterThanZeroConstraint<UserRoleDto>(propertyRole, m => m.RoleID);
			requiredRole.SetResourceService(ResourceService);

			yield return requiredRole;
		}

		#region Handle external messages
		private async void OnDetailsMessage(UserRoleDetailsViewModel sender, string message, UserRoleDto changed)
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
									var item = await UserRoleService.GetUserRoleAsync(current.UserRoleID);
									item ??= new UserRoleDto { UserRoleID = UserRoleId, UserID = UserId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_ThisUserRoleHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										StatusMessage(message);
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
						if (args is IList<UserRoleDto> deletedModels)
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
							var model = await UserRoleService.GetUserRoleAsync(current.UserRoleID);
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
				string resourceKey = string.Concat(nameof(UserRoleDetailsViewModel), "_ThisUserRoleHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				StatusMessage(message);
			});
		}
		#endregion
	}
}
