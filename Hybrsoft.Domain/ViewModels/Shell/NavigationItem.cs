using Hybrsoft.Domain.Infrastructure.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace Hybrsoft.Domain.ViewModels
{
	public class NavigationItem : ObservableObject
	{
		public NavigationItem(Type viewModel)
		{
			ViewModel = viewModel;
		}
		public NavigationItem(int glyph, string label, int superiorId, Type viewModel) : this(viewModel)
		{
			Label = label;
			Icon = null;//Char.ConvertFromUtf32(glyph).ToString();
			SuperiorId = superiorId;
		}
		public NavigationItem(int glyph, string label, int? superiorId, ObservableCollection<NavigationItem> children, Type viewModel) : this(viewModel)
		{
			Label = label;
			Icon = new SymbolIcon((Symbol)glyph);
			SuperiorId = superiorId;
			Children = children;
			ViewModel = viewModel;
		}

		public readonly SymbolIcon Icon;
		public readonly string Label;
		public readonly int? SuperiorId;
		public readonly ObservableCollection<NavigationItem> Children;
		public readonly Type ViewModel;

		private string _badge = null;
		public string Badge
		{
			get => _badge;
			set => Set(ref _badge, value);
		}
	}
}
