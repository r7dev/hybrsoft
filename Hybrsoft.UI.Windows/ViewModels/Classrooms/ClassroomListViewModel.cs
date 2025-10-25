﻿using Hybrsoft.UI.Windows.Models;
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
	public partial class ClassroomListViewModel(IClassroomService classroomService, ICommonServices commonServices) : GenericListViewModel<ClassroomModel>(commonServices)
	{
		public IClassroomService ClassroomService { get; } = classroomService;

		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.InfoMessages, "LoadingClassrooms");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.InfoMessages, "ClassroomsLoaded");
		public string Prefix => ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.UI, "Prefix");

		public ClassroomListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(ClassroomListArgs args)
		{
			ViewModelArgs = args ?? ClassroomListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			await RefreshWithStatusAsync();
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
			MessageService.Subscribe<ClassroomListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public ClassroomListArgs CreateArgs()
		{
			return new ClassroomListArgs
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
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
				string message = ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.Errors, "ErrorLoadingClassrooms0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Classrooms", "Refresh", ex);
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

		private async Task<IList<ClassroomModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Classroom> request = BuildDataRequest();
				return await ClassroomService.GetClassroomsAsync(request);
			}
			return [];
		}

		public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
		private async void OnOpenInNewView()
		{
			if (SelectedItem != null)
			{
				await NavigationService.CreateNewViewAsync<ClassroomDetailsViewModel>(new ClassroomDetailsArgs { ClassroomID = SelectedItem.ClassroomID });
			}
		}

		protected override async void OnNew()
		{
			if (IsMainView)
			{
				await NavigationService.CreateNewViewAsync<ClassroomDetailsViewModel>(new ClassroomDetailsArgs());
			}
			else
			{
				NavigationService.Navigate<ClassroomDetailsViewModel>(new ClassroomDetailsArgs());
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
			string content = ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteSelectedClassrooms");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.InfoMessages, "Deleting0Classrooms");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
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
					string message = ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.Errors, "ErrorDeleting0Classrooms1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("Classrooms", "Delete", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						string title = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
						string message = ResourceService.GetString<ClassroomListViewModel>(ResourceFiles.InfoMessages, "0ClassroomsDeleted");
						EndStatusMessage(title, string.Format(message, count), LogType.Warning);
					}
				}
				else
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
					string message = ResourceService.GetString(ResourceFiles.Errors, "DeleteNotAllowed");
					StatusError(title, message);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<ClassroomModel> models)
		{
			foreach (var model in models)
			{
				await ClassroomService.DeleteClassroomAsync(model);
				LogWarning(model);
			}
		}

		private async Task<bool> DeleteRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Classroom> request = BuildDataRequest();

			List<ClassroomModel> models = [];
			foreach (var range in ranges)
			{
				var items = await ClassroomService.GetClassroomsAsync(range.Index, range.Length, request);
				models.AddRange(items);
			}
			foreach (var range in ranges.Reverse())
			{
				await ClassroomService.DeleteClassroomRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogWarning(model);
			}
			return true;
		}

		private DataRequest<Classroom> BuildDataRequest()
		{
			return new DataRequest<Classroom>()
			{
				Query = Query,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogWarning(ClassroomModel model)
		{
			LogWarning("Classroom", "Delete", "Classroom deleted", $"Classroom {model.ClassroomID} '{model.Name}' was deleted.");
		}

		private async void OnMessage(ViewModelBase sender, string message, object args)
		{
			switch (message)
			{
				case "NewItemSaved":
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