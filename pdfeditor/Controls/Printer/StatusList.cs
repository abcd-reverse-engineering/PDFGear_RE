using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x0200022A RID: 554
	public class StatusList : ListView
	{
		// Token: 0x06001F19 RID: 7961 RVA: 0x0008C370 File Offset: 0x0008A570
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			if (base.View is GridView)
			{
				base.ItemContainerGenerator.IndexFromContainer(element);
				(element as ListViewItem).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
			}
		}
	}
}
