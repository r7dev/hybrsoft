using Hybrsoft.Domain.Dtos;
using Hybrsoft.Domain.Interfaces;
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

namespace Hybrsoft.EnterpriseManager.Controls;

public sealed partial class RelativeSuggestBox : UserControl
{
	public RelativeSuggestBox()
	{
		if (!DesignMode.DesignModeEnabled)
		{
			RelativeService = ServiceLocator.Current.GetService<IRelativeService>();
		}
		InitializeComponent();
	}

	private IRelativeService RelativeService { get; }

	#region ExcludedRelativeKeys
	public IList<long> ExcludedRelativeKeys
	{
		get { return (IList<long>)GetValue(ExcludedRelativeKeysProperty); }
		set { SetValue(ExcludedRelativeKeysProperty, value); }
	}
	public static readonly DependencyProperty ExcludedRelativeKeysProperty = DependencyProperty.Register(nameof(ExcludedRelativeKeys), typeof(IList<long>), typeof(RelativeSuggestBox), new PropertyMetadata(null));
	#endregion

	#region Items
	public IList<RelativeDto> Items
	{
		get { return (IList<RelativeDto>)GetValue(ItemsProperty); }
		set { SetValue(ItemsProperty, value); }
	}

	public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<RelativeDto>), typeof(RelativeSuggestBox), new PropertyMetadata(null));
	#endregion

	#region DisplayText
	public string DisplayText
	{
		get { return (string)GetValue(DisplayTextProperty); }
		set { SetValue(DisplayTextProperty, value); }
	}

	public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(RelativeSuggestBox), new PropertyMetadata(null));
	#endregion

	#region IsReadOnly*
	public bool IsReadOnly
	{
		get { return (bool)GetValue(IsReadOnlyProperty); }
		set { SetValue(IsReadOnlyProperty, value); }
	}

	private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var control = d as RelativeSuggestBox;
		control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
	}

	public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RelativeSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
	#endregion

	#region RelativeSelectedCommand
	public ICommand RelativeSelectedCommand
	{
		get { return (ICommand)GetValue(RelativeSelectedCommandProperty); }
		set { SetValue(RelativeSelectedCommandProperty, value); }
	}

	public static readonly DependencyProperty RelativeSelectedCommandProperty = DependencyProperty.Register(nameof(RelativeSelectedCommand), typeof(ICommand), typeof(RelativeSuggestBox), new PropertyMetadata(null));
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

	private async Task<IList<RelativeDto>> GetItems(string query)
	{
		var request = new DataRequest<Relative>()
		{
			Query = query,
			Where = r => !ExcludedRelativeKeys.Contains(r.RelativeId),
			OrderBy = r => r.FirstName
		};
		return await RelativeService.GetRelativesAsync(0, 20, request);
	}

	private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
	{
		RelativeSelectedCommand?.TryExecute(args.SelectedItem);
	}
}
