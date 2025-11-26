using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PDFLauncher
{
	// Token: 0x0200000A RID: 10
	public partial class PurchasedWindow : Window
	{
		// Token: 0x06000021 RID: 33 RVA: 0x000023CD File Offset: 0x000005CD
		public PurchasedWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000023DB File Offset: 0x000005DB
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}
}
