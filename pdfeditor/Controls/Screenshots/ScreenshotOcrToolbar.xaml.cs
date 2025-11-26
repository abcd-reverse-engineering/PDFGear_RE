using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000215 RID: 533
	public partial class ScreenshotOcrToolbar : UserControl
	{
		// Token: 0x06001D8E RID: 7566 RVA: 0x0007F6F0 File Offset: 0x0007D8F0
		public ScreenshotOcrToolbar()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06001D8F RID: 7567 RVA: 0x0007F6FE File Offset: 0x0007D8FE
		// (set) Token: 0x06001D90 RID: 7568 RVA: 0x0007F710 File Offset: 0x0007D910
		public ScreenshotDialog ScreenshotDialog
		{
			get
			{
				return (ScreenshotDialog)base.GetValue(ScreenshotOcrToolbar.ScreenshotDialogProperty);
			}
			set
			{
				base.SetValue(ScreenshotOcrToolbar.ScreenshotDialogProperty, value);
			}
		}

		// Token: 0x06001D91 RID: 7569 RVA: 0x0007F71E File Offset: 0x0007D91E
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog == null)
			{
				return;
			}
			screenshotDialog.CompleteExtractTextAsync(true);
		}

		// Token: 0x06001D92 RID: 7570 RVA: 0x0007F732 File Offset: 0x0007D932
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog == null)
			{
				return;
			}
			screenshotDialog.Close(null);
		}

		// Token: 0x04000B22 RID: 2850
		public static readonly DependencyProperty ScreenshotDialogProperty = DependencyProperty.Register("ScreenshotDialog", typeof(ScreenshotDialog), typeof(ScreenshotOcrToolbar), new PropertyMetadata(null));
	}
}
