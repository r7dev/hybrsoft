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
	public partial class RelativeDetailsViewModel(IRelativeService relativeService,
		IFilePickerService filePickerService,
		ICommonServices commonServices) : GenericDetailsViewModel<RelativeModel>(commonServices)
	{
		private readonly IRelativeService _relativeService = relativeService;
		private readonly IFilePickerService _filePickerService = filePickerService;

		public override string Title => ItemIsNew
				? ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.UI, "TitleNew")
				: Item.FullName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RelativeDetailsArgs ViewModelArgs { get; private set; }

		public ICommand RelativeTypeSelectedCommand => new RelayCommand<RelativeTypeModel>(RelativeTypeSelected);
		private void RelativeTypeSelected(RelativeTypeModel relativeType)
		{
			EditableItem.RelativeType = relativeType;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(RelativeDetailsArgs args)
		{
			ViewModelArgs = args ?? RelativeDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new RelativeModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _relativeService.GetRelativeAsync(ViewModelArgs.RelativeID);
					Item = item ?? new RelativeModel { RelativeID = ViewModelArgs.RelativeID, IsEmpty = true };
					await Task.Delay(200);
					EditableItem.NotifyChanges();
				}
				catch (Exception ex)
				{
					LogException("Relative", "Load", ex);
				}
			}
		}

		public void Unload()
		{
			ViewModelArgs.RelativeID = Item?.RelativeID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<RelativeDetailsViewModel, RelativeModel>(this, OnDetailsMessage);
			MessageService.Subscribe<RelativeListViewModel>(this, OnListMessage);
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

		protected override async Task<bool> SaveItemAsync(RelativeModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.InfoMessages, "SavingRelative");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _relativeService.UpdateRelativeAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.InfoMessages, "RelativeSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("Relative", "Save", "Relative saved successfully", $"Relative {model.RelativeID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingRelative0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Relative", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RelativeModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingRelative");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _relativeService.DeleteRelativeAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.InfoMessages, "RelativeDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("Relative", "Delete", "Relative deleted", $"Relative {model.RelativeID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingRelative0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Relative", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentRelative");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<RelativeModel>> GetValidationConstraints(RelativeModel model)
		{
			string propertyFirstName = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyFirstName");
			var requiredFirstName = new RequiredConstraint<RelativeModel>(propertyFirstName, m => m.FirstName);
			requiredFirstName.SetResourceService(ResourceService);

			string propertyLastName = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyLastName");
			var requiredLastName = new RequiredConstraint<RelativeModel>(propertyLastName, m => m.LastName);
			requiredLastName.SetResourceService(ResourceService);

			string propertyRelativeType = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyRelativeType");
			var requiredRelativeType = new RequiredGreaterThanZeroConstraint<RelativeModel>(propertyRelativeType, m => m.RelativeTypeID);
			requiredRelativeType.SetResourceService(ResourceService);

			yield return requiredFirstName;
			yield return requiredLastName;
			yield return requiredRelativeType;
		}

		#region Handle external messages
		private async void OnDetailsMessage(RelativeDetailsViewModel sender, string message, RelativeModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.RelativeID == current?.RelativeID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _relativeService.GetRelativeAsync(current.RelativeID);
									item ??= new RelativeModel { RelativeID = current.RelativeID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.Warnings, "ThisRelativeHasBeenModifiedExternally");
										WarningMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("Relative", "Handle Changes", ex);
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

		private async void OnListMessage(RelativeListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<RelativeModel> deletedModels)
						{
							if (deletedModels.Any(r => r.RelativeID == current.RelativeID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _relativeService.GetRelativeAsync(current.RelativeID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Relative", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<RelativeDetailsViewModel>(ResourceFiles.Warnings, "ThisRelativeHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
