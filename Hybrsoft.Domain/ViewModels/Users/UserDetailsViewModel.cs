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
	#region UserDetailsArgs
	public class UserDetailsArgs
	{
		static public UserDetailsArgs CreateDefault() => new();

		public long UserID { get; set; }

		public bool IsNew => UserID <= 0;
	}
	#endregion

	public partial class UserDetailsViewModel(IUserService userService, ICommonServices commonServices) : GenericDetailsViewModel<UserDto>(commonServices)
	{
		public IUserService UserService { get; } = userService;

		override public string Title => (Item?.IsNew ?? true) ? "New User" : TitleEdit;
		public string TitleEdit => Item == null ? "User" : $"{Item.FullName}";

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public UserDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserDetailsArgs args)
		{
			ViewModelArgs = args ?? UserDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new UserDto();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await UserService.GetUserAsync(ViewModelArgs.UserID);
					Item = item ?? new UserDto { UserID = ViewModelArgs.UserID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("User", "Load", ex);
				}
			}
		}

		public void Unload()
		{
			ViewModelArgs.UserID = Item?.UserID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<UserDetailsViewModel, UserDto>(this, OnDetailsMessage);
			MessageService.Subscribe<UserListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(UserDto model)
		{
			try
			{
				StartStatusMessage("Saving user...");
				await Task.Delay(100);
				await UserService.UpdateUserAsync(model);
				EndStatusMessage("User saved", LogType.Success);
				LogSuccess("User", "Save", "User saved successfully", $"User {model.UserID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error saving User: {ex.Message}");
				LogException("User", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserDto model)
		{
			try
			{
				StartStatusMessage("Deleting user...");
				await Task.Delay(100);
				await UserService.DeleteUserAsync(model);
				EndStatusMessage("User deleted", LogType.Warning);
				LogWarning("User", "Delete", "User deleted", $"User {model.UserID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting User: {ex.Message}");
				LogException("User", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current user?", "Delete", "Cancel");
		}

		override protected IEnumerable<IValidationConstraint<UserDto>> GetValidationConstraints(UserDto model)
		{
			yield return new RequiredConstraint<UserDto>("Fist Name", m => m.FirstName);
			yield return new RequiredConstraint<UserDto>("Last Name", m => m.LastName);
			yield return new RequiredConstraint<UserDto>("Email", m => m.Email);
			yield return new EmailValidationConstraint<UserDto>("Email", m => m.Email);
		}

		#region Handle external messages
		private async void OnDetailsMessage(UserDetailsViewModel sender, string message, UserDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.UserID == current?.UserID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await UserService.GetUserAsync(current.UserID);
									item ??= new UserDto { UserID = current.UserID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										StatusMessage("WARNING: This user has been modified externally");
									}
								}
								catch (Exception ex)
								{
									LogException("User", "Handle Changes", ex);
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

		private async void OnListMessage(UserListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<UserDto> deletedModels)
						{
							if (deletedModels.Any(r => r.UserID == current.UserID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await UserService.GetUserAsync(current.UserID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("User", "Handle Ranges Deleted", ex);
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
				StatusMessage("WARNING: This user has been deleted externally");
			});
		}
		#endregion
	}
}
