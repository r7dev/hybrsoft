using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class LostAndFoundDetailsViewModel(ILostAndFoundService lostAndFoundService,
		IFilePickerService filePickerService,
		ICommonServices commonServices) : GenericDetailsViewModel<LostAndFoundModel>(commonServices)
	{
		private readonly ILostAndFoundService _lostAndFoundService = lostAndFoundService;
		private readonly IFilePickerService _filePickerService = filePickerService;

		public override string Title => ItemIsNew
				? ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.UI, "TitleNew")
				: Item.DisplayName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public LostAndFoundDetailsArgs ViewModelArgs { get; private set; }

		public ICommand LostAndFoundStatusSelectedCommand => new RelayCommand<LostAndFoundStatusModel>(LostAndFoundStatusSelected);
		private void LostAndFoundStatusSelected(LostAndFoundStatusModel lostAndFoundStatus)
		{
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(LostAndFoundDetailsArgs args)
		{
			ViewModelArgs = args ?? LostAndFoundDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new LostAndFoundModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _lostAndFoundService.GetLostAndFoundAsync(ViewModelArgs.LostAndFoundID);
					Item = item ?? new LostAndFoundModel { LostAndFoundID = ViewModelArgs.LostAndFoundID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("LostAndFound", "Load", ex);
				}
			}
			await Task.Delay(200);
			EditableItem.NotifyChanges();
		}

		public void Unload()
		{
			ViewModelArgs.LostAndFoundID = Item?.LostAndFoundID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<LostAndFoundDetailsViewModel, LostAndFoundModel>(this, OnDetailsMessage);
			MessageService.Subscribe<LostAndFoundListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		private object _newPictureSource = null;
		public object NewPictureSource
		{
			get => _newPictureSource;
			set => Set(ref _newPictureSource, value);
		}

		public override void BeginEdit()
		{
			NewPictureSource = null;
			base.BeginEdit();
		}

		public ICommand EditPictureCommand => new RelayCommand(OnEditPicture);
		protected virtual void OnEditPicture()
		{
			EditPictureAsync();
		}
		public virtual async void EditPictureAsync()
		{
			NewPictureSource = null;
			var result = await _filePickerService.OpenImagePickerAsync();
			if (result != null)
			{
				EditableItem.Picture = result.ImageBytes;
				EditableItem.PictureSource = result.ImageSource;
				EditableItem.Thumbnail = result.ThumbnailBytes;
				EditableItem.ThumbnailSource = result.ThumbnailSource;
				NewPictureSource = result.ImageSource;
			}
			else
			{
				NewPictureSource = null;
			}
		}

		protected override async Task<bool> SaveItemAsync(LostAndFoundModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.InfoMessages, "SavingLostAndFound");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _lostAndFoundService.UpdateLostAndFoundAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.InfoMessages, "LostAndFoundSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("LostAndFound", "Save", "LostAndFound saved successfully", $"LostAndFound {model.LostAndFoundID} '{model.DisplayName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingLostAndFound0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("LostAndFound", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(LostAndFoundModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingLostAndFound");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _lostAndFoundService.DeleteLostAndFoundAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.InfoMessages, "LostAndFoundDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("LostAndFound", "Delete", "LostAndFound deleted", $"LostAndFound {model.LostAndFoundID} '{model.DisplayName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingLostAndFound0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("LostAndFound", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentLostAndFound");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<LostAndFoundModel>> GetValidationConstraints(LostAndFoundModel model)
		{
			string propertyDisplayName = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDisplayName");
			var requiredDisplayName = new RequiredConstraint<LostAndFoundModel>(propertyDisplayName, m => m.DisplayName);
			requiredDisplayName.SetResourceService(ResourceService);

			string propertyDescription = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDescription");
			var requiredDescription = new RequiredConstraint<LostAndFoundModel>(propertyDescription, m => m.Description);
			requiredDescription.SetResourceService(ResourceService);

			string propertyPicture = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyPicture");
			var requiredPicture = new PictureValidationConstraint<LostAndFoundModel>(propertyPicture, m => m.Picture);
			requiredPicture.SetResourceService(ResourceService);

			string propertyDonationDate = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDonationDate");
			var requiredDonationDateIfDonated = new RequiredIfConstraint<LostAndFoundModel>(propertyDonationDate, m => m.DonationDate, m => m.Status == LostAndFoundStatus.Donated);
			requiredDonationDateIfDonated.SetResourceService(ResourceService);

			yield return requiredDisplayName;
			yield return requiredDescription;
			yield return requiredPicture;
			yield return requiredDonationDateIfDonated;
		}

		#region Handle external messages
		private async void OnDetailsMessage(LostAndFoundDetailsViewModel sender, string message, LostAndFoundModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.LostAndFoundID == current?.LostAndFoundID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _lostAndFoundService.GetLostAndFoundAsync(current.LostAndFoundID);
									item ??= new LostAndFoundModel { LostAndFoundID = current.LostAndFoundID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.Warnings, "ThisLostAndFoundHasBeenModifiedExternally");
										WarningMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("LostAndFound", "Handle Changes", ex);
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

		private async void OnListMessage(LostAndFoundListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<LostAndFoundModel> deletedModels)
						{
							if (deletedModels.Any(r => r.LostAndFoundID == current.LostAndFoundID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _lostAndFoundService.GetLostAndFoundAsync(current.LostAndFoundID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("LostAndFound", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<LostAndFoundDetailsViewModel>(ResourceFiles.Warnings, "ThisLostAndFoundHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
