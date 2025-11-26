using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Views
{
	// Token: 0x0200004F RID: 79
	public partial class SaveAttachmentSuccessWindow : Window
	{
		// Token: 0x06000400 RID: 1024 RVA: 0x00014FDA File Offset: 0x000131DA
		public SaveAttachmentSuccessWindow()
		{
			this.InitializeComponent();
			base.Owner = Application.Current.MainWindow;
			base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00014FFF File Offset: 0x000131FF
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0001500D File Offset: 0x0001320D
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}
	}
}
