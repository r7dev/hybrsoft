using Hybrsoft.UI.Windows.Models;
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
	public partial class ClassroomDetailsViewModel(IClassroomService classroomService, ICommonServices commonServices) : GenericDetailsViewModel<ClassroomModel>(commonServices)
	{
		public IClassroomService ClassroomService { get; } = classroomService;

		public override string Title
		{
			get
			{
				if (Item?.IsNew ?? true)
				{
					string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_Title");
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
					string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_TitleEdit");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return $"{Item.Name}";
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public ClassroomDetailsArgs ViewModelArgs { get; private set; }

		public ICommand ScheduleTypeSelectedCommand => new RelayCommand<ScheduleTypeModel>(ScheduleTypeSelected);
		private void ScheduleTypeSelected(ScheduleTypeModel scheduleType)
		{
			EditableItem.ScheduleType = scheduleType;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(ClassroomDetailsArgs args)
		{
			ViewModelArgs = args ?? ClassroomDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new ClassroomModel() {
					MinimumYear = 2025,
					MaximumYear = 2030,
					MinimumEducationLevel = 0,
					MaximumEducationLevel = 15,
				};
				Item.Year = Item.MinimumYear;
				Item.EducationLevel = Item.MinimumEducationLevel;
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await ClassroomService.GetClassroomAsync(ViewModelArgs.ClassroomID);
					Item = item ?? new ClassroomModel { ClassroomID = ViewModelArgs.ClassroomID, IsEmpty = true };
					await Task.Delay(200);
					EditableItem.NotifyChanges();
				}
				catch (Exception ex)
				{
					LogException("Classroom", "Load", ex);
				}
			}
			NotifyPropertyChanged(nameof(ItemIsNew));
		}

		public void Unload()
		{
			ViewModelArgs.ClassroomID = Item?.ClassroomID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<ClassroomDetailsViewModel, ClassroomModel>(this, OnDetailsMessage);
			MessageService.Subscribe<ClassroomListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public override void BeginEdit()
		{
			base.BeginEdit();
		}

		protected override async Task<bool> SaveItemAsync(ClassroomModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomDetailsViewModel), "_SavingClassroom"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await ClassroomService.UpdateClassroomAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomDetailsViewModel), "_ClassroomSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Classroom", "Save", "Classroom saved successfully", $"Classroom {model.ClassroomID} '{model.Name}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_ErrorSavingClassroom0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Classroom", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(ClassroomModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomDetailsViewModel), "_DeletingClassroom"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await ClassroomService.DeleteClassroomAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomDetailsViewModel), "_ClassroomDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("Classroom", "Delete", "Classroom saved successfully", $"Classroom {model.ClassroomID} '{model.Name}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_ErrorDeletingClassroom0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Classroom", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(ClassroomDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentClassroom"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		override protected IEnumerable<IValidationConstraint<ClassroomModel>> GetValidationConstraints(ClassroomModel model)
		{
			string resourceKeyForName = string.Concat(nameof(ClassroomDetailsViewModel), "_PropertyName");
			string propertyName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForName);
			var requiredName = new RequiredConstraint<ClassroomModel>(propertyName, m => m.Name);
			requiredName.SetResourceService(ResourceService);

			string resourceKeyForYear = string.Concat(nameof(ClassroomDetailsViewModel), "_PropertyYear");
			string propertyYear = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForYear);
			var requiredYear = new RequiredGreaterThanZeroConstraint<ClassroomModel>(propertyYear, m => m.Year);
			requiredYear.SetResourceService(ResourceService);

			string resourceKeyEducationLevel = string.Concat(nameof(ClassroomDetailsViewModel), "_PropertyEducationLevel");
			string propertyEducationLevel = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyEducationLevel);
			var requiredEducationLevel = new RequiredGreaterThanZeroConstraint<ClassroomModel>(propertyEducationLevel, m => m.EducationLevel);
			requiredEducationLevel.SetResourceService(ResourceService);

			string resourceKeyForScheduleType = string.Concat(nameof(ClassroomDetailsViewModel), "_PropertyScheduleType");
			string propertyScheduleType = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForScheduleType);
			var requiredScheduleType = new RequiredGreaterThanZeroConstraint<ClassroomModel>(propertyScheduleType, m => m.ScheduleTypeID);
			requiredScheduleType.SetResourceService(ResourceService);

			yield return requiredName;
			yield return requiredYear;
			yield return requiredEducationLevel;
			yield return requiredScheduleType;
		}

		#region Handle external messages
		private async void OnDetailsMessage(ClassroomDetailsViewModel sender, string message, ClassroomModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.ClassroomID == current?.ClassroomID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await ClassroomService.GetClassroomAsync(current.ClassroomID);
									item ??= new ClassroomModel { ClassroomID = current.ClassroomID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_ThisClassroomHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
									}
								}
								catch (Exception ex)
								{
									LogException("Classroom", "Handle Changes", ex);
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

		private async void OnListMessage(ClassroomListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<ClassroomModel> deletedModels)
						{
							if (deletedModels.Any(r => r.ClassroomID == current.ClassroomID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await ClassroomService.GetClassroomAsync(current.ClassroomID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Classroom", "Handle Ranges Deleted", ex);
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
				string resourceKey = string.Concat(nameof(ClassroomDetailsViewModel), "_ThisClassroomHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
