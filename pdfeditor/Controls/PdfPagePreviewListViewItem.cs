using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls
{
	// Token: 0x020001D9 RID: 473
	public class PdfPagePreviewListViewItem : ListBoxItem
	{
		// Token: 0x06001AC7 RID: 6855 RVA: 0x0006B6C7 File Offset: 0x000698C7
		static PdfPagePreviewListViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewListViewItem), new FrameworkPropertyMetadata(typeof(PdfPagePreviewListViewItem)));
		}
	}
}
