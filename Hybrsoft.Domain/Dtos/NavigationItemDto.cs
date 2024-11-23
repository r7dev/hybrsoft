﻿using Hybrsoft.Domain.Infrastructure.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace Hybrsoft.Domain.Dtos
{
	public class NavigationItemDto : ObservableObject
	{
		public NavigationItemDto(Type viewModel)
		{
			ViewModel = viewModel;
		}

		public NavigationItemDto(string label,
			int glyph,
			string tag,
			int? parentId,
			ObservableCollection<NavigationItemDto> children,
			Type viewModel) : this(viewModel)
		{
			Label = label;
			Icon = new SymbolIcon((Symbol)glyph);
			Tag = tag;
			ParentId = parentId;
			Children = children;
			ViewModel = viewModel;
		}

		public readonly string Label;
		public readonly SymbolIcon Icon;
		public readonly string Tag;
		public readonly int? ParentId;
		public readonly ObservableCollection<NavigationItemDto> Children;
		public readonly Type ViewModel;
	}
}