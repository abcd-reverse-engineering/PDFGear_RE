using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000260 RID: 608
	public class ToolbarButtonContextMenu : ContextMenu
	{
		// Token: 0x06002339 RID: 9017 RVA: 0x000A5FD2 File Offset: 0x000A41D2
		static ToolbarButtonContextMenu()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarButtonContextMenu), new FrameworkPropertyMetadata(typeof(ToolbarButtonContextMenu)));
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x000A5FF7 File Offset: 0x000A41F7
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ToolbarButtonContextMenuItem();
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x000A5FFE File Offset: 0x000A41FE
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is ToolbarButtonContextMenuItem;
		}
	}
}
