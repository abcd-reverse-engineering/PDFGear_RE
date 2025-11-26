using System;
using System.Windows;
using System.Windows.Controls;
using pdfeditor.Models.Menus;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x02000263 RID: 611
	public class ToolbarButtonContentTemplateSelector : DataTemplateSelector
	{
		// Token: 0x0600235E RID: 9054 RVA: 0x000A65FC File Offset: 0x000A47FC
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			ToolbarButtonModel toolbarButtonModel = item as ToolbarButtonModel;
			if (toolbarButtonModel != null)
			{
				if (toolbarButtonModel.ChildButtonModel is ToolbarChildCheckableButtonModel)
				{
					return this.ToggleButtonTemplate;
				}
				if (toolbarButtonModel.ChildButtonModel != null)
				{
					return this.TextTemplate;
				}
				if (toolbarButtonModel.ChildButtonModel == null)
				{
					return this.TextTemplate;
				}
			}
			else if (item is string)
			{
				return this.PlainTextTemplate;
			}
			return base.SelectTemplate(item, container);
		}

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x0600235F RID: 9055 RVA: 0x000A665C File Offset: 0x000A485C
		// (set) Token: 0x06002360 RID: 9056 RVA: 0x000A6664 File Offset: 0x000A4864
		public DataTemplate TextTemplate { get; set; }

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06002361 RID: 9057 RVA: 0x000A666D File Offset: 0x000A486D
		// (set) Token: 0x06002362 RID: 9058 RVA: 0x000A6675 File Offset: 0x000A4875
		public DataTemplate ToggleButtonTemplate { get; set; }

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x06002363 RID: 9059 RVA: 0x000A667E File Offset: 0x000A487E
		// (set) Token: 0x06002364 RID: 9060 RVA: 0x000A6686 File Offset: 0x000A4886
		public DataTemplate PlainTextTemplate { get; set; }
	}
}
