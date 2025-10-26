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
	public partial class StudentRelativeListViewModel(IStudentRelativeService studentRelativeService, ICommonServices commonServices) : GenericListViewModel<StudentRelativeModel>(commonServices)
	{
		public IStudentRelativeService StudentRelativeService { get; } = studentRelativeService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.InfoMessages, "LoadingStudentRelatives");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.InfoMessages, "StudentsRelativesLoaded");

		public StudentRelativeListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(StudentRelativeListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? StudentRelativeListArgs.CreateEmpty();
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
			MessageService.Subscribe<StudentRelativeListViewModel>(this, OnMessage);
			MessageService.Subscribe<StudentRelativeDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public StudentRelativeListArgs CreateArgs()
		{
			return new StudentRelativeListArgs
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys,
				StudentID = ViewModelArgs.StudentID
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
				string message = ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.Errors, "ErrorLoadingStudentsRelatives0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("StudentRelatives", "Refresh", ex);
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

		private async Task<IList<StudentRelativeModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<StudentRelative> request = BuildDataRequest();
				return await StudentRelativeService.GetStudentRelativesAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<StudentRelativeDetailsViewModel>(new StudentRelativeDetailsArgs { StudentRelativeID = SelectedItem.StudentRelativeID, StudentID = SelectedItem.StudentID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<StudentRelativeDetailsViewModel>(new StudentRelativeDetailsArgs { StudentID = ViewModelArgs.StudentID });
			}
			else
			{
				NavigationService.Navigate<StudentRelativeDetailsViewModel>(new StudentRelativeDetailsArgs { StudentID = ViewModelArgs.StudentID });
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
			string content = ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteTheSelectedRelativesOfTheStudent");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.InfoMessages, "Deleting0StudentRelatives");
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
					string errorTitle = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.Errors, "ErrorDeleting0RelativesOfTheStudent1");
					StatusError(dialogTitle, string.Format(message, count, ex.Message));
					LogException("StudentRelatives", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<StudentRelativeListViewModel>(ResourceFiles.InfoMessages, "0StudentRelativesDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<StudentRelativeModel> models)
		{
			foreach (var model in models)
			{
				await StudentRelativeService.DeleteStudentRelativeAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<StudentRelative> request = BuildDataRequest();

			List<StudentRelativeModel> models = [];
			foreach (var range in ranges)
			{
				var items = await StudentRelativeService.GetStudentRelativesAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await StudentRelativeService.DeleteStudentRelativeRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private DataRequest<StudentRelative> BuildDataRequest()
		{
			var request = new DataRequest<StudentRelative>()
			{
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
			if (ViewModelArgs.StudentID > 0)
			{
				request.Where = (r) => r.StudentID == ViewModelArgs.StudentID;
			}
			return request;
		}

		private void LogWarning(StudentRelativeModel model)
		{
			LogWarning("StudentRelative", "Delete", "Student Relative deleted", $"Student Relative #{model.StudentID}, '{model.Relative.FullName}' was deleted.");
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
