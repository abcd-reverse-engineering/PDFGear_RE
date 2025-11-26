using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls.OcrComponents
{
	// Token: 0x02000259 RID: 601
	internal class OcrLanguageItemsControl : ItemsControl
	{
		// Token: 0x060022BD RID: 8893 RVA: 0x000A3F38 File Offset: 0x000A2138
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is ContentControl;
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x000A3F43 File Offset: 0x000A2143
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ContentControl();
		}
	}
}
