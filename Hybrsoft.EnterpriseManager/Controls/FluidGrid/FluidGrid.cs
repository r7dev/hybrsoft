using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hybrsoft.EnterpriseManager.Controls
{
	public partial class FluidGrid : Panel
	{
		#region Columns
		public int Columns
		{
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns), typeof(int), typeof(FluidGrid), new PropertyMetadata(2));
		#endregion

		#region ColumnWidth
		public double ColumnWidth
		{
			get { return (double)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(nameof(ColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));
		#endregion

		#region MinColumnWidth
		public double MinColumnWidth
		{
			get { return (double)GetValue(MinColumnWidthProperty); }
			set { SetValue(MinColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(180.0));
		#endregion

		#region MaxColumnWidth
		public double MaxColumnWidth
		{
			get { return (double)GetValue(MaxColumnWidthProperty); }
			set { SetValue(MaxColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(nameof(MaxColumnWidth), typeof(double), typeof(FluidGrid), new PropertyMetadata(360.0));
		#endregion

		#region ColumnSpan
		static public int GetColumnSpan(UIElement element)
		{
			return (int)element.GetValue(ColumnSpanProperty);
		}

		static public void SetColumnSpan(UIElement element, int value)
		{
			element.SetValue(ColumnSpanProperty, value);
		}

		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(FluidGrid), new PropertyMetadata(1));
		#endregion

		#region ColumnSpacing
		public double ColumnSpacing
		{
			get { return (double)GetValue(ColumnSpacingProperty); }
			set { SetValue(ColumnSpacingProperty, value); }
		}

		public static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));
		#endregion

		#region RowSpacing
		public double RowSpacing
		{
			get { return (double)GetValue(RowSpacingProperty); }
			set { SetValue(RowSpacingProperty, value); }
		}

		public static readonly DependencyProperty RowSpacingProperty = DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(FluidGrid), new PropertyMetadata(0.0));
		#endregion

		protected override Size MeasureOverride(Size availableSize)
		{
			var countWidth = InferColumns(availableSize.Width);
			int count = countWidth.Item1;
			double width = countWidth.Item2;

			foreach (FrameworkElement item in Children.Cast<FrameworkElement>())
			{
				int span = GetActualColumnSpan(item, count);
				Size size = new (width * span + (ColumnSpacing * (span - 1)), availableSize.Height);
				item.Measure(size);
				if (item is FormComboBox && item.ActualWidth >= 0 && item.ActualWidth < size.Width)
				{
					item.Width = size.Width;
				}
			}

			int x = 0;
			int y = 0;
			double[] rowHeights = new double[Children.Count];

			foreach (FrameworkElement item in Children.Cast<FrameworkElement>())
			{
				int span = GetActualColumnSpan(item, count);
				if (x > 0 && x + span > count)
				{
					x = 0;
					y++;
				}
				rowHeights[y] = Math.Max(rowHeights[y], item.DesiredSize.Height);
				x += span;
			}

			double height = 0;
			for (int n = 0; n < rowHeights.Length; n++)
			{
				height += rowHeights[n];
			}
			height += RowSpacing * y;

			return new Size(count * width + (ColumnSpacing * (count - 1)), height);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var countWidth = InferColumns(finalSize.Width);
			int count = countWidth.Item1;
			double width = countWidth.Item2;
			double widthspacing = width + ColumnSpacing;

			int x = 0;
			int y = 0;
			double[] rowHeights = new double[Children.Count];
			FrameworkElement[,] cells = new FrameworkElement[count, Children.Count];

			foreach (FrameworkElement item in Children.Cast<FrameworkElement>())
			{
				int span = GetActualColumnSpan(item, count);
				if (x > 0 && x + span > count)
				{
					x = 0;
					y++;
				}
				rowHeights[y] = Math.Max(rowHeights[y], item.DesiredSize.Height);
				cells[x, y] = item;
				x += span;
			}

			double height = 0;
			for (int i = 0; i < Children.Count; i++)
			{
				double h = 0;
				for (int j = 0; j < count; j++)
				{
					var cell = cells[j, i];
					if (cell != null)
					{
						int span = GetActualColumnSpan(cell, count);
						cell.Arrange(new Rect(j * widthspacing, height, width * span + (ColumnSpacing * (span - 1)), rowHeights[i]));
						h = Math.Max(h, cell.DesiredSize.Height);
					}
				}
				height += h + RowSpacing;
			}

			return finalSize;
		}

		private static int GetActualColumnSpan(FrameworkElement element, int count)
		{
			int span = Math.Max(1, GetColumnSpan(element));
			return Math.Min(count, span);
		}

		private Tuple<int, double> InferColumns(double availableWidth)
		{
			if (ColumnWidth > 0)
			{
				return Tuple.Create(Columns, Math.Min(ColumnWidth, MaxColumnWidth));
			}

			if (Double.IsInfinity(availableWidth))
			{
				return Tuple.Create(Columns, MaxColumnWidth);
			}

			double maxDesiredWidth = Columns * MaxColumnWidth + ColumnSpacing * (Columns - 1);
			if (maxDesiredWidth <= availableWidth)
			{
				return Tuple.Create(Columns, MaxColumnWidth);
			}

			double minDesiredWidth = Columns * MinColumnWidth + ColumnSpacing * (Columns - 1);
			if (minDesiredWidth <= availableWidth)
			{
				return Tuple.Create(Columns, (availableWidth - ColumnSpacing * (Columns - 1)) / Columns);
			}

			if (MinColumnWidth >= availableWidth)
			{
				return Tuple.Create(1, availableWidth);
			}

			double width = MinColumnWidth + ColumnSpacing;
			int n = 1;
			for (; n < Columns; n++)
			{
				width += MinColumnWidth;
				if (width >= availableWidth)
				{
					break;
				}
			}

			return Tuple.Create(n, (availableWidth - ColumnSpacing * (n - 1)) / n);
		}
	}
}
