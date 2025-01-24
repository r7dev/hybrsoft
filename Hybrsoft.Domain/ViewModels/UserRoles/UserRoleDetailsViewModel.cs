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
		public string TitleNew => $"New User Role, User #{UserId}";
		public string TitleEdit => $"Role {Item?.RoleId}, #{Item?.UserId}" ?? String.Empty;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public UserRoleDetailsArgs ViewModelArgs { get; private set; }

		public long UserRoleId { get; set; }
		public long UserId { get; set; }

		public ICommand RoleSelectedCommand => new RelayCommand<RoleDto>(RoleSelected);
		private void RoleSelected(RoleDto role)
		{
			EditableItem.RoleId = role?.RoleId ?? 0;
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
				Item = new UserRoleDto { UserId = UserId };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await UserRoleService.GetUserRoleAsync(UserRoleId);
					Item = item ?? new UserRoleDto { UserRoleId = UserRoleId, UserId = UserId, IsEmpty = true };
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
				UserRoleId = Item?.UserRoleId ?? 0,
				UserId = Item?.UserId ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(UserRoleDto model)
		{
			try
			{
				StartStatusMessage("Saving user role...");
				await Task.Delay(100);
				await UserRoleService.UpdateUserRoleAsync(model);
				EndStatusMessage("User role saved", LogType.Success);
				LogSuccess("UserRole", "Save", "User role saved successfully", $"User role #{model.UserId}, {model.Role.Name} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error saving User role: {ex.Message}");
				LogException("UserRole", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserRoleDto model)
		{
			try
			{
				StartStatusMessage("Deleting user role...");
				await Task.Delay(100);
				await UserRoleService.DeleteUserRoleAsync(model);
				EndStatusMessage("User role deleted", LogType.Warning);
				LogWarning("UserRole", "Delete", "User role deleted", $"User role #{model.UserId}, {model.Role.Name} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting User role: {ex.Message}");
				LogException("UserRole", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current user role?", "Ok", "Cancel");
		}

		override protected IEnumerable<IValidationConstraint<UserRoleDto>> GetValidationConstraints(UserRoleDto model)
		{
			yield return new RequiredGreaterThanZeroConstraint<UserRoleDto>("Role", m => m.RoleId);
		}

		#region Handle external messages
		private async void OnDetailsMessage(UserRoleDetailsViewModel sender, string message, UserRoleDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.UserRoleId == current?.UserRoleId)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await UserRoleService.GetUserRoleAsync(current.UserRoleId);
									item ??= new UserRoleDto { UserRoleId = UserRoleId, UserId = UserId, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										StatusMessage("WARNING: This UserRole has been modified externally");
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
							if (deletedModels.Any(r => r.UserRoleId == current.UserRoleId))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await UserRoleService.GetUserRoleAsync(current.UserRoleId);
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
				StatusMessage("WARNING: This UserRole has been deleted externally");
			});
		}
		#endregion
	}
}
