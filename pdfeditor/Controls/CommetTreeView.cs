using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls
{
	// Token: 0x020001B3 RID: 435
	internal class CommetTreeView : TreeView
	{
		// Token: 0x060018DA RID: 6362 RVA: 0x000602FB File Offset: 0x0005E4FB
		static CommetTreeView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommetTreeView), new FrameworkPropertyMetadata(typeof(CommetTreeView)));
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x00060320 File Offset: 0x0005E520
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is CommetTreeViewItem;
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0006032B File Offset: 0x0005E52B
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new CommetTreeViewItem();
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x00060334 File Offset: 0x0005E534
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			CommetTreeViewItem commetTreeViewItem = element as CommetTreeViewItem;
			if (commetTreeViewItem != null && commetTreeViewItem.IsSelected)
			{
				commetTreeViewItem.BringIntoView();
			}
			base.PrepareContainerForItemOverride(element, item);
		}
	}
}
