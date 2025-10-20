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
	public partial class UserDetailsViewModel(IUserService userService, ICommonServices commonServices) : GenericDetailsViewModel<UserModel>(commonServices)
	{
		public IUserService UserService { get; } = userService;

		public override string Title =>
			ItemIsNew
				? ResourceService.GetString(nameof(ResourceFiles.UI), $"{nameof(UserDetailsViewModel)}_TitleNew")
				: Item.FullName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public UserDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(UserDetailsArgs args)
		{
			ViewModelArgs = args ?? UserDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new UserModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await UserService.GetUserAsync(ViewModelArgs.UserID);
					Item = item ?? new UserModel { UserID = ViewModelArgs.UserID, IsEmpty = true };
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
			MessageService.Subscribe<UserDetailsViewModel, UserModel>(this, OnDetailsMessage);
			MessageService.Subscribe<UserListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(UserModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(UserDetailsViewModel)}_SavingUser");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await UserService.UpdateUserAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(UserDetailsViewModel)}_UserSaved");
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("User", "Save", "User saved successfully", $"User {model.UserID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(UserDetailsViewModel)}_ErrorSavingUser0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("User", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(UserDetailsViewModel)}_DeletingUser");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await UserService.DeleteUserAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(UserDetailsViewModel)}_UserDeleted");
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("User", "Delete", "User deleted", $"User {model.UserID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(UserDetailsViewModel)}_ErrorDeletingUser0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("User", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), $"{nameof(UserDetailsViewModel)}_AreYouSureYouWantToDeleteCurrentUser");
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, "Are you sure you want to delete current user?", delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<UserModel>> GetValidationConstraints(UserModel model)
		{
			string resourceKeyForFirstName = $"{nameof(UserDetailsViewModel)}_PropertyFirstName";
			string propertyFirstName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForFirstName);
			var requiredFirstName = new RequiredConstraint<UserModel>(propertyFirstName, m => m.FirstName);
			requiredFirstName.SetResourceService(ResourceService);

			string resourceKeyForLastName = $"{nameof(UserDetailsViewModel)}_PropertyLastName";
			string propertyLastName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForLastName);
			var requiredLastName = new RequiredConstraint<UserModel>(propertyLastName, m => m.LastName);
			requiredLastName.SetResourceService(ResourceService);

			string resourceKeyForEmail = $"{nameof(UserDetailsViewModel)}_PropertyEmail";
			string propertyEmail = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForEmail);
			var requiredEmail = new RequiredConstraint<UserModel>(propertyEmail, m => m.Email);
			requiredEmail.SetResourceService(ResourceService);
			var emailIsValid = new EmailValidationConstraint<UserModel>(propertyEmail, m => m.Email);
			emailIsValid.SetResourceService(ResourceService);

			string resourceKeyForPassword = $"{nameof(UserDetailsViewModel)}_PropertyPassword";
			string propertyPassword = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForPassword);
			var requiredPassword = new RequiredConstraint<UserModel>(propertyPassword, m => m.Password);
			requiredPassword.SetResourceService(ResourceService);

			yield return requiredFirstName;
			yield return requiredLastName;
			yield return requiredEmail;
			yield return emailIsValid;
			yield return requiredPassword;
		}

		#region Handle external messages
		private async void OnDetailsMessage(UserDetailsViewModel sender, string message, UserModel changed)
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
									item ??= new UserModel { UserID = current.UserID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = $"{nameof(UserDetailsViewModel)}_ThisUserHasBeenModifiedExternally";
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										StatusMessage(message);
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
						if (args is IList<UserModel> deletedModels)
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
				string resourceKey = $"{nameof(UserDetailsViewModel)}_ThisUserHasBeenDeletedExternally";
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				StatusMessage(message);
			});
		}
		#endregion
	}
}
