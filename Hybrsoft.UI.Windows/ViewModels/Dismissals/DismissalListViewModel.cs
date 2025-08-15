using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Infrastructure.ViewModels;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
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

		public string Prefix => ResourceService.GetString(nameof(ResourceFiles.UI), string.Concat(nameof(DismissalListViewModel), "_Prefix"));
		private bool HasPermissionToAccept;

		public DismissalListArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(DismissalListArgs args)
		{
			ViewModelArgs = args ?? DismissalListArgs.CreateEmpty();
			Query = ViewModelArgs.Query;
			HasPermissionToAccept = UserPermissionService.HasPermission(Permissions.DismissalConfirmator);

			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalListViewModel), "_LoadingDismissals"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalListViewModel), "_DismissalsLoaded"));
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
				string resourceKey = string.Concat(nameof(DismissalListViewModel), "_ErrorLoadingDismissals0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
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
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDismissal");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(DismissalListViewModel), "_AreYouSureYouWantToApproveTheSelectedDismissalRequests"));
			string approve = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Approve");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			if (await DialogService.ShowAsync(title, content, approve, cancel))
			{
				bool success = false;
				int count = 0;
				try
				{
					string resourceKey = string.Concat(nameof(DismissalListViewModel), "_Approving0Dismissals");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
					if (SelectedIndexRanges != null)
					{
						count = SelectedIndexRanges.Sum(r => r.Length);
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						success = await ApproveRangesAsync(SelectedIndexRanges);
						if (success)
						{
							MessageService.Send(this, "ItemRangesApproved", SelectedIndexRanges);
						}
					}
					else if (SelectedItems != null)
					{
						count = SelectedItems.Count;
						string message = string.Format(resourceValue, count);
						StartStatusMessage(message);
						await ApproveItemsAsync(SelectedItems);
						MessageService.Send(this, "ItemsAprroved", SelectedItems);
					}
				}
				catch (Exception ex)
				{
					string resourceKey = string.Concat(nameof(DismissalListViewModel), "_ErrorApproving0Dismissals1");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
					string message = string.Format(resourceValue, count, ex.Message);
					StatusError(message);
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
						string resourceKey = string.Concat(nameof(DismissalListViewModel), "_0DismissalsApproved");
						string resourceValue = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), resourceKey);
						string message = string.Format(resourceValue, count);
						EndStatusMessage(message, LogType.Success);
					}
				}
			}
			await Task.CompletedTask;
		}

		private bool CanAcceptSelection()
		{
			return HasPermissionToAccept;
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
			string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalListViewModel), "_LoadingDismissals"));
			StartStatusMessage(startMessage);
			if (await RefreshAsync())
			{
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(DismissalListViewModel), "_DismissalsLoaded"));
				EndStatusMessage(endMessage);
			}
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
