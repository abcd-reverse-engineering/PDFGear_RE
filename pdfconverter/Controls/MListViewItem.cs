using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfconverter.Controls
{
	// Token: 0x020000A7 RID: 167
	internal class MListViewItem : ListViewItem
	{
		// Token: 0x06000730 RID: 1840 RVA: 0x0001A482 File Offset: 0x00018682
		static MListViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(MListViewItem), new FrameworkPropertyMetadata(typeof(MListViewItem)));
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0001A4AF File Offset: 0x000186AF
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
		}
	}
}
