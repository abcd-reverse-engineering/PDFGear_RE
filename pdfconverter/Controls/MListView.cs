using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfconverter.Controls
{
	// Token: 0x020000A6 RID: 166
	public class MListView : ListView
	{
		// Token: 0x0600072E RID: 1838 RVA: 0x0001A40C File Offset: 0x0001860C
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			if (base.View is GridView)
			{
				bool flag = base.ItemContainerGenerator.IndexFromContainer(element) != 0;
				ListViewItem listViewItem = element as ListViewItem;
				if ((flag ? 1 : 0) % 2 == 0)
				{
					listViewItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
					return;
				}
				listViewItem.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
			}
		}
	}
}
