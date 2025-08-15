using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Infrastructure.Commom;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.UI.Windows.Interfaces.Infrastructure;
using Hybrsoft.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hybrsoft.UI.Windows.ViewModels
{
	public partial class CompanyDetailsViewModel(ICompanyService companyService, ICommonServices commonServices) : GenericDetailsViewModel<CompanyModel>(commonServices)
	{
		public ICompanyService CompanyService { get; } = companyService;

		private bool _hasEditorPermission;

		public override string Title
		{
			get
			{
				if (Item?.IsNew ?? true)
				{
					string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_Title");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return TitleEdit;
			}
		}
		public string TitleEdit
		{
			get
			{
				if (Item == null)
				{
					string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_TitleEdit");
					string resourceValue = ResourceService.GetString(nameof(ResourceFiles.UI), resourceKey);
					return resourceValue;
				}
				return $"{Item.LegalName}";
			}
		}

		public override bool ItemIsNew => Item?.IsNew ?? true;

		public CompanyDetailsArgs ViewModelArgs { get; private set; }

		public ICommand CountrySelectedCommand => new RelayCommand<CountryModel>(CountrySelected);
		private void CountrySelected(CountryModel country)
		{
			EditableItem.Country = country;
			EditableItem.NotifyChanges();
		}

		public async Task LoadAsync(CompanyDetailsArgs args)
		{
			ViewModelArgs = args ?? CompanyDetailsArgs.CreateDefault();
			_hasEditorPermission = UserPermissionService.HasPermission(Permissions.CompanyEditor);

			if (ViewModelArgs.IsNew)
			{
				Item = new CompanyModel();
				IsEditMode = true;
			}
			else
			{
				try
				{
					var item = await CompanyService.GetCompanyAsync(ViewModelArgs.CompanyID);
					Item = item ?? new CompanyModel { CompanyID = ViewModelArgs.CompanyID, IsEmpty = true };
					await Task.Delay(200);
					EditableItem.NotifyChanges();
				}
				catch (Exception ex)
				{
					LogException("Company", "Load", ex);
				}
			}
			NotifyPropertyChanged(nameof(ItemIsNew));
		}

		public void Unload()
		{
			ViewModelArgs.CompanyID = Item?.CompanyID ?? 0;
		}

		public void Subscribe()
		{
			MessageService.Subscribe<CompanyDetailsViewModel, CompanyModel>(this, OnDetailsMessage);
			MessageService.Subscribe<CompanyListViewModel>(this, OnListMessage);
		}

		public void Unsubscribe()
		{
			MessageService.Unsubscribe(this);
		}

		public new ICommand EditCommand => new RelayCommand(OnEdit, CanEdit);
		public override void BeginEdit()
		{
			base.BeginEdit();
		}
		private bool CanEdit()
		{
			return _hasEditorPermission;
		}

		protected override async Task<bool> SaveItemAsync(CompanyModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyDetailsViewModel), "_SavingCompany"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await CompanyService.UpdateCompanyAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyDetailsViewModel), "_CompanySaved"));
				EndStatusMessage(endMessage, LogType.Success);
				LogSuccess("Company", "Save", "Company saved successfully", $"Company {model.CompanyID} '{model.LegalName}' was saved successfully.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_ErrorSavingCompany0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Company", "Save", ex);
				return false;
			}
		}

		public new ICommand DeleteCommand => new RelayCommand(OnDelete, CanDelete);
		protected override async Task<bool> DeleteItemAsync(CompanyModel model)
		{
			try
			{
				string startMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyDetailsViewModel), "_DeletingCompany"));
				StartStatusMessage(startMessage);
				await Task.Delay(100);
				await CompanyService.DeleteCompanyAsync(model);
				string endMessage = ResourceService.GetString(nameof(ResourceFiles.InfoMessages), string.Concat(nameof(CompanyDetailsViewModel), "_CompanyDeleted"));
				EndStatusMessage(endMessage, LogType.Warning);
				LogWarning("Company", "Delete", "Company saved successfully", $"Company {model.CompanyID} '{model.LegalName}' was deleted.");
				return true;
			}
			catch (Exception ex)
			{
				string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_ErrorDeletingCompany0");
				string resourceValue = ResourceService.GetString(nameof(ResourceFiles.Errors), resourceKey);
				string message = string.Format(resourceValue, ex.Message);
				StatusError(message);
				LogException("Company", "Delete", ex);
				return false;
			}
		}

		protected override async Task<bool> ConfirmDeleteAsync()
		{
			string title = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_Title_ConfirmDelete");
			string content = ResourceService.GetString(nameof(ResourceFiles.Questions), string.Concat(nameof(CompanyDetailsViewModel), "_AreYouSureYouWantToDeleteCurrentCompany"));
			string delete = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_PrimaryButtonText_Delete");
			string cancel = ResourceService.GetString(nameof(ResourceFiles.UI), "ContentDialog_CloseButtonText_Cancel");
			return await DialogService.ShowAsync(title, content, delete, cancel);
		}
		private bool CanDelete()
		{
			return _hasEditorPermission;
		}

		override protected IEnumerable<IValidationConstraint<CompanyModel>> GetValidationConstraints(CompanyModel model)
		{
			string resourceKeyForLegalName = string.Concat(nameof(CompanyDetailsViewModel), "_PropertyLegalName");
			string propertyLegalName = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForLegalName);
			var requiredLegalName = new RequiredConstraint<CompanyModel>(propertyLegalName, m => m.LegalName);
			requiredLegalName.SetResourceService(ResourceService);

			string resourceKeyForCountry = string.Concat(nameof(CompanyDetailsViewModel), "_PropertyCountry");
			string propertyCountry = ResourceService.GetString(nameof(ResourceFiles.ValidationErrors), resourceKeyForCountry);
			var requiredCountry = new RequiredGreaterThanZeroConstraint<CompanyModel>(propertyCountry, m => m.CountryID);
			requiredCountry.SetResourceService(ResourceService);

			yield return requiredLegalName;
			yield return requiredCountry;
		}

		#region Handle external messages
		private async void OnDetailsMessage(CompanyDetailsViewModel sender, string message, CompanyModel changed)
		{
			var current = Item;
			if (current != null)
			{
				if (changed != null && changed.CompanyID == current?.CompanyID)
				{
					switch (message)
					{
						case "ItemChanged":
							await ContextService.RunAsync(async () =>
							{
								try
								{
									var item = await CompanyService.GetCompanyAsync(current.CompanyID);
									item ??= new CompanyModel { CompanyID = current.CompanyID, IsEmpty = true };
									current.Merge(item);
									current.NotifyChanges();
									NotifyPropertyChanged(nameof(Title));
									if (IsEditMode)
									{
										string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_ThisCompanyHasBeenModifiedExternally");
										string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
										WarningMessage(message);
									}
								}
								catch (Exception ex)
								{
									LogException("Company", "Handle Changes", ex);
								}
							});
							break;
						case "ItemDeleted":
							await OnItemDeletedExternally();
							break;
					}
				}
			}
		}

		private async void OnListMessage(CompanyListViewModel sender, string message, object args)
		{
			var current = Item;
			if (current != null)
			{
				switch (message)
				{
					case "ItemsDeleted":
						if (args is IList<CompanyModel> deletedModels)
						{
							if (deletedModels.Any(r => r.CompanyID == current.CompanyID))
							{
								await OnItemDeletedExternally();
							}
						}
						break;
					case "ItemRangesDeleted":
						try
						{
							var model = await CompanyService.GetCompanyAsync(current.CompanyID);
							if (model == null)
							{
								await OnItemDeletedExternally();
							}
						}
						catch (Exception ex)
						{
							LogException("Company", "Handle Ranges Deleted", ex);
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
				string resourceKey = string.Concat(nameof(CompanyDetailsViewModel), "_ThisCompanyHasBeenDeletedExternally");
				string message = ResourceService.GetString(nameof(ResourceFiles.Warnings), resourceKey);
				WarningMessage(message);
			});
		}
		#endregion
	}
}
