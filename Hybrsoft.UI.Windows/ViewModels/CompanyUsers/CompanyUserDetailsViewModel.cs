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
	public partial class CompanyUserDetailsViewModel(ICompanyService companyService,
		ICompanyUserService companyUserService,
		ICommonServices commonServices) : GenericDetailsViewModel<CompanyUserModel>(commonServices)
	{
		private readonly ICompanyService _companyService = companyService;
		private readonly ICompanyUserService _companyUserService = companyUserService;

		private bool _hasEditorPermission;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => string.Format(ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.UI, "TitleNew"), CompanyID);
		public string TitleEdit => string.Format(ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.UI, "TitleEdit"), Item?.CompanyUserID, Item?.CompanyID);

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public CompanyUserDetailsArgs ViewModelArgs { get; private set; }

		public long CompanyUserID { get; set; }
		public long CompanyID { get; set; }
		private IList<long> _addedUserKeys;
		public IList<long> AddedUserKeys
		{
			get => _addedUserKeys;
			set
			{
				_addedUserKeys = value;
				NotifyPropertyChanged(nameof(AddedUserKeys));
			}
		}

		public ICommand UserSelectedCommand => new RelayCommand<UserModel>(UserSelected);
		private void UserSelected(UserModel model)
		{
			EditableItem.UserID = model?.UserID ?? 0;
			EditableItem.User = model;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(CompanyUserDetailsArgs args)
		{
			ViewModelArgs = args ?? CompanyUserDetailsArgs.CreateDefault();
			CompanyUserID = ViewModelArgs.CompanyUserID;
			CompanyID = ViewModelArgs.CompanyID;
			var company = await _companyService.GetCompanyAsync(CompanyID);
			AddedUserKeys = await _companyUserService.GetAddedUserKeysInCompanyAsync(CompanyID);
			_hasEditorPermission = AuthorizationService.HasPermission(Permissions.CompanyEditor);

			if (ViewModelArgs.IsNew)
			{
				Item = new CompanyUserModel() { CompanyID = CompanyID, Company = company };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _companyUserService.GetCompanyUserAsync(CompanyUserID);
					Item = item ?? new CompanyUserModel() { CompanyUserID = CompanyUserID, CompanyID = CompanyID, IsEmpty = true };
					Item.Company = company;
				}
				catch (Exception ex)
				{
					LogException("CompanyUser", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.CompanyID = CompanyID;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<CompanyUserDetailsViewModel, CompanyUserModel>(this, OnDetailsMessage);
			MessageService.Subscribe<CompanyUserListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public CompanyUserDetailsArgs CreateArgs()
		{
			return new CompanyUserDetailsArgs
			{
				CompanyUserID = Item?.CompanyUserID ?? 0,
				CompanyID = Item?.CompanyID ?? 0
			};
		}

		public new ICommand EditCommand => new RelayCommand(OnEdit, CanEdit);
		private bool CanEdit()
		{
			return _hasEditorPermission;
		}

		protected override async Task<bool> SaveItemAsync(CompanyUserModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.InfoMessages, "SavingCompanyUser");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _companyUserService.UpdateCompanyUserAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.InfoMessages, "CompanyUserHasBeenSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("CompanyUser", "Save", "Company User saved successfully", $"Company User #{model.CompanyID}, {model.User.FullName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingCompanyUser0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("CompanyUser", "Save", ex);
				return false;
			}
		}

		public new ICommand DeleteCommand => new RelayCommand(OnDelete, CanDelete);
		protected override async Task<bool> DeleteItemAsync(CompanyUserModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingCompanyUser");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _companyUserService.DeleteCompanyUserAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.InfoMessages, "CompanyUserHasBeenDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("CompanyUser", "Delete", "Company User deleted", $"Company User #{model.CompanyID}, {model.User.FullName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingCompanyUser0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("CompanyUser", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentCompanyUser");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}
		private bool CanDelete()
		{
			return _hasEditorPermission;
		}

		override protected IEnumerable<IValidationConstraint<CompanyUserModel>> GetValidationConstraints(CompanyUserModel model)
		{
			string propertyUser = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyUser");
			var requiredUser = new RequiredGreaterThanZeroConstraint<CompanyUserModel>(propertyUser, m => m.UserID);
			requiredUser.SetResourceService(ResourceService);

			yield return requiredUser;
		}

		#region Handle external messages
		private async void OnDetailsMessage(CompanyUserDetailsViewModel sender, string message, CompanyUserModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.CompanyUserID == current?.CompanyUserID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _companyUserService.GetCompanyUserAsync(current.CompanyUserID);
									item ??= new CompanyUserModel { CompanyUserID = CompanyUserID, CompanyID = CompanyID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.Warnings, "ThisCompanyUserHasBeenModifiedExternally");
										StatusMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("CompanyUser", "Handle Changes", ex);
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

		private async void OnListMessage(CompanyUserListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<CompanyUserModel> deletedModels)
						{
							if (deletedModels.Any(r => r.CompanyUserID == current.CompanyUserID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _companyUserService.GetCompanyUserAsync(current.CompanyUserID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("CompanyUser", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<CompanyUserDetailsViewModel>(ResourceFiles.Warnings, "ThisCompanyUserHasBeenDeletedExternally");
				StatusMessage(title, message);
			});
		}
		#endregion
	}
}
