using Hybrsoft.Enums;
using Hybrsoft.UI.Windows.Infrastructure.Common;
using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class AppLogDetailsViewModel(ICommonServices commonServices) : GenericDetailsViewModel<AppLogModel>(commonServices)
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
				Item = item ?? new AppLogModel { AppLogID = 0, IsEmpty = true };
			}
			catch (Exception ex)
			{
				LogException("AppLog", "Load", ex);
			}
		}
		public void Unload()
		{
			ViewModelArgs.AppLogID = Item?.AppLogID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<AppLogDetailsViewModel, AppLogModel>(this, OnDetailsMessage);
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
				AppLogID = Item?.AppLogID ?? 0
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

		protected override Task<bool> SaveItemAsync(AppLogModel model)
		{
			throw new NotImplementedException();
		}

		protected override async Task<bool> DeleteItemAsync(AppLogModel model)
		{
			try
			{
				string startTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "Processing");
				string startMessage = ResourceService.GetString<AppLogDetailsViewModel>(ResourceFiles.InfoMessages, "DeletingLog");
				StartStatusMessage(startTitle, startMessage);
				await Task.Delay(100);
				await LogService.DeleteLogAsync(model);
				string endTitle = ResourceService.GetString(ResourceFiles.InfoMessages, "DeletionSuccessful");
				string endMessage = ResourceService.GetString<AppLogDetailsViewModel>(ResourceFiles.InfoMessages, "LogDeleted");
				EndStatusMessage(endTitle, endMessage, LogType.Warning);
				return true;
			}
			catch (Exception ex)
			{
				string title = ResourceService.GetString(ResourceFiles.Errors, "DeletionFailed");
				string message = ResourceService.GetString<AppLogDetailsViewModel>(ResourceFiles.Errors, "ErrorDeletingLog0");
				StatusError(title, string.Format(message, ex.Message));
				LogException("AppLog", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString<AppLogDetailsViewModel>(ResourceFiles.Questions, "AreYouSureYouWantToDeleteCurrentLog");
			string delete = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(ResourceFiles.UI, "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		#region Handle external messages
		private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.AppLogID == current?.AppLogID)
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
						if (args is IList<AppLogModel> deletedModels)
						{
							if (deletedModels.Any(r => r.AppLogID == current.AppLogID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						var model = await LogService.GetLogAsync(current.AppLogID);
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
				string title = ResourceService.GetString(ResourceFiles.Warnings, "ExternalDeletion");
				string message = ResourceService.GetString<AppLogDetailsViewModel>(ResourceFiles.Warnings, "ThisLogHasBeenDeletedExternally");
				WarningMessage(title, message);
			});
		}
		#endregion
	}
}
