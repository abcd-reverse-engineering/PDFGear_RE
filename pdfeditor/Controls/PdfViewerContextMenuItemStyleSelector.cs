using System;
using System.Windows;
using System.Windows.Controls;
using pdfeditor.Models.Menus;

namespace pdfeditor.Controls
{
	// Token: 0x020001CB RID: 459
	public class PdfViewerContextMenuItemStyleSelector : StyleSelector
	{
		// Token: 0x060019F5 RID: 6645 RVA: 0x00067720 File Offset: 0x00065920
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is ContextMenuSeparator)
			{
				return this.SeparatorStyle;
			}
			TypedContextMenuItemModel typedContextMenuItemModel = item as TypedContextMenuItemModel;
			if (typedContextMenuItemModel != null && (typedContextMenuItemModel.Type == ContextMenuItemType.StrokeColor || typedContextMenuItemModel.Type == ContextMenuItemType.FillColor))
			{
				return this.ColorMenuStyle;
			}
			ContextMenuItemModel contextMenuItemModel = item as ContextMenuItemModel;
			if (contextMenuItemModel != null && contextMenuItemModel.TagData != null && (contextMenuItemModel.TagData.MenuItemType == ContextMenuItemType.FillColor || contextMenuItemModel.TagData.MenuItemType == ContextMenuItemType.StrokeColor))
			{
				return this.ColorItemStyle;
			}
			if (item is ContextMenuTranslateModel)
			{
				return this.TranslateStyle;
			}
			if (item is ContextMenuSeparator)
			{
				return this.SeparatorStyle;
			}
			if (item is ContextMenuHorizontalButton)
			{
				return this.HorizontalButtonStyle;
			}
			if (item is ContextMenuRecentFileList)
			{
				return this.RecentFileListStyle;
			}
			if (item is IContextMenuModel)
			{
				return this.DefaultItemStyle;
			}
			return base.SelectStyle(item, container);
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x000677E7 File Offset: 0x000659E7
		// (set) Token: 0x060019F7 RID: 6647 RVA: 0x000677EF File Offset: 0x000659EF
		public Style DefaultItemStyle { get; set; }

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x060019F8 RID: 6648 RVA: 0x000677F8 File Offset: 0x000659F8
		// (set) Token: 0x060019F9 RID: 6649 RVA: 0x00067800 File Offset: 0x00065A00
		public Style ColorItemStyle { get; set; }

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x060019FA RID: 6650 RVA: 0x00067809 File Offset: 0x00065A09
		// (set) Token: 0x060019FB RID: 6651 RVA: 0x00067811 File Offset: 0x00065A11
		public Style ColorMenuStyle { get; set; }

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x060019FC RID: 6652 RVA: 0x0006781A File Offset: 0x00065A1A
		// (set) Token: 0x060019FD RID: 6653 RVA: 0x00067822 File Offset: 0x00065A22
		public Style SeparatorStyle { get; set; }

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x060019FE RID: 6654 RVA: 0x0006782B File Offset: 0x00065A2B
		// (set) Token: 0x060019FF RID: 6655 RVA: 0x00067833 File Offset: 0x00065A33
		public Style HorizontalButtonStyle { get; set; }

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06001A00 RID: 6656 RVA: 0x0006783C File Offset: 0x00065A3C
		// (set) Token: 0x06001A01 RID: 6657 RVA: 0x00067844 File Offset: 0x00065A44
		public Style RecentFileListStyle { get; set; }

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06001A02 RID: 6658 RVA: 0x0006784D File Offset: 0x00065A4D
		// (set) Token: 0x06001A03 RID: 6659 RVA: 0x00067855 File Offset: 0x00065A55
		public Style TranslateStyle { get; set; }
	}
}
