using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Infrastructure.ViewModels;
using Hybrsoft.Domain.Interfaces;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.Domain.ViewModels
{
	public partial class ClassroomListViewModel(IClassroomService classroomService, ICommonServices commonServices) : GenericListViewModel<ClassroomDto>(commonServices)
	{
		public IClassroomService ClassroomService { get; } = classroomService;

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(ClassroomListViewModel), "_Prefix"));

		public ClassroomListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(ClassroomListArgs args)
		{
			ViewModelArgs = args ?? ClassroomListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomListViewModel), "_LoadingClassrooms"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomListViewModel), "_ClassroomsLoaded"));
				EndStatusMessage(endMessage);
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
				string resourceKey = string.Concat(nameof(ClassroomListViewModel), "_ErrorLoadingClassrooms0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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

		private async Task<IList<ClassroomDto>> GetItemsAsync()
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomListViewModel), "_LoadingClassrooms"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(ClassroomListViewModel), "_ClassroomsLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			StatusReady();
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(ClassroomListViewModel), "_AreYouSureYouWantToDeleteSelectedClassrooms"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, delete, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(ClassroomListViewModel), "_Deleting0Classrooms");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						success = await DeleteRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
						}
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
					string resourceKey = string.Concat(nameof(ClassroomListViewModel), "_ErrorDeleting0Classrooms1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
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
						string resourceKey = string.Concat(nameof(ClassroomListViewModel), "_0ClassroomsDeleted");
						string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
						string message = string.Format(resourceValue, count);
						EndStatusMessage(message, LogType.Warning);
					}
				}
				else
				{
					string message = ResourceService.GetString(nameof(ResourceFiles.Errors), "DeleteNotAllowed");
					StatusError(message);
				}
			}
		}

		private async Task DeleteItemsAsync(IEnumerable<ClassroomDto> models)
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

			List<ClassroomDto> models = [];
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

		private void LogWarning(ClassroomDto model)
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