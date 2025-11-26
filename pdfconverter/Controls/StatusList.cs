using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfconverter.Controls
{
	// Token: 0x020000AD RID: 173
	public class StatusList : ListView
	{
		// Token: 0x06000771 RID: 1905 RVA: 0x0001B6BC File Offset: 0x000198BC
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			if (base.View is GridView)
			{
				base.ItemContainerGenerator.IndexFromContainer(element);
				(element as ListViewItem).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
			}
		}
	}
}
