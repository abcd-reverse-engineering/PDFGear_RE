using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000220 RID: 544
	public partial class ScreenshotExtractTextToolbar : UserControl
	{
		// Token: 0x06001E79 RID: 7801 RVA: 0x0008757B File Offset: 0x0008577B
		public ScreenshotExtractTextToolbar()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06001E7A RID: 7802 RVA: 0x00087589 File Offset: 0x00085789
		// (set) Token: 0x06001E7B RID: 7803 RVA: 0x0008759B File Offset: 0x0008579B
		public ScreenshotDialog ScreenshotDialog
		{
			get
			{
				return (ScreenshotDialog)base.GetValue(ScreenshotExtractTextToolbar.ScreenshotDialogProperty);
			}
			set
			{
				base.SetValue(ScreenshotExtractTextToolbar.ScreenshotDialogProperty, value);
			}
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x000875AC File Offset: 0x000857AC
		private async void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			await ((screenshotDialog != null) ? screenshotDialog.CompleteExtractTextAsync(false) : null);
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x000875E3 File Offset: 0x000857E3
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog == null)
			{
				return;
			}
			screenshotDialog.Close(null);
		}

		// Token: 0x04000BB4 RID: 2996
		public static readonly DependencyProperty ScreenshotDialogProperty = DependencyProperty.Register("ScreenshotDialog", typeof(ScreenshotDialog), typeof(ScreenshotExtractTextToolbar), new PropertyMetadata(null));
	}
}
