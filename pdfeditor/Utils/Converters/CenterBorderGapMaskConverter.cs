using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000EB RID: 235
	internal class CenterBorderGapMaskConverter : IMultiValueConverter
	{
		// Token: 0x06000C4E RID: 3150 RVA: 0x00040660 File Offset: 0x0003E860
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			Type typeFromHandle = typeof(double);
			if (values == null || values.Length != 3 || values[0] == null || values[1] == null || values[2] == null || !typeFromHandle.IsAssignableFrom(values[0].GetType()) || !typeFromHandle.IsAssignableFrom(values[1].GetType()) || !typeFromHandle.IsAssignableFrom(values[2].GetType()))
			{
				return DependencyProperty.UnsetValue;
			}
			double num = (double)values[0];
			double num2 = (double)values[1];
			double num3 = (double)values[2];
			if (num2 == 0.0 || num3 == 0.0)
			{
				return null;
			}
			Grid grid = new Grid();
			grid.Width = num2;
			grid.Height = num3;
			ColumnDefinition columnDefinition = new ColumnDefinition();
			ColumnDefinition columnDefinition2 = new ColumnDefinition();
			ColumnDefinition columnDefinition3 = new ColumnDefinition();
			columnDefinition.Width = new GridLength(30.0, GridUnitType.Pixel);
			columnDefinition2.Width = new GridLength(num);
			columnDefinition3.Width = new GridLength(1.0, GridUnitType.Star);
			grid.ColumnDefinitions.Add(columnDefinition);
			grid.ColumnDefinitions.Add(columnDefinition2);
			grid.ColumnDefinitions.Add(columnDefinition3);
			RowDefinition rowDefinition = new RowDefinition();
			RowDefinition rowDefinition2 = new RowDefinition();
			rowDefinition.Height = new GridLength(num3 / 2.0);
			rowDefinition2.Height = new GridLength(1.0, GridUnitType.Star);
			grid.RowDefinitions.Add(rowDefinition);
			grid.RowDefinitions.Add(rowDefinition2);
			Rectangle rectangle = new Rectangle();
			Rectangle rectangle2 = new Rectangle();
			Rectangle rectangle3 = new Rectangle();
			rectangle.Fill = Brushes.Black;
			rectangle2.Fill = Brushes.Black;
			rectangle3.Fill = Brushes.Black;
			Grid.SetRowSpan(rectangle, 2);
			Grid.SetRow(rectangle, 0);
			Grid.SetColumn(rectangle, 0);
			Grid.SetRow(rectangle2, 1);
			Grid.SetColumn(rectangle2, 1);
			Grid.SetRowSpan(rectangle3, 2);
			Grid.SetRow(rectangle3, 0);
			Grid.SetColumn(rectangle3, 2);
			grid.Children.Add(rectangle);
			grid.Children.Add(rectangle2);
			grid.Children.Add(rectangle3);
			return new VisualBrush(grid);
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x00040880 File Offset: 0x0003EA80
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { Binding.DoNothing };
		}
	}
}
