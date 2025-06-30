using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class StudentRelativeListViewModel(IStudentRelativeService studentRelativeService, ICommonServices commonServices) : GenericListViewModel<StudentRelativeDto>(commonServices)
	{
		public IStudentRelativeService StudentRelativeService { get; } = studentRelativeService;

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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeListViewModel), "_LoadingStudentRelatives"));
				StartStatusMessage(startMessage);
				if (await RefreshAsync())
				{
					string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeListViewModel), "_StudentsRelativesLoaded"));
					EndStatusMessage(endMessage);
				}
			}
		}
		public void Unload()
		{
			ViewModelArgs.Query = Query;
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc,
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
				string resourceKey = string.Concat(nameof(StudentRelativeListViewModel), "_ErrorLoadingStudentsRelatives0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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

		private async Task<IList<StudentRelativeDto>> GetItemsAsync()
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeListViewModel), "_LoadingStudentRelatives"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(StudentRelativeListViewModel), "_StudentsRelativesLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(StudentRelativeListViewModel), "_AreYouSureYouWantToDeleteTheSelectedRelativesOfTheStudent"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(StudentRelativeListViewModel), "_Deleting0StudentRelatives");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteRangesAsync(SelectedIndexRanges);
						MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await DeleteItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsDeleted", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string resourceKey = string.Concat(nameof(StudentRelativeListViewModel), "_ErrorDeleting0RelativesOfTheStudent1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("StudentRelatives", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(StudentRelativeListViewModel), "_0StudentRelativesDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage($"{count} user relatives deleted", LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<StudentRelativeDto> models)
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

			List<StudentRelativeDto> models = [];
			foreach (var range in ranges)
			{
				var studentRelatives = await StudentRelativeService.GetStudentRelativesAsync(range.Index, range.Length, request);
				models.AddRange(studentRelatives);
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
			if (ViewModelArgs.StudentID > 0)
			{
				request.Where = (r) => r.StudentID == ViewModelArgs.StudentID;
			}
			return request;
		}

		private void LogWarning(StudentRelativeDto model)
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
