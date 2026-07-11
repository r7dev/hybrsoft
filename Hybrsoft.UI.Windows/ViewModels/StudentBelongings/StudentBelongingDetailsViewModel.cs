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
	public partial class StudentBelongingDetailsViewModel(IStudentBelongingService studentBelongingService,
		IFilePickerService filePickerService,
		ICommonServices commonServices) : GenericDetailsViewModel<StudentBelongingModel>(commonServices)
	{
		private readonly IStudentBelongingService _studentBelongingService = studentBelongingService;
		private readonly IFilePickerService _filePickerService = filePickerService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => string.Format(ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.UI, "TitleNew"), StudentID);
		public string TitleEdit => string.Format(ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.UI, "TitleEdit"), Item?.StudentBelongingID, Item?.StudentID);

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public StudentBelongingDetailsArgs ViewModelArgs { get; private set; }

		public long StudentBelongingID { get; set; }
		public long StudentID { get; set; }

		public async Task LoadAsync(StudentBelongingDetailsArgs args)
		{
			ViewModelArgs = args ?? StudentBelongingDetailsArgs.CreateDefault();
			StudentBelongingID = ViewModelArgs.StudentBelongingID;
			StudentID = ViewModelArgs.StudentID;

			if (ViewModelArgs.IsNew)
			{
				Item = new StudentBelongingModel() { StudentID = StudentID };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await _studentBelongingService.GetStudentBelongingAsync(StudentBelongingID);
					Item = item ?? new StudentBelongingModel() { StudentBelongingID = StudentBelongingID, StudentID = StudentID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("StudentBelonging", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.StudentID = StudentID;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<StudentBelongingDetailsViewModel, StudentBelongingModel>(this, OnDetailsMessage);
			MessageService.Subscribe<StudentBelongingListViewModel>(this, OnListMessage);
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

		protected override async Task<bool> SaveItemAsync(StudentBelongingModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.InfoMessages, "SavingStudentsBelonging");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _studentBelongingService.UpdateStudentBelongingAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.InfoMessages, "TheStudentsBelongingWasSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("StudentBelonging", "Save", "Student's belonging saved successfully", $"Student's belonging #{model.StudentID}, {model.DisplayName} was saved successfully.");
				await SaveEmbeddingAsync(model);
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingStudentsBelonging0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("StudentBelonging", "Save", ex);
				return false;
			}
		}

		private async Task SaveEmbeddingAsync(StudentBelongingModel model)
		{
			await ContextService.RunAsync(async () =>
			{
				try
				{
					await _studentBelongingService.UpdateStudentBelongingEmbeddingAsync(model);
				}
				catch (Exception ex)
				{
					LogException("StudentBelonging", "Save Embedding", ex);
				}
			});
		}

		protected override async Task<bool> DeleteItemAsync(StudentBelongingModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingStudentsBelonging");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await _studentBelongingService.DeleteStudentBelongingAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.InfoMessages, "StudentsBelongingWasDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("StudentBelonging", "Delete", "Student's belonging deleted", $"Student's belonging #{model.StudentID}, {model.DisplayName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingStudentsBelonging0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("StudentBelonging", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteTheStudentsCurrentBelonging");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<StudentBelongingModel>> GetValidationConstraints(StudentBelongingModel model)
		{
			string propertyDisplayName = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDisplayName");
			var requiredDisplayName = new RequiredConstraint<StudentBelongingModel>(propertyDisplayName, m => m.DisplayName);
			requiredDisplayName.SetResourceService(ResourceService);

			string propertyDescription = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyDescription");
			var requiredDescription = new RequiredConstraint<StudentBelongingModel>(propertyDescription, m => m.Description);
			requiredDescription.SetResourceService(ResourceService);

			string propertyPicture = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyPicture");
			var requiredPicture = new PictureValidationConstraint<StudentBelongingModel>(propertyPicture, m => m.Picture);
			requiredPicture.SetResourceService(ResourceService);

			yield return requiredDisplayName;
			yield return requiredDescription;
			yield return requiredPicture;
		}

		#region Handle external messages
		private async void OnDetailsMessage(StudentBelongingDetailsViewModel sender, string message, StudentBelongingModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.StudentBelongingID == current?.StudentBelongingID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await _studentBelongingService.GetStudentBelongingAsync(current.StudentBelongingID);
									item ??= new StudentBelongingModel { StudentBelongingID = StudentBelongingID, StudentID = StudentID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.Warnings, "ThisStudentsBelongingWasModifiedExternally");
										StatusMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("StudentBelonging", "Handle Changes", ex);
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

		private async void OnListMessage(StudentBelongingListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<StudentBelongingModel> deletedModels)
						{
							if (deletedModels.Any(r => r.StudentBelongingID == current.StudentBelongingID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await _studentBelongingService.GetStudentBelongingAsync(current.StudentBelongingID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("StudentBelonging", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<StudentBelongingDetailsViewModel>(ResourceFiles.Warnings, "ThisStudentsBelongingWasDeletedExternally");
				StatusMessage(title, message);
			});
		}
		#endregion
	}
}
