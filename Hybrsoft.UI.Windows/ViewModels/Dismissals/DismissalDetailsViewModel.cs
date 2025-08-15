using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Services;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class DismissalDetailsViewModel(IDismissalService dismissalService,
		IStudentService studentService,
		IStudentRelativeService studentRelativeService,
		IClassroomService classroomService,
		ICommonServices commonServices) : GenericDetailsViewModel<DismissalModel>(commonServices)
	{
		public IDismissalService DismissalService { get; } = dismissalService;
		public IStudentService StudentService { get; } = studentService;
		public IStudentRelativeService StudentRelativeService { get; } = studentRelativeService;
		public IClassroomService ClassroomService { get; } = classroomService;

		public IList<RelativeModel> Relatives { get; set; } = [];
		public ICommand RelativeSelectedCommand => new RelayCommand<RelativeModel>(RelativeSelected);
		private void RelativeSelected(RelativeModel relative)
		{
			EditableItem.RelativeID = relative?.RelativeID ?? 0;
			EditableItem.Relative = relative;
			EditableItem.NotifyChanges();
		}

		public override string Title
		{
			get
			{
				if (Item?.IsNew ?? true)
				{
					string resourceKey = string.Concat(nameof(DismissalDetailsViewModel), "_Title");
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
					string resourceKey = string.Concat(nameof(DismissalDetailsViewModel), "_TitleEdit");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return $"{Item.Student.FullName}";
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public DismissalDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(DismissalDetailsArgs args)
		{
			ViewModelArgs = args ?? DismissalDetailsArgs.CreateDefault();

			if (ViewModelArgs.IsNew)
			{
				Item = new DismissalModel() { ClassroomID = ViewModelArgs.ClassroomID, StudentID = ViewModelArgs.StudentID };
				var student = await StudentService.GetStudentAsync(Item.StudentID);
				if (student is not null)
				{
					Item.Student = student;
				}
				var classroom = await ClassroomService.GetClassroomAsync(Item.ClassroomID);
				if (classroom is not null)
				{
					Item.Classroom = classroom;
				}
				var request = new DataRequest<StudentRelative>
				{
					Where = r => r.StudentID == Item.StudentID,
					OrderBy = r => r.Relative.FirstName,
				};
				var studentRelatives = await StudentRelativeService.GetStudentRelativesAsync(request);
				Relatives = [.. studentRelatives.Select(r => r.Relative)];
				IsEditMode = true;
				NotifyChanges();
			}
			else
			{
				try
				{
					var item = await DismissalService.GetDismissalAsync(ViewModelArgs.DismissalID);
					Item = item ?? new DismissalModel { DismissalID = ViewModelArgs.DismissalID, IsEmpty = true };
				}
				catch (Exception ex)
				{
					LogException("Dismissal", "Load", ex);
				}
			}
		}

		public void Unload()
		{
			ViewModelArgs.DismissalID = Item?.DismissalID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<DismissalDetailsViewModel, DismissalModel>(this, OnDetailsMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		protected override async Task<bool> SaveItemAsync(DismissalModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalDetailsViewModel), "_SavingDismissal"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await DismissalService.UpdateDismissalAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalDetailsViewModel), "_DismissalSaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Dismissal", "Save", "Dismissal saved successfully", $"Dismissal {model.DismissalID} '{model.Student.FullName}' of '{model.Classroom.Name}' with requester '{model.Relative.FullName}' was saved successfully.");
				BackAfterSave = true;
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(DismissalDetailsViewModel), "_ErrorSavingDismissal0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Dismissal", "Save", ex);
				return false;
			}
		}

		protected override async Task<bool> DeleteItemAsync(DismissalModel model)
		{
			await Task.CompletedTask;
			throw new NotImplementedException("DeleteItemAsync is not implemented for DismissalDetailsViewModel.");
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			await Task.CompletedTask;
			throw new NotImplementedException("ConfirmDeleteAsync is not implemented for DismissalDetailsViewModel.");
		}

		override protected IEnumerable<IValidationConstraint<DismissalModel>> GetValidationConstraints(DismissalModel model)
		{
			string resourceKeyForRelative = string.Concat(nameof(DismissalDetailsViewModel), "_PropertyRelative");
			string propertyRelative = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForRelative);
			var requiredRelative = new RequiredGreaterThanZeroConstraint<DismissalModel>(propertyRelative, m => m.RelativeID);
			requiredRelative.SetResourceService(ResourceService);

			yield return requiredRelative;
		}

		#region Handle external messages
		private async void OnDetailsMessage(DismissalDetailsViewModel sender, string message, DismissalModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.DismissalID == current?.DismissalID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await DismissalService.GetDismissalAsync(current.DismissalID);
									item ??= new DismissalModel { DismissalID = current.DismissalID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(DismissalDetailsViewModel), "_ThisDismissalHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
									}
								}
								catch (Exception ex)
								{
									LogException("Dismissal", "Handle Changes", ex);
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

		private async Task OnItemDeletedExternally()
		{
			await ContextService.RunAsync(() =>
			{
				CancelEdit();
				IsEnabled = false;
				string resourceKey = string.Concat(nameof(DismissalDetailsViewModel), "_ThisDismissalHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
