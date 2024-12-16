using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Infrastructure.Commom;
using Hybrsoft.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Hybrsoft.Domain.ViewModels
{
	public class AppLogDetailsViewModel(ICommonServices commonServices) : GenericDetailsViewModel<AppLogDto>(commonServices)
	{
		override public string Title => "Activity Logs";

		public override bool ItemIsNew => false;

		public AppLogDetailsArgs ViewModelArgs { get; private set; }

		public async Task LoadAsync(AppLogDetailsArgs args)
		{
			ViewModelArgs = args ?? AppLogDetailsArgs.CreateDefault();

			try
			{
				var item = await LogService.GetLogAsync(ViewModelArgs.AppLogID);
				Item = item ?? new AppLogDto { AppLogId = 0, IsEmpty = true };
			}
			catch (Exception ex)
			{
				LogException("AppLog", "Load", ex);
			}
		}
		public void Unload()
		{
			ViewModelArgs.AppLogID = Item?.AppLogId ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<AppLogDetailsViewModel, AppLogDto>(this, OnDetailsMessage);
			MessageService.Subscribe<AppLogListViewModel>(this, OnListMessage);
		}
		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public AppLogDetailsArgs CreateArgs()
		{
			return new AppLogDetailsArgs
			{
				AppLogID = Item?.AppLogId ?? 0
			};
		}

		public ICommand CopyDescriptionCommand => new RelayCommand(OnCopyDescription);
		virtual protected void OnCopyDescription()
		{
			CopyDescriptionAsync();
		}
		virtual public void CopyDescriptionAsync()
		{
			DataPackage dataPackage = new()
			{
				RequestedOperation = DataPackageOperation.Copy
			};
			dataPackage.SetText(Item.Description);
			Clipboard.SetContent(dataPackage);
		}

		protected override Task<bool> SaveItemAsync(AppLogDto model)
		{
			throw new NotImplementedException();
		}

		protected override async Task<bool> DeleteItemAsync(AppLogDto model)
		{
			try
			{
				StartStatusMessage("Deleting log...");
				await Task.Delay(100);
				await LogService.DeleteLogAsync(model);
				EndStatusMessage("Log deleted");
				return true;
			}
			catch (Exception ex)
			{
				StatusError($"Error deleting log: {ex.Message}");
				LogException("AppLog", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current log?", "Ok", "Cancel");
		}

		#region Handle external messages
		private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogDto changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.AppLogId == current?.AppLogId)
				{
					switch (message)
					{
						case "ItemDeleted":
							await OnItemDeletedExternally();
							break;
					}
				}
			}
		}

		private async void OnListMessage(AppLogListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<AppLogDto> deletedModels)
						{
							if (deletedModels.Any(r => r.AppLogId == current.AppLogId))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						var model = await LogService.GetLogAsync(current.AppLogId);
						if (model == null)
						{
							await OnItemDeletedExternally();
						}
						break;
				}
			}
		}

		private async Task OnItemDeletedExternally()
		{
			await ContextService.RunAsync(() =>
			{
				CancelEdit();
				IsEnabled = false;
				StatusMessage("WARNING: This log has been deleted externally");
			});
		}
		#endregion
	}
}
