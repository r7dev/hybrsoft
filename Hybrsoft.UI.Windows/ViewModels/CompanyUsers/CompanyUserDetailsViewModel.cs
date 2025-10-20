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
	public partial class CompanyUserDetailsViewModel(ICompanyService companyService, ICompanyUserService companyUserService, ICommonServices commonServices) : GenericDetailsViewModel<CompanyUserModel>(commonServices)
	{
		public ICompanyService CompanyService { get; } = companyService;
		public ICompanyUserService CompanyUserService { get; } = companyUserService;

		private bool _hasEditorPermission;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew
		{
			get
			{
				string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_TitleNew";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, CompanyID);
				return message;
			}
		}
		public string TitleEdit
		{
			get
			{
				string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_TitleEdit";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, Item?.CompanyUserID, Item?.CompanyID);
				return message ?? String.Empty;
			}
		}

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
			var company = await CompanyService.GetCompanyAsync(CompanyID);
			AddedUserKeys = await CompanyUserService.GetAddedUserKeysInCompanyAsync(CompanyID);
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
					var item = await CompanyUserService.GetCompanyUserAsync(CompanyUserID);
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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(CompanyUserDetailsViewModel)}_SavingCompanyUser");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await CompanyUserService.UpdateCompanyUserAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(CompanyUserDetailsViewModel)}_CompanyUserHasBeenSaved");
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("CompanyUser", "Save", "Company User saved successfully", $"Company User #{model.CompanyID}, {model.User.FullName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_ErrorSavingCompanyUser0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("CompanyUser", "Save", ex);
				return false;
			}
		}

		public new ICommand DeleteCommand => new RelayCommand(OnDelete, CanDelete);
		protected override async Task<bool> DeleteItemAsync(CompanyUserModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(CompanyUserDetailsViewModel)}_DeletingCompanyUser");
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await CompanyUserService.DeleteCompanyUserAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), $"{nameof(CompanyUserDetailsViewModel)}_CompanyUserHasBeenDeleted");
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("CompanyUser", "Delete", "Company User deleted", $"Company User #{model.CompanyID}, {model.User.FullName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_ErrorDeletingCompanyUser0";
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("CompanyUser", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), $"{nameof(CompanyUserDetailsViewModel)}_AreYouSureYouWantToDeleteCurrentCompanyUser");
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}
		private bool CanDelete()
		{
			return _hasEditorPermission;
		}

		override protected IEnumerable<IValidationConstraint<CompanyUserModel>> GetValidationConstraints(CompanyUserModel model)
		{
			string resourceKeyForUser = $"{nameof(CompanyUserDetailsViewModel)}_PropertyUser";
			string propertyUser = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForUser);
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
									var item = await CompanyUserService.GetCompanyUserAsync(current.CompanyUserID);
									item ??= new CompanyUserModel { CompanyUserID = CompanyUserID, CompanyID = CompanyID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_ThisCompanyUserHasBeenModifiedExternally";
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										StatusMessage(message);
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
							var model = await CompanyUserService.GetCompanyUserAsync(current.CompanyUserID);
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
				string resourceKey = $"{nameof(CompanyUserDetailsViewModel)}_ThisCompanyUserHasBeenDeletedExternally";
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				StatusMessage(message);
			});
		}
		#endregion
	}
}
