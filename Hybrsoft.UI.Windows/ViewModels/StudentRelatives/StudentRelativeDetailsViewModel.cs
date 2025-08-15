using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class StudentRelativeDetailsViewModel(IStudentRelativeService studentRelativeService, ICommonServices commonServices) : GenericDetailsViewModel<StudentRelativeModel>(commonServices)
	{
		IStudentRelativeService StudentRelativeService { get; } = studentRelativeService;

		public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
		public string TitleNew
		{
			get
			{
				string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_TitleNew");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, StudentID);
				return message;
			}
		}
		public string TitleEdit
		{
			get
			{
				string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_TitleEdit");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
				string message = string.Format(resourceValue, Item?.StudentRelativeID, Item?.StudentID);
				return message ?? String.Empty;
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public StudentRelativeDetailsArgs ViewModelArgs { get; private set; }

		public long StudentRelativeID { get; set; }
		public long StudentID { get; set; }
		private IList<long> _addedRelativeKeys;
		public IList<long> AddedRelativeKeys
		{
			get => _addedRelativeKeys;
			set
			{
				_addedRelativeKeys = value;
				NotifyPropertyChanged(nameof(AddedRelativeKeys));
			}
		}

		public ICommand RelativeSelectedCommand => new RelayCommand<RelativeModel>(RelativeSelected);
		private void RelativeSelected(RelativeModel model)
		{
			EditableItem.RelativeID = model?.RelativeID ?? 0;
			EditableItem.Relative = model;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(StudentRelativeDetailsArgs args)
		{
			ViewModelArgs = args ?? StudentRelativeDetailsArgs.CreateDefault();
			StudentRelativeID = ViewModelArgs.StudentRelativeID;
			StudentID = ViewModelArgs.StudentID;
			AddedRelativeKeys = await StudentRelativeService.GetAddedRelativeKeysInStudentAsync(StudentID);

			if (ViewModelArgs.IsNew)
			{
				Item = new StudentRelativeModel() { StudentID = StudentID };
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await StudentRelativeService.GetStudentRelativeAsync(StudentRelativeID);
					Item = item ?? new StudentRelativeModel() { StudentRelativeID = StudentRelativeID, StudentID = StudentID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("StudentRelative", "Load", ex);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.StudentID = StudentID;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<StudentRelativeDetailsViewModel, StudentRelativeModel>(this, OnDetailsMessage);
			MessageService.Subscribe<StudentRelativeListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public StudentRelativeDetailsArgs CreateArgs()
		{
			return new StudentRelativeDetailsArgs
			{
				StudentRelativeID = Item?.StudentRelativeID ?? 0,
				StudentID = Item?.StudentID ?? 0
			};
		}

		protected override async Task<bool> SaveItemAsync(StudentRelativeModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeDetailsViewModel), "_SavingStudentsRelative"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await StudentRelativeService.UpdateStudentRelativeAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeDetailsViewModel), "_TheStudentsRelativeWasSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("StudentRelative", "Save", "Student's relative saved successfully", $"Student's relative #{model.StudentID}, {model.Relative.FullName} was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_ErrorSavingStudentsRelative0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("StudentRelative", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(StudentRelativeModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeDetailsViewModel), "_DeletingStudentRelative"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await StudentRelativeService.DeleteStudentRelativeAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeDetailsViewModel), "_StudentsRelativeHasBeenDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("StudentRelative", "Delete", "Student's relative deleted", $"Student's relative #{model.StudentID}, {model.Relative.FullName} was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_ErrorDeletingStudentsRelative0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("StudentRelative", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(StudentRelativeDetailsViewModel), "_AreYouSureYouWantToDeleteTheCurrentRelativeOfTheStudent"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<StudentRelativeModel>> GetValidationConstraints(StudentRelativeModel model)
		{
			string resourceKeyForRelative = string.Concat(nameof(StudentRelativeDetailsViewModel), "_PropertyRelative");
			string propertyRelative = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForRelative);
			var requiredRelative = new RequiredGreaterThanZeroConstraint<StudentRelativeModel>(propertyRelative, m => m.RelativeID);
			requiredRelative.SetResourceService(ResourceService);

			yield return requiredRelative;
		}

		#region Handle external messages
		private async void OnDetailsMessage(StudentRelativeDetailsViewModel sender, string message, StudentRelativeModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.StudentRelativeID == current?.StudentRelativeID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await StudentRelativeService.GetStudentRelativeAsync(current.StudentRelativeID);
									item ??= new StudentRelativeModel { StudentRelativeID = StudentRelativeID, StudentID = StudentID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_ThisStudentsRelativeHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										StatusMessage(message);
									}
								}
								catch (Exception ex)
								{
									LogException("StudentRelative", "Handle Changes", ex);
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

		private async void OnListMessage(StudentRelativeListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<StudentRelativeModel> deletedModels)
						{
							if (deletedModels.Any(r => r.StudentRelativeID == current.StudentRelativeID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await StudentRelativeService.GetStudentRelativeAsync(current.StudentRelativeID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("StudentRelative", "Handle Ranges Deleted", ex);
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
				string resourceKey = string.Concat(nameof(StudentRelativeDetailsViewModel), "_ThisStudentsRelativeHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				StatusMessage(message);
			});
		}
		#endregion
	}
}
