using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class ClassroomStudentDetailsViewModel(IClassroomService classroomService, IClassroomStudentService classroomStudentService, ICommonServices commonServices) : GenericDetailsViewModel<ClassroomStudentModel>(commonServices)
	{
		public IClassroomService ClassroomService { get; } = classroomService;
		public IClassroomStudentService ClassroomStudentService { get; } = classroomStudentService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew => string.Format(ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.UI, "TitleNew"), ClassroomID);
		public string TitleEdit => string.Format(ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.UI, "TitleEdit"), Item?.ClassroomStudentID, Item?.ClassroomID);

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public ClassroomStudentDetailsArgs ViewModelArgs { get; private set; }

		public long ClassroomStudentID { get; set; }
		public long ClassroomID { get; set; }
		private IList<long> _addedStudentKeys;
		public IList<long> AddedStudentKeys
		{
			get => _addedStudentKeys;
			set
			{
				_addedStudentKeys = value;
				NotifyPropertyChanged(nameof(AddedStudentKeys));
			}
		}

		public ICommand StudentSelectedCommand => new RelayCommand<StudentModel>(StudentSelected);
		private void StudentSelected(StudentModel model)
		{
			EditableItem.StudentID = model?.StudentID ?? 0;
			EditableItem.Student = model;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(ClassroomStudentDetailsArgs args)
		{
			ViewModelArgs = args ?? ClassroomStudentDetailsArgs.CreateDefault();
			ClassroomStudentID = ViewModelArgs.ClassroomStudentID;
			ClassroomID = ViewModelArgs.ClassroomID;
			var classroom = await ClassroomService.GetClassroomAsync(ClassroomID);
			AddedStudentKeys = await ClassroomStudentService.GetAddedStudentKeysInClassroomAsync(ClassroomID);

			if (ViewModelArgs.IsNew)
			{
				Item = new ClassroomStudentModel() { ClassroomID = ClassroomID, Classroom = classroom};
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await ClassroomStudentService.GetClassroomStudentAsync(ClassroomStudentID);
					Item = item ?? new ClassroomStudentModel() { ClassroomStudentID = ClassroomStudentID, ClassroomID = ClassroomID, IsEmpty = true };
					Item.Classroom = classroom;
				}
				catch (Exception ex)
				{
					LogException("ClassroomStudent", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.ClassroomID = ClassroomID;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<ClassroomStudentDetailsViewModel, ClassroomStudentModel>(this, OnDetailsMessage);
			MessageService.Subscribe<ClassroomStudentListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public ClassroomStudentDetailsArgs CreateArgs()
		{
			return new ClassroomStudentDetailsArgs
			{
				ClassroomStudentID = Item?.ClassroomStudentID ?? 0,
				ClassroomID = Item?.ClassroomID ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(ClassroomStudentModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.InfoMessages, "SavingStudentInTheClassroom");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await ClassroomStudentService.UpdateClassroomStudentAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "SaveSuccessful");
				string endMessage = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.InfoMessages, "StudentInTheClassroomHasBeenSaved");
				EndStatusMessage(endTitle, endMessage, LogType.Success);
				LogSuccess("ClassroomStudent", "Save", "Student in the classroom saved successfully", $"Student in the classroom #{model.ClassroomID}, {model.Student.FullName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "SaveFailed");
				string message = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.Errors, "ErrorSavingStudentInTheClassroom0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("ClassroomStudent", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(ClassroomStudentModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingStudentFromClassroom");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await ClassroomStudentService.DeleteClassroomStudentAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.InfoMessages, "StudentInTheClassroomHasBeenDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				LogWarning("ClassroomStudent", "Delete", "Student in the classroom deleted", $"Student in the classroom #{model.ClassroomID}, {model.Student.FullName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingStudentFromTheClassroom0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("ClassroomStudent", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentStudentFromTheClassroom");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<ClassroomStudentModel>> GetValidationConstraints(ClassroomStudentModel model)
		{
			string propertyStudent = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.ValidationErrors, "PropertyStudent");
			var requiredStudent = new RequiredGreaterThanZeroConstraint<ClassroomStudentModel>(propertyStudent, m => m.StudentID);
			requiredStudent.SetResourceService(ResourceService);

			yield return requiredStudent;
		}

		#region Handle external messages
		private async void OnDetailsMessage(ClassroomStudentDetailsViewModel sender, string message, ClassroomStudentModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.ClassroomStudentID == current?.ClassroomStudentID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await ClassroomStudentService.GetClassroomStudentAsync(current.ClassroomStudentID);
									item ??= new ClassroomStudentModel { ClassroomStudentID = ClassroomStudentID, ClassroomID = ClassroomID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalModification");
										string message = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.Warnings, "ThisClassroomStudentHasBeenModifiedExternally");
										StatusMessage(title, message);
									}
								}
								catch (Exception ex)
								{
									LogException("ClassroomStudent", "Handle Changes", ex);
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

		private async void OnListMessage(ClassroomStudentListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<ClassroomStudentModel> deletedModels)
						{
							if (deletedModels.Any(r => r.ClassroomStudentID == current.ClassroomStudentID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await ClassroomStudentService.GetClassroomStudentAsync(current.ClassroomStudentID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("ClassroomStudent", "Handle Ranges Deleted", ex);
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
				string message = ResourceService.GetString<ClassroomStudentDetailsViewModel>(ResourceFiles.Warnings, "ThisClassroomStudentHasBeenDeletedExternally");
				StatusMessage(title, message);
			});
		}
		#endregion
	}
}
