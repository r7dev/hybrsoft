using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
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
	public partial class DismissibleStudentListViewModel(IDismissalService dismissalService, ICommonServices commonServices) : GenericListViewModel<DismissibleStudentModel>(commonServices)
	{
		IDismissalService DismissalService { get; } = dismissalService;

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(DismissibleStudentListViewModel), "_Prefix"));
		private bool HasPermissionToItemInvoke;

		public DismissibleStudentListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(DismissibleStudentListArgs args)
		{
			ViewModelArgs = args ?? DismissibleStudentListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			HasPermissionToItemInvoke = UserPermissionService.HasPermission(Permissions.DismissibleStudentsRequester);

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissibleStudentListViewModel), "_LoadingDismissibleStudents"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissibleStudentListViewModel), "_DismissibleStudentsLoaded"));
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
			MessageService.Subscribe<DismissibleStudentListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public DismissibleStudentListArgs CreateArgs()
		{
			return new DismissibleStudentListArgs
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
				string resourceKey = string.Concat(nameof(DismissibleStudentListViewModel), "_ErrorLoadingDismissibleStudents0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("DismissibleStudents", "Refresh", ex);
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

		private async Task<IList<DismissibleStudentModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<ClassroomStudent> request = BuildDataRequest();
				return await DismissalService.GetDismissibleStudentsAsync(request);
			}
			return [];
		}

		public ICommand ItemInvokedCommand => new RelayCommand<DismissibleStudentModel>(OnItemInvoked, CanItemInvoked);
		private async void OnItemInvoked(DismissibleStudentModel dismissible)
		{
			if (dismissible != null)
			{
				NavigationService.Navigate<DismissalDetailsViewModel>(new DismissalDetailsArgs { ClassroomID = dismissible.ClassroomID, StudentID = dismissible.StudentID });
			}
			await Task.CompletedTask;
		}

		private bool CanItemInvoked(DismissibleStudentModel dismissible)
		{
			return HasPermissionToItemInvoke;
		}

		protected override async void OnNew()
		{
			await Task.CompletedTask;
			throw new NotImplementedException("NewCommand not implemented in DismissibleStudentListViewModel.");
		}

		protected override async void OnRefresh()
		{
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissibleStudentListViewModel), "_LoadingDismissibleStudents"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissibleStudentListViewModel), "_DismissibleStudentsLoaded"));
				EndStatusMessage(endMessage);
			}
		}

		protected override async void OnDeleteSelection()
		{
			await Task.Delay(100);
			throw new NotImplementedException("DeleteSelectionCommand not implemented in DismissibleStudentListViewModel.");
		}

		private DataRequest<ClassroomStudent> BuildDataRequest()
		{
			IList<short> scheduleTypeIds = [];
			var now = DateTimeOffset.Now;
			// 1 - Full Day 2 - Morning, 3 - Afternoon, 4 - Nocturnal
			scheduleTypeIds = [1];
			scheduleTypeIds.Add(
				(short)(now.Hour < 12 ? 2 :
						now.Hour > 18 ? 4 : 3)
			);
			return new DataRequest<ClassroomStudent>()
			{
				Query = Query,
				Where = r => r.Classroom.Year == now.Year
					&& scheduleTypeIds.Contains(r.Classroom.ScheduleType.ScheduleTypeID),
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
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
