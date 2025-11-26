using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001FD RID: 509
	public partial class SignatureSaveNumTipWin : Window
	{
		// Token: 0x06001C89 RID: 7305 RVA: 0x00077342 File Offset: 0x00075542
		public SignatureSaveNumTipWin()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x00077350 File Offset: 0x00075550
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}
}
