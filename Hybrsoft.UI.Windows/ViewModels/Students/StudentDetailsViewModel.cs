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
	public partial class StudentDetailsViewModel(IStudentService studentService, IFilePickerService filePickerService, ICommonServices commonServices) : GenericDetailsViewModel<StudentModel>(commonServices)
	{
		public IStudentService StudentService { get; } = studentService;

		public IFilePickerService FilePickerService { get; } = filePickerService;

		public override string Title => ItemIsNew
				? ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.UI, "TitleNew")
				: Item.FullName;

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public StudentDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(StudentDetailsArgs args)
		{
			ViewModelArgs = args ?? StudentDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new StudentModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await StudentService.GetStudentAsync(ViewModelArgs.StudentID);
					Item = item ?? new StudentModel { StudentID = ViewModelArgs.StudentID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("Student", "Load", ex);
				}
			}
			NotifyPropertyChanged(nameof(ItemIsNew));
		}

		public void Unload()
		{
			ViewModelArgs.StudentID = Item?.StudentID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<StudentDetailsViewModel, StudentModel>(this, OnDetailsMessage);
			MessageService.Subscribe<StudentListViewModel>(this, OnListMessage);
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

		protected override async Task<bool> SaveItemAsync(StudentModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.InfoMessages, "SavingStudent");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await StudentService.UpdateStudentAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.InfoMessages, "StudentSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("Student", "Save", "Student saved successfully", $"Student {model.StudentID} '{model.FullName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingStudent0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Student", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(StudentModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingStudent");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await StudentService.DeleteStudentAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.InfoMessages, "StudentDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("Student", "Delete", "Student deleted", $"Student {model.StudentID} '{model.FullName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingStudent0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Student", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentStudent");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<StudentModel>> GetValidationConstraints(StudentModel model)
		{
			string propertyFirstName = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyFirstName");
			var requiredFirstName = new RequiredConstraint<StudentModel>(propertyFirstName, m => m.FirstName);
			requiredFirstName.SetResourceService(ResourceService);

			string propertyLastName = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyLastName");
			var requiredLastName = new RequiredConstraint<StudentModel>(propertyLastName, m => m.LastName);
			requiredLastName.SetResourceService(ResourceService);

			string propertyEmail = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyEmail");
			var requiredEmail = new RequiredConstraint<StudentModel>(propertyEmail, m => m.Email);
			requiredEmail.SetResourceService(ResourceService);
			var emailIsValid = new EmailValidationConstraint<StudentModel>(propertyEmail, m => m.Email);
			emailIsValid.SetResourceService(ResourceService);

			yield return requiredFirstName;
			yield return requiredLastName;
			yield return requiredEmail;
			yield return emailIsValid;
		}

		#region Handle external messages
		private async void OnDetailsMessage(StudentDetailsViewModel sender, string message, StudentModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.StudentID == current?.StudentID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await StudentService.GetStudentAsync(current.StudentID);
									item ??= new StudentModel { StudentID = current.StudentID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.Warnings, "ThisStudentHasBeenModifiedExternally");
										WarningMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("Student", "Handle Changes", ex);
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

		private async void OnListMessage(StudentListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<StudentModel> deletedModels)
						{
							if (deletedModels.Any(r => r.StudentID == current.StudentID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await StudentService.GetStudentAsync(current.StudentID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Student", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<StudentDetailsViewModel>(ResourceFiles.Warnings, "ThisStudentHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
