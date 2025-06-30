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
	public partial class ClassroomStudentListViewModel(IClassroomStudentService classroomStudentService, ICommonServices commonServices) : GenericListViewModel<ClassroomStudentDto>(commonServices)
	{
		public IClassroomStudentService ClassroomStudentService { get; } = classroomStudentService;

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
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentListViewModel), "_LoadingStudentsInTheClassroom"));
				StartStatusMessage(startMessage);
				if (await RefreshAsync())
				{
					string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentListViewModel), "_StudentsInTheClassroomLoaded"));
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc,
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
				string resourceKey = string.Concat(nameof(ClassroomStudentListViewModel), "_ErrorLoadingStudentsInTheClassroom0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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

		private async Task<IList<ClassroomStudentDto>> GetItemsAsync()
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentListViewModel), "_LoadingStudentsInTheClassroom"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomStudentListViewModel), "_StudentsInTheClassroomLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(ClassroomStudentListViewModel), "_AreYouSureYouWantToDeleteSelectedStudentsFromTheClassroom"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(ClassroomStudentListViewModel), "_Deleting0ClassroomStudents");
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
					string resourceKey = string.Concat(nameof(ClassroomStudentListViewModel), "_ErrorDeleting0StudentsFromTheClassroom1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
					LogException("ClassroomStudents", "Delete", ex);
					count = 0;
				}
				await RefreshAsync();
				SelectedIndexRanges = null;
				SelectedItems = null;
				if (count > 0)
				{
					string resourceKey = string.Concat(nameof(ClassroomStudentListViewModel), "_0StudentsInTheClassroomDeleted");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					string message = string.Format(resourceValue, count);
					EndStatusMessage($"{count} Students in the classroom deleted", LogType.Warning);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<ClassroomStudentDto> models)
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

			List<ClassroomStudentDto> models = [];
			foreach (var range in ranges)
			{
				var ClassroomStudents = await ClassroomStudentService.GetClassroomStudentsAsync(range.Index, range.Length, request);
				models.AddRange(ClassroomStudents);
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
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
			if (ViewModelArgs.ClassroomID > 0)
			{
				request.Where = (r) => r.ClassroomID == ViewModelArgs.ClassroomID;
			}
			return request;
		}

		private void LogWarning(ClassroomStudentDto model)
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
