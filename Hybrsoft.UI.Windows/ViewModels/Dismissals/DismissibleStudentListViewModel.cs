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
	public partial class DismissibleStudentListViewModel(IDismissalService dismissalService,
		ICommonServices commonServices) : GenericListViewModel<DismissibleStudentModel>(commonServices)
	{
		private readonly IDismissalService _dismissalService = dismissalService;

		private bool _hasPermissionToItemInvoke;
		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<DismissibleStudentListViewModel>(ResourceFiles.InfoMessages, "LoadingDismissibleStudents");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<DismissibleStudentListViewModel>(ResourceFiles.InfoMessages, "DismissibleStudentsLoaded");
		public string Prefix => ResourceService.GetString<DismissibleStudentListViewModel>(ResourceFiles.UI, "Prefix");

		public DismissibleStudentListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(DismissibleStudentListArgs args)
		{
			ViewModelArgs = args ?? DismissibleStudentListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasPermissionToItemInvoke = AuthorizationService.HasPermission(Permissions.DismissibleStudentsRequester);

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
				OrderBys = ViewModelArgs.OrderBys
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
				string message = ResourceService.GetString<DismissibleStudentListViewModel>(ResourceFiles.Errors, "ErrorLoadingDismissibleStudents0");
				StatusError(title, string.Format(message, ex.Message));
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
				return await _dismissalService.GetDismissibleStudentsAsync(request);
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
			return _hasPermissionToItemInvoke;
		}

		protected override async void OnNew()
		{
			await Task.CompletedTask;
			throw new NotImplementedException("NewCommand not implemented in DismissibleStudentListViewModel.");
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
				OrderBys = ViewModelArgs.OrderBys
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
