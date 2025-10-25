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
	public partial class DismissalListViewModel(IDismissalService dismissalService, ICommonServices commonServices) : GenericListViewModel<DismissalModel>(commonServices)
	{
		IDismissalService DismissalService { get; } = dismissalService;

		private bool _hasPermissionToAccept;
		private string StartTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
		private string StartMessage => ResourceService.GetString<DismissalListViewModel>(ResourceFiles.InfoMessages, "LoadingDismissals");
		private string EndTitle => ResourceService.GetString(ResourceFiles.InfoMessages, "LoadSuccessful");
		private string EndMessage => ResourceService.GetString<DismissalListViewModel>(ResourceFiles.InfoMessages, "DismissalsLoaded");
		public string Prefix => ResourceService.GetString<DismissalListViewModel>(ResourceFiles.UI, "Prefix");

		public DismissalListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(DismissalListArgs args)
		{
			ViewModelArgs = args ?? DismissalListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			_hasPermissionToAccept = AuthorizationService.HasPermission(Permissions.DismissalConfirmator);

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
			MessageService.Subscribe<DismissalListViewModel>(this, OnMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public DismissalListArgs CreateArgs()
		{
			return new DismissalListArgs
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
				string message = ResourceService.GetString<DismissalListViewModel>(ResourceFiles.Errors, "ErrorLoadingDismissals0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("Dismissals", "Refresh", ex);
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

		private async Task<IList<DismissalModel>> GetItemsAsync()
		{
			if (!ViewModelArgs.IsEmpty)
			{
				DataRequest<Dismissal> request = BuildDataRequest();
				return await DismissalService.GetDismissalsAsync(request);
			}
			return [];
		}

		public ICommand AcceptCommand => new RelayCommand(OnAcceptSelection, CanAcceptSelection);
		private async void OnAcceptSelection()
		{
			StatusReady();
			string dialogTitle = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDismissal");
			string content = ResourceService.GetString<DismissalListViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToApproveTheSelectedDismissalRequests");
			string approve = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Approve");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(dialogTitle, content, approve, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string message = ResourceService.GetString<DismissalListViewModel>(ResourceFiles.InfoMessages, "Approving0Dismissals");
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						StartStatusMessage(StartTitle, string.Format(message, count));
						success = await ApproveRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesApproved", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						StartStatusMessage(StartTitle, string.Format(message, count));
						await ApproveItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsAprroved", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string title = ResourceService.GetString(ResourceFiles.Errors, "ApprovalFailed");
					string message = ResourceService.GetString<DismissalListViewModel>(ResourceFiles.Errors, "ErrorApproving0Dismissals1");
					StatusError(title, string.Format(message, count, ex.Message));
					LogException("Dismissals", "Accept", ex);
					count = 0;
				}
				if (success)
				{
					await RefreshAsync();
					SelectedIndexRanges = null;
					SelectedItems = null;
					if (count > 0)
					{
						string title = ResourceService.GetString(ResourceFiles.InfoMessages, "ApprovalSuccessful");
						string message = ResourceService.GetString<DismissalListViewModel>(ResourceFiles.InfoMessages, "0DismissalsApproved");
						EndStatusMessage(title, string.Format(message, count), LogType.Success);
					}
				}
			}
			await Task.CompletedTask;
		}

		private bool CanAcceptSelection()
		{
			return _hasPermissionToAccept;
		}

		private async Task ApproveItemsAsync(IEnumerable<DismissalModel> models)
		{
			foreach (var model in models)
			{
				await DismissalService.ApproveDismissalAsync(model);
				LogSuccess(model);
			}
		}

		private async Task<bool> ApproveRangesAsync(IEnumerable<IndexRange> ranges)
		{
			DataRequest<Dismissal> request = BuildDataRequest();

			List<DismissalModel> models = [];
			foreach (var range in ranges)
			{
				var dismissals = await DismissalService.GetDismissalsAsync(range.Index, range.Length, request);
				models.AddRange(dismissals);
			}
			foreach (var range in ranges.Reverse())
			{
				await DismissalService.ApproveDismissalRangeAsync(range.Index, range.Length, request);
			}
			foreach (var model in models)
			{
				LogSuccess(model);
			}
			return true;
		}

		protected override async void OnNew()
		{
			await Task.CompletedTask;
			throw new NotImplementedException("NewCommand not implemented in DismissalListViewModel.");
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
			await Task.CompletedTask;
			throw new NotImplementedException("DeleteSelectionCommand not implemented in DismissalListViewModel.");
		}

		private DataRequest<Dismissal> BuildDataRequest()
		{
			return new DataRequest<Dismissal>()
			{
				Query = Query,
				Where = r => r.DismissedOn == null,
				OrderBy = ViewModelArgs.OrderBy,
				OrderByDesc = ViewModelArgs.OrderByDesc
			};
		}

		private void LogSuccess(DismissalModel model)
		{
			LogSuccess("Dismissal", "Approve", "Dismissal approved", $"Dismissal {model.DismissalID} '{model.Student.FullName}' of '{model.Classroom.Name}' with requester '{model.Relative.FullName}' was approved.");
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
