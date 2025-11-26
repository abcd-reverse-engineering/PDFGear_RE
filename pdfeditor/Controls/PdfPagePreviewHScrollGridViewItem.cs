using System;
using System.Windows;

namespace pdfeditor.Controls
{
	// Token: 0x020001C8 RID: 456
	public class PdfPagePreviewHScrollGridViewItem : PdfPagePreviewListViewItem
	{
		// Token: 0x060019D8 RID: 6616 RVA: 0x00066CB8 File Offset: 0x00064EB8
		static PdfPagePreviewHScrollGridViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewHScrollGridViewItem), new FrameworkPropertyMetadata(typeof(PdfPagePreviewHScrollGridViewItem)));
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x060019D9 RID: 6617 RVA: 0x00066D16 File Offset: 0x00064F16
		// (set) Token: 0x060019DA RID: 6618 RVA: 0x00066D28 File Offset: 0x00064F28
		public bool IsSelectedIndex
		{
			get
			{
				return (bool)base.GetValue(PdfPagePreviewHScrollGridViewItem.IsSelectedIndexProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewHScrollGridViewItem.IsSelectedIndexProperty, value);
			}
		}

		// Token: 0x040008EE RID: 2286
		public static readonly DependencyProperty IsSelectedIndexProperty = DependencyProperty.Register("IsSelectedIndex", typeof(bool), typeof(PdfPagePreviewListViewItem), new PropertyMetadata(false));
	}
}
