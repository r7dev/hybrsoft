using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class RelativeDetailsViewModel(IRelativeService relativeService, IFilePickerService filePickerService, ICommonServices commonServices) : GenericDetailsViewModel<RelativeDto>(commonServices)
	{
		public IRelativeService RelativeService { get; } = relativeService;

		public IFilePickerService FilePickerService { get; } = filePickerService;

		public override string Title
		{
			get
			{
				if (Item?.IsNew ?? true)
				{
					string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_Title");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return TitleEdit;
			}
		}
		public string TitleEdit
		{
			get
			{
				if (Item == null)
				{
					string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_TitleEdit");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return $"{Item.FullName}";
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public RelativeDetailsArgs ViewModelArgs { get; private set; }

		public ICommand RelativeTypeSelectedCommand => new RelayCommand<RelativeTypeDto>(RelativeTypeSelected);
		private void RelativeTypeSelected(RelativeTypeDto relativeType)
		{
			EditableItem.RelativeType = relativeType;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(RelativeDetailsArgs args)
		{
			ViewModelArgs = args ?? RelativeDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new RelativeDto();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await RelativeService.GetRelativeAsync(ViewModelArgs.RelativeID);
					Item = item ?? new RelativeDto { RelativeID = ViewModelArgs.RelativeID, IsEmpty = true };
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
			MessageService.Subscribe<RelativeDetailsViewModel, RelativeDto>(this, OnDetailsMessage);
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
			var result = await FilePickerService.OpenImagePickerAsync();
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

		protected override async Task<bool> SaveItemAsync(RelativeDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RelativeDetailsViewModel), "_SavingRelative"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RelativeService.UpdateRelativeAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RelativeDetailsViewModel), "_RelativeSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Relative", "Save", "Relative saved successfully", $"Relative {model.RelativeID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_ErrorSavingRelative0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Relative", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(RelativeDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RelativeDetailsViewModel), "_DeletingRelative"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await RelativeService.DeleteRelativeAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(RelativeDetailsViewModel), "_RelativeDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("Relative", "Delete", "Relative saved successfully", $"Relative {model.RelativeID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_ErrorDeletingRelative0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Relative", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(RelativeDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentRelative"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<RelativeDto>> GetValidationConstraints(RelativeDto model)
		{
			string resourceKeyForFirstName = string.Concat(nameof(RelativeDetailsViewModel), "_PropertyFirstName");
			string propertyFirstName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForFirstName);
			var requiredFirstName = new RequiredConstraint<RelativeDto>(propertyFirstName, m => m.FirstName);
			requiredFirstName.SetResourceService(ResourceService);

			string resourceKeyForLastName = string.Concat(nameof(RelativeDetailsViewModel), "_PropertyLastName");
			string propertyLastName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForLastName);
			var requiredLastName = new RequiredConstraint<RelativeDto>(propertyLastName, m => m.LastName);
			requiredLastName.SetResourceService(ResourceService);

			string resourceKeyForRelativeType = string.Concat(nameof(RelativeDetailsViewModel), "_PropertyRelativeType");
			string propertyRelativeType = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForRelativeType);
			var requiredRelativeType = new RequiredGreaterThanZeroConstraint<RelativeDto>(propertyRelativeType, m => m.RelativeTypeID);
			requiredRelativeType.SetResourceService(ResourceService);

			yield return requiredFirstName;
			yield return requiredLastName;
			yield return requiredRelativeType;
		}

		#region Handle external messages
		private async void OnDetailsMessage(RelativeDetailsViewModel sender, string message, RelativeDto changed)
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
									var item = await RelativeService.GetRelativeAsync(current.RelativeID);
									item ??= new RelativeDto { RelativeID = current.RelativeID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_ThisRelativeHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
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
						if (args is IList<RelativeDto> deletedModels)
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
							var model = await RelativeService.GetRelativeAsync(current.RelativeID);
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
				string resourceKey = string.Concat(nameof(RelativeDetailsViewModel), "_ThisRelativeHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
