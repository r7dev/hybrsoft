using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
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
	public partial class ClassroomStudentListViewModel(IClassroomStudentService classroomStudentService, ICommonServices commonServices) : GenericListViewModel<ClassroomStudentModel>(commonServices)
	{
		public IClassroomStudentService ClassroomStudentService { get; } = classroomStudentService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.InfoMessages, "LoadingStudentsInTheClassroom");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.InfoMessages, "StudentsInTheClassroomLoaded");

		public ClassroomStudentListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(ClassroomStudentListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? ClassroomStudentListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			if (silent)
			{
				await RefreshAsync();
			}
			else
			{
				await RefreshWithStatusAsync();
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;

			// Release heavy collections.
			(Items as IDisposable)?.Dispose();
			Items = null;
			SelectedItems = null;
			SelectedIndexRanges = null;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<ClassroomStudentListViewModel>(this, OnMessage);
			MessageService.Subscribe<ClassroomStudentDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public ClassroomStudentListArgs CreateArgs()
		{
			return new ClassroomStudentListArgs
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys,
				ClassroomID = ViewModelArgs.ClassroomID
			};
		}

		public async Task<bool> RefreshAsync()
		{
			bool isOk = true;

			Items = null;
			ItemsCount = 0;
			SelectedItem = null;

			try
			{
				Items = await GetItemsAsync();
			}
			catch (Exception ex)
			{
				Items = [];
				string title = ResourceService.GetString(ResourceFiles.Errors, "LoadFailed");
				string message = ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.Errors, "ErrorLoadingStudentsInTheClassroom0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("ClassroomStudents", "Refresh", ex);
				isOk = false;
			}

			ItemsCount = Items.Count;
			if (!IsMultipleSelection)
			{
				SelectedItem = Items.FirstOrDefault();
			}
			NotifyPropertyChanged(nameof(Title));

			return isOk;
		}

		private async Task<IList<ClassroomStudentModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<ClassroomStudent> request = BuildDataRequest();
				return await ClassroomStudentService.GetClassroomStudentsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<ClassroomStudentDetailsViewModel>(new ClassroomStudentDetailsArgs { ClassroomStudentID = SelectedItem.ClassroomStudentID, ClassroomID = SelectedItem.ClassroomID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<ClassroomStudentDetailsViewModel>(new ClassroomStudentDetailsArgs { ClassroomID = ViewModelArgs.ClassroomID });
			}
			else
			{
				NavigationService.Navigate<ClassroomStudentDetailsViewModel>(new ClassroomStudentDetailsArgs { ClassroomID = ViewModelArgs.ClassroomID });
			}

			StatusReady();
		}

		protected override async void OnRefresh()
		{
			await RefreshWithStatusAsync();
		}

		private async Task<bool> RefreshWithStatusAsync()
		{
			StartStatusMessage(StartTitle, StartMessage);
			bool isOk = await RefreshAsync();
			if (isOk)
			{
				EndStatusMessage(EndTitle, EndMessage);
			}
			return isOk;
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string dialogTitle = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedStudentsFromTheClassroom");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.InfoMessages, "Deleting0ClassroomStudents");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage(StartTitle, string.Format(message, count));
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.Errors, "ErrorDeleting0StudentsFromTheClassroom1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("ClassroomStudents", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<ClassroomStudentListViewModel>(ResourceFiles.InfoMessages, "0StudentsInTheClassroomDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<ClassroomStudentModel> models)
		{
			foreach (var model in models)
			{
				await ClassroomStudentService.DeleteClassroomStudentAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<ClassroomStudent> request = BuildDataRequest();

			List<ClassroomStudentModel> models = [];
			foreach (var range in ranges)
			{
				var items = await ClassroomStudentService.GetClassroomStudentsAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await ClassroomStudentService.DeleteClassroomStudentRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<ClassroomStudent> BuildDataRequest()
		{
			var request = new DataRequest<ClassroomStudent>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
			if (ViewModelArgs.ClassroomID > 0)
			{
				request.Where = (r) => r.ClassroomID == ViewModelArgs.ClassroomID;
			}
			return request;
		}

		private void LogWarning(ClassroomStudentModel model)
		{
			LogWarning("ClassroomStudent", "Delete", "Student in the classroom deleted", $"Student in the classroom #{model.ClassroomID}, '{model.Student.FullName}' was deleted.");
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
				case "ItemChanged":
				case "ItemDeleted":
				case "ItemsDeleted":
				case "ItemRangesDeleted":
					await ContextService.RunAsync(async () =>
					{
						await RefreshAsync();
					});
					break;
			}
		}
	}
}
