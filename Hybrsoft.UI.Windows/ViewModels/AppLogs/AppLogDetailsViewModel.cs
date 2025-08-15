using Hybrsoft.UI.Windows.Dtos;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class AppLogDetailsViewModel(ICommonServices commonServices) : GenericDetailsViewModel<AppLogDto>(commonServices)
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
				Item = item ?? new AppLogDto { AppLogID = 0, IsEmpty = true };
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

		protected override Task<bool> SaveItemAsync(AppLogDto model)
		{
			throw new NotImplementedException();
		}

		protected override async Task<bool> DeleteItemAsync(AppLogDto model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogDetailsViewModel), "_DeletingLog"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await LogService.DeleteLogAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(AppLogDetailsViewModel), "_LogDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				return true;
			}
			catch (Exception ex)
			{
				string message = ResourceService.GetString(nameof(ResourceFiles.Errors), string.Concat(nameof(AppLogDetailsViewModel), "_ErrorDeletingLog0"));
				StatusError(string.Format(message, ex.Message));
				LogException("AppLog", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(AppLogDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentLog"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}

		#region Handle external messages
		private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogDto changed)
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
						if (args is IList<AppLogDto> deletedModels)
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
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), string.Concat(nameof(AppLogDetailsViewModel), "_ThisLogHasBeenDeletedExternally"));
				WarningMessage(message);
			});
		}
		#endregion
	}
}
