using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001F4 RID: 500
	public partial class SignatureClearConfirmWin : Window
	{
		// Token: 0x06001C33 RID: 7219 RVA: 0x000756BA File Offset: 0x000738BA
		public SignatureClearConfirmWin()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000756C8 File Offset: 0x000738C8
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000756DC File Offset: 0x000738DC
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}
	}
}
