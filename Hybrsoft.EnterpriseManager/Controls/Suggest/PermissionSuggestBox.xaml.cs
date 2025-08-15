using Hybrsoft.UI.Windows.Models;
using Hybrsoft.UI.Windows.Interfaces;
using Hybrsoft.EnterpriseManager.Configuration;
using Hybrsoft.EnterpriseManager.Extensions;
using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public sealed partial class PermissionSuggestBox : UserControl
	{
		public PermissionSuggestBox()
		{
			if (!DesignMode.DesignModeEnabled)
			{
				PermissionService = ServiceLocator.Current.GetService<IPermissionService>();
			}
			this.InitializeComponent();
		}

		private IPermissionService PermissionService { get; }

		#region ExcludedKeys
		public IList<long> ExcludedKeys
		{
			get { return (IList<long>)GetValue(ExcludedKeysProperty); }
			set { SetValue(ExcludedKeysProperty, value); }
		}
		public static readonly DependencyProperty ExcludedKeysProperty = DependencyProperty.Register(nameof(ExcludedKeys), typeof(IList<long>), typeof(PermissionSuggestBox), new PropertyMetadata(null));
		#endregion

		#region Items
		public IList<PermissionModel> Items
		{
			get { return (IList<PermissionModel>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<PermissionModel>), typeof(PermissionSuggestBox), new PropertyMetadata(null));
		#endregion

		#region DisplayText
		public string DisplayText
		{
			get { return (string)GetValue(DisplayTextProperty); }
			set { SetValue(DisplayTextProperty, value); }
		}

		public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(PermissionSuggestBox), new PropertyMetadata(null));
		#endregion

		#region IsReadOnly*
		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}

		private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = d as PermissionSuggestBox;
			control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
		}

		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PermissionSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
		#endregion

		#region SelectedCommand
		public ICommand SelectedCommand
		{
			get { return (ICommand)GetValue(SelectedCommandProperty); }
			set { SetValue(SelectedCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectedCommandProperty = DependencyProperty.Register(nameof(SelectedCommand), typeof(ICommand), typeof(PermissionSuggestBox), new PropertyMetadata(null));
		#endregion

		private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				if (args.CheckCurrent())
				{
					Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
					if (String.IsNullOrEmpty(sender.Text) && !string.IsNullOrEmpty(DisplayText))
					{
						DisplayText = string.Empty;
						var argsChosen = (AutoSuggestBoxSuggestionChosenEventArgs)Activator.CreateInstance(typeof(AutoSuggestBoxSuggestionChosenEventArgs), true);
						OnSuggestionChosen(sender, argsChosen);
					}
				}
			}
		}

		private async Task<IList<PermissionModel>> GetItems(string query)
		{
			var request = new DataRequest<Permission>()
			{
				Query = query,
				Where = r => !ExcludedKeys.Contains(r.PermissionID),
				OrderBy = r => r.Name
			};
			return await PermissionService.GetPermissionsAsync(0, 20, request);
		}

		private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			SelectedCommand?.TryExecute(args.SelectedItem);
		}
	}
}
