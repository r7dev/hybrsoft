using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using Microsoft.Data.SqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class StudentBelongingListViewModel(IStudentBelongingService studentBelongingService,
		ISettingsService settingsService,
		ICommonServices commonServices) : GenericListViewModel<StudentBelongingModel>(commonServices)
	{
		private readonly IStudentBelongingService _studentBelongingService = studentBelongingService;
		private readonly ISettingsService _settingsService = settingsService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.InfoMessages, "LoadingStudentsBelongings");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.InfoMessages, "StudentBelongingsLoaded");

		public StudentBelongingListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(StudentBelongingListArgs args, bool silent = false)
		{
			ViewModelArgs = args ?? StudentBelongingListArgs.CreateEmpty();
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
			MessageService.Subscribe<StudentBelongingListViewModel>(this, OnMessage);
			MessageService.Subscribe<StudentBelongingDetailsViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
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
				string message = ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.Errors, "ErrorLoadingStudentBelongings0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("StudentBelongings", "Refresh", ex);
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

		private async Task<IList<StudentBelongingModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<StudentBelonging> request = await BuildDataRequestAsync();
				return await _studentBelongingService.GetStudentBelongingsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<StudentBelongingDetailsViewModel>(new StudentBelongingDetailsArgs { StudentBelongingID = SelectedItem.StudentBelongingID, StudentID = SelectedItem.StudentID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<StudentBelongingDetailsViewModel>(new StudentBelongingDetailsArgs { StudentID = ViewModelArgs.StudentID });
			}
			else
			{
				NavigationService.Navigate<StudentBelongingDetailsViewModel>(new StudentBelongingDetailsArgs { StudentID = ViewModelArgs.StudentID });
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
			string content = ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteTheSelectedStudentsBelonging");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string message = ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.InfoMessages, "Deleting0StudentBelongings");
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
					string message = ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.Errors, "ErrorDeleting0BelongingsOfTheStudent1");
					StatusError(dialogTitle, string.Format(message, count, ex.Message));
					LogException("StudentBelongings", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
					string message = ResourceService.GetString<StudentBelongingListViewModel>(ResourceFiles.InfoMessages, "0StudentBelongingsDeleted");
					EndStatusMessage(title, string.Format(message, count), LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<StudentBelongingModel> models)
		{
			foreach (var model in models)
			{
				await _studentBelongingService.DeleteStudentBelongingAsync(model);
				LogWarning(model);
			}
		}

		private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<StudentBelonging> request = await BuildDataRequestAsync();

			List<StudentBelongingModel> models = [];
			foreach (var range in ranges)
			{
				var items = await _studentBelongingService.GetStudentBelongingsAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await _studentBelongingService.DeleteStudentBelongingRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
		}

		private async Task<DataRequest<StudentBelonging>> BuildDataRequestAsync()
		{
			bool useSemanticSearch = _settingsService.UseSemanticSearch;
			var request = new DataRequest<StudentBelonging>()
			{
				UseSemanticSearch = useSemanticSearch,
				QueryEmbedding = useSemanticSearch && !string.IsNullOrWhiteSpace(Query)
					? await EmbeddingService.GenerateEmbeddingAsync(Query)
					: SqlVector<float>.CreateNull(EmbeddingService.EmbeddingDimension),
				Query = Query,
				OrderBys = ViewModelArgs.OrderBys
			};
			if (ViewModelArgs.StudentID > 0)
			{
				request.Where = (r) => r.StudentID == ViewModelArgs.StudentID;
			}
			return request;
		}

		private void LogWarning(StudentBelongingModel model)
		{
			LogWarning("StudentBelonging", "Delete", "Student Belonging deleted", $"Student Belonging #{model.StudentID}, '{model.DisplayName}' was deleted.");
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
