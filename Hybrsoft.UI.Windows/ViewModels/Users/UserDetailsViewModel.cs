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
	public partial class UserDetailsViewModel(IUserService userService,
		ICommonServices commonServices) : GenericDetailsViewModel<UserModel>(commonServices)
	{
		private readonly IUserService _userService = userService;

		public override string Title => ItemIsNew
				? ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.UI, "TitleNew")
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
					var item = await _userService.GetUserAsync(ViewModelArgs.UserID);
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
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.InfoMessages, "SavingUser");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _userService.UpdateUserAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.InfoMessages, "UserSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("User", "Save", "User saved successfully", $"User {model.UserID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingUser0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("User", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(UserModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingUser");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _userService.DeleteUserAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.InfoMessages, "UserDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("User", "Delete", "User deleted", $"User {model.UserID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string errorTitle = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingUser0");
				StatusError(errorTitle, string.Format(message, ex.Message));
				LogException("User", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentUser");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<UserModel>> GetValidationConstraints(UserModel model)
		{
			string propertyFirstName = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyFirstName");
			var requiredFirstName = new RequiredConstraint<UserModel>(propertyFirstName, m => m.FirstName);
			requiredFirstName.SetResourceService(ResourceService);

			string propertyLastName = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyLastName");
			var requiredLastName = new RequiredConstraint<UserModel>(propertyLastName, m => m.LastName);
			requiredLastName.SetResourceService(ResourceService);

			string propertyEmail = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyEmail");
			var requiredEmail = new RequiredConstraint<UserModel>(propertyEmail, m => m.Email);
			requiredEmail.SetResourceService(ResourceService);
			var emailIsValid = new EmailValidationConstraint<UserModel>(propertyEmail, m => m.Email);
			emailIsValid.SetResourceService(ResourceService);

			string propertyPassword = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyPassword");
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
									var item = await _userService.GetUserAsync(current.UserID);
									item ??= new UserModel { UserID = current.UserID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.Warnings, "ThisUserHasBeenModifiedExternally");
										StatusMessage(title, message);
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
							var model = await _userService.GetUserAsync(current.UserID);
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
				string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalDeletion");
				string message = ResourceService.GetString<UserDetailsViewModel>(ResourceFiles.Warnings, "ThisUserHasBeenDeletedExternally");
				StatusMessage(title, message);
			});
		}
		#endregion
	}
}
