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
	public partial class ClassroomStudentDetailsViewModel(IClassroomService classroomService, IClassroomStudentService classroomStudentService, ICommonServices commonServices) : GenericDetailsViewModel<ClassroomStudentDto>(commonServices)
	{
		public IClassroomService ClassroomService { get; } = classroomService;
		public IClassroomStudentService ClassroomStudentService { get; } = classroomStudentService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew
		{
			get
			{
				string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_TitleNew");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, ClassroomID);
				return message;
			}
		}
		public string TitleEdit
		{
			get
			{
				string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_TitleEdit");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, Item?.ClassroomStudentID, Item?.ClassroomID);
				return message ?? String.Empty;
			}
		}

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

		public ICommand StudentSelectedCommand => new RelayCommand<StudentDto>(StudentSelected);
		private void StudentSelected(StudentDto model)
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
				Item = new ClassroomStudentDto() { ClassroomID = ClassroomID, Classroom = classroom};
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await ClassroomStudentService.GetClassroomStudentAsync(ClassroomStudentID);
					Item = item ?? new ClassroomStudentDto() { ClassroomStudentID = ClassroomStudentID, ClassroomID = ClassroomID, IsEmpty = true };
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
			MessageService.Subscribe<ClassroomStudentDetailsViewModel, ClassroomStudentDto>(this, OnDetailsMessage);
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

		protected override async Task<bool> SaveItemAsync(ClassroomStudentDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentDetailsViewModel), "_SavingStudentInTheClassroom"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await ClassroomStudentService.UpdateClassroomStudentAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentDetailsViewModel), "_StudentInTheClassroomHasBeenSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("ClassroomStudent", "Save", "Student in the classroom saved successfully", $"Student in the classroom #{model.ClassroomID}, {model.Student.FullName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_ErrorSavingStudentInTheClassroom0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("ClassroomStudent", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(ClassroomStudentDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentDetailsViewModel), "_DeletingStudentFromClassroom"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await ClassroomStudentService.DeleteClassroomStudentAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentDetailsViewModel), "_StudentInTheClassroomHasBeenDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("ClassroomStudent", "Delete", "Student in the classroom deleted", $"Student in the classroom #{model.ClassroomID}, {model.Student.FullName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_ErrorDeletingStudentFromTheClassroom0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("ClassroomStudent", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(ClassroomStudentDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentStudentFromTheClassroom"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<ClassroomStudentDto>> GetValidationConstraints(ClassroomStudentDto model)
		{
			string resourceKeyForStudent = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_PropertyStudent");
			string propertyStudent = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForStudent);
			var requiredStudent = new RequiredGreaterThanZeroConstraint<ClassroomStudentDto>(propertyStudent, m => m.StudentID);
			requiredStudent.SetResourceService(ResourceService);

			yield return requiredStudent;
		}

		#region Handle external messages
		private async void OnDetailsMessage(ClassroomStudentDetailsViewModel sender, string message, ClassroomStudentDto changed)
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
									item ??= new ClassroomStudentDto { ClassroomStudentID = ClassroomStudentID, ClassroomID = ClassroomID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_ThisClassroomStudentHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										StatusMessage(message);
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
						if (args is IList<ClassroomStudentDto> deletedModels)
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
				string resourceKey = string.Concat(nameof(ClassroomStudentDetailsViewModel), "_ThisClassroomStudentHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				StatusMessage(message);
			});
		}
		#endregion
	}
}
