using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Utils;

namespace pdfeditor.Controls
{
	// Token: 0x020001DB RID: 475
	public partial class ShareSendFileDialog : Window
	{
		// Token: 0x06001AD0 RID: 6864 RVA: 0x0006B814 File Offset: 0x00069A14
		public ShareSendFileDialog(string filePath)
		{
			this.InitializeComponent();
			this.filePath = filePath;
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x0006B829 File Offset: 0x00069A29
		private void Button_Click()
		{
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x0006B82C File Offset: 0x00069A2C
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			await ShareUtils.ShowInFolderAsync(this.filePath);
		}

		// Token: 0x04000956 RID: 2390
		private readonly string filePath;
	}
}
