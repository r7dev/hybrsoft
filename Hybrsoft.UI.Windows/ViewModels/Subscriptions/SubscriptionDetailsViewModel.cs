using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class SubscriptionDetailsViewModel : GenericDetailsViewModel<SubscriptionModel>
	{
		public SubscriptionDetailsViewModel(ISubscriptionService subscriptionService,
			ICommonServices commonServices) : base(commonServices)
		{
			_subscriptionService = subscriptionService;
			_cancelSecondaryCommand = new RelayCommand(OnCancelSecondary, CanCancelSecondary);
		}

		private readonly ISubscriptionService _subscriptionService;
		private readonly RelayCommand _cancelSecondaryCommand;
		private bool _hasEditorPermission;

		public override string Title => ItemIsNew
				? ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.UI, "TitleNew")
				: Item.FullName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public SubscriptionDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(SubscriptionDetailsArgs args)
		{
			ViewModelArgs = args ?? SubscriptionDetailsArgs.CreateDefault();
			_hasEditorPermission = AuthorizationService.HasPermission(Permissions.SubscriptionEditor);

			if (ViewModelArgs.IsNew)
			{
				Item = new SubscriptionModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _subscriptionService.GetSubscriptionAsync(ViewModelArgs.SubscriptionID);
					Item = item ?? new SubscriptionModel { SubscriptionID = ViewModelArgs.SubscriptionID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("Subscription", "Load", ex);
				}
			}
			_cancelSecondaryCommand.OnCanExecuteChanged();
		}

		public void Unload()
		{
			ViewModelArgs.SubscriptionID = Item?.SubscriptionID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<SubscriptionDetailsViewModel, SubscriptionModel>(this, OnDetailsMessage);
			MessageService.Subscribe<SubscriptionListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public ICommand SubscriptionPlanSelectedCommand => new RelayCommand<SubscriptionPlanModel>(SubscriptionPlanSelected);
		private void SubscriptionPlanSelected(SubscriptionPlanModel subscriptionPlan)
		{
			if (subscriptionPlan?.DurationMonths == 12)
			{
				EditableItem.DurationDays = 365;
			}
			else
			{
				EditableItem.DurationDays = (short)(subscriptionPlan?.DurationMonths * 30 ?? 0);
			}
			EditableItem.NotifyChanges();
		}

		public ICommand SubscriptionTypeSelectedCommand => new RelayCommand<SubscriptionTypeModel>(SubscriptionTypeSelected);
		private void SubscriptionTypeSelected(SubscriptionTypeModel subscriptionType)
		{
			EditableItem.SubscriptionType = subscriptionType;
			EditableItem.NotifyChanges();
		}

		public ICommand CompanySelectedCommand => new RelayCommand<CompanyModel>(CompanySelected);
		private void CompanySelected(CompanyModel model)
		{
			EditableItem.CompanyID = model?.CompanyID ?? 0;
			EditableItem.Company = model;
			EditableItem.NotifyChanges();
		}

		public ICommand UserSelectedCommand => new RelayCommand<UserModel>(UserSelected);
		private void UserSelected(UserModel model)
		{
			EditableItem.UserID = model?.UserID ?? 0;
			EditableItem.User = model;
			EditableItem.NotifyChanges();
		}

		public ICommand CancelSecondaryCommand => _cancelSecondaryCommand;
		private async void OnCancelSecondary()
		{
			if (await ConfirmCancellationAsync())
			{
				EditableItem.CancelledOn = DateTimeOffset.Now;
				await base.SaveAsync();
			}
		}
		private async Task<bool> ConfirmCancellationAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmCancellation");
			string content = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToCancelCurrentSubscription");
			string confirm = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Confirm");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, confirm, cancel);
		}
		private bool CanCancelSecondary()
		{
			return _hasEditorPermission
				&& !ItemIsNew
				&& EditableItem.CancelledOn == null
				&& EditableItem.Status != SubscriptionStatus.Expired;
		}

		public ICommand CopyDescriptionCommand => new RelayCommand(OnCopyDescription);
		virtual protected void OnCopyDescription()
		{
			CopyDescriptionAsync();
		}
		virtual public void CopyDescriptionAsync()
		{
			DataPackage dataPackage = new()
			{
				RequestedOperation = DataPackageOperation.Copy
			};
			dataPackage.SetText(Item.LicenseKey);
			Clipboard.SetContent(dataPackage);
		}

		public new ICommand EditCommand => new RelayCommand(OnEdit, CanEdit);
		public override void BeginEdit()
		{
			base.BeginEdit();
		}
		private bool CanEdit()
		{
			return _hasEditorPermission;
		}

		protected override async Task<bool> SaveItemAsync(SubscriptionModel model)
		{
			try
			{
				if (EditableItem.Type == SubscriptionType.Enterprise)
				{
					EditableItem.UserID = 0;
					EditableItem.User = null;
				}
				else if (EditableItem.Type == SubscriptionType.Individual)
				{
					EditableItem.CompanyID = 0;
					EditableItem.Company = null;
				}
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.InfoMessages, "SavingSubscription");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _subscriptionService.UpdateSubscriptionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.InfoMessages, "SubscriptionSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("Subscription", "Save", "Subscription saved successfully", $"Subscription {model.SubscriptionID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingSubscription0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Subscription", "Save", ex);
				return false;
			}
		}

		public new ICommand DeleteCommand => new RelayCommand(OnDelete, CanDelete);
		protected override async Task<bool> DeleteItemAsync(SubscriptionModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingSubscription");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _subscriptionService.DeleteSubscriptionAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.InfoMessages, "SubscriptionDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("Subscription", "Delete", "Subscription deleted", $"Subscription {model.SubscriptionID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingSubscription0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Subscription", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentSubscription");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}
		private bool CanDelete()
		{
			return _hasEditorPermission;
		}

		override protected IEnumerable<IValidationConstraint<SubscriptionModel>> GetValidationConstraints(SubscriptionModel model)
		{
			string propertySubscriptionPlan = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertySubscriptionPlan");
			var requiredSubscriptionPlan = new RequiredGreaterThanZeroConstraint<SubscriptionModel>(propertySubscriptionPlan, m => m.SubscriptionPlanID);
			requiredSubscriptionPlan.SetResourceService(ResourceService);
			yield return requiredSubscriptionPlan;

			if (model.Type == SubscriptionType.Enterprise)
			{
				string propertyCompany = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyCompany");
				var requiredCompany = new RequiredGreaterThanZeroConstraint<SubscriptionModel>(propertyCompany, m => m.CompanyID);
				requiredCompany.SetResourceService(ResourceService);

				yield return requiredCompany;
			}
			else if (model.Type == SubscriptionType.Individual)
			{
				string propertyUser = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyUser");
				var requiredUser = new RequiredGreaterThanZeroConstraint<SubscriptionModel>(propertyUser, m => m.UserID);
				requiredUser.SetResourceService(ResourceService);

				yield return requiredUser;
			}
		}

		#region Handle external messages
		private async void OnDetailsMessage(SubscriptionDetailsViewModel sender, string message, SubscriptionModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.SubscriptionID == current?.SubscriptionID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _subscriptionService.GetSubscriptionAsync(current.SubscriptionID);
									item ??= new SubscriptionModel { SubscriptionID = current.SubscriptionID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Warnings, "ThisSubscriptionHasBeenModifiedExternally");
										WarningMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("Subscription", "Handle Changes", ex);
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

		private async void OnListMessage(SubscriptionListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<SubscriptionModel> deletedModels)
						{
							if (deletedModels.Any(r => r.SubscriptionID == current.SubscriptionID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _subscriptionService.GetSubscriptionAsync(current.SubscriptionID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Subscription", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<SubscriptionDetailsViewModel>(ResourceFiles.Warnings, "ThisSubscriptionHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
