using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000282 RID: 642
	public class CertComboBoxItemTemplateSelector : DataTemplateSelector
	{
		// Token: 0x06002528 RID: 9512 RVA: 0x000AD1C4 File Offset: 0x000AB3C4
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			ContentPresenter contentPresenter = container as ContentPresenter;
			if (contentPresenter != null)
			{
				FrameworkElement frameworkElement = contentPresenter.Parent as FrameworkElement;
				if (((frameworkElement != null) ? frameworkElement.Name : null) == "templateRoot")
				{
					return this.ComboBoxItemTemplate;
				}
			}
			return this.DropDownItemTemplate;
		}

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x06002529 RID: 9513 RVA: 0x000AD20B File Offset: 0x000AB40B
		// (set) Token: 0x0600252A RID: 9514 RVA: 0x000AD213 File Offset: 0x000AB413
		public DataTemplate ComboBoxItemTemplate { get; set; }

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x0600252B RID: 9515 RVA: 0x000AD21C File Offset: 0x000AB41C
		// (set) Token: 0x0600252C RID: 9516 RVA: 0x000AD224 File Offset: 0x000AB424
		public DataTemplate DropDownItemTemplate { get; set; }
	}
}
