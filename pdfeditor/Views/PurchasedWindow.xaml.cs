using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Views
{
	// Token: 0x0200004D RID: 77
	public partial class PurchasedWindow : Window
	{
		// Token: 0x060003F5 RID: 1013 RVA: 0x00014CDF File Offset: 0x00012EDF
		public PurchasedWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00014CED File Offset: 0x00012EED
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}
}
