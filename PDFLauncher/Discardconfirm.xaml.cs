using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using PDFLauncher.CustomControl;

namespace PDFLauncher
{
	// Token: 0x02000007 RID: 7
	public partial class Discardconfirm : Window
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002185 File Offset: 0x00000385
		public Discardconfirm()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002193 File Offset: 0x00000393
		private void DicardBtn_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021A1 File Offset: 0x000003A1
		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}
	}
}
