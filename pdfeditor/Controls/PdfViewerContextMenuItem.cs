using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls
{
	// Token: 0x020001CA RID: 458
	public class PdfViewerContextMenuItem : MenuItem
	{
		// Token: 0x060019EF RID: 6639 RVA: 0x00067678 File Offset: 0x00065878
		static PdfViewerContextMenuItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfViewerContextMenuItem), new FrameworkPropertyMetadata(typeof(PdfViewerContextMenuItem)));
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x000676DE File Offset: 0x000658DE
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PdfViewerContextMenuItem();
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x000676E5 File Offset: 0x000658E5
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PdfViewerContextMenuItem;
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x060019F2 RID: 6642 RVA: 0x000676F0 File Offset: 0x000658F0
		// (set) Token: 0x060019F3 RID: 6643 RVA: 0x00067702 File Offset: 0x00065902
		public double BlankArea
		{
			get
			{
				return (double)base.GetValue(PdfViewerContextMenuItem.BlankAreaProperty);
			}
			set
			{
				base.SetValue(PdfViewerContextMenuItem.BlankAreaProperty, value);
			}
		}

		// Token: 0x040008F5 RID: 2293
		public static readonly DependencyProperty BlankAreaProperty = DependencyProperty.Register("BlankArea", typeof(double), typeof(PdfViewerContextMenuItem), new PropertyMetadata(36.0));
	}
}
