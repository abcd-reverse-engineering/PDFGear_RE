using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Translation.Popups
{
	// Token: 0x020001E6 RID: 486
	public partial class TranslationWin : TranslatePopupWindow
	{
		// Token: 0x06001B95 RID: 7061 RVA: 0x000707D8 File Offset: 0x0006E9D8
		public TranslationWin(TranslateViewModel translateViewModel, PdfViewerContextMenu pdfViewerContext)
		{
			base.DataContext = translateViewModel;
			this.pdfViewerContext = pdfViewerContext;
			this.InitializeComponent();
			base.Activated += this.TranslationWin_Activated;
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x00070806 File Offset: 0x0006EA06
		private void TranslationWin_Activated(object sender, EventArgs e)
		{
			this.pdfViewerContext.IsEnabled = false;
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x00070814 File Offset: 0x0006EA14
		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			this.pdfViewerContext.IsEnabled = false;
		}

		// Token: 0x06001B98 RID: 7064 RVA: 0x00070822 File Offset: 0x0006EA22
		private void TranslatePopupWindow_MouseLeave(object sender, MouseEventArgs e)
		{
			this.pdfViewerContext.IsEnabled = true;
		}

		// Token: 0x040009DB RID: 2523
		private PdfViewerContextMenu pdfViewerContext;
	}
}
