using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x02000228 RID: 552
	public partial class NoPrintDetectedWindow : Window
	{
		// Token: 0x06001EE5 RID: 7909 RVA: 0x0008AE15 File Offset: 0x00089015
		public NoPrintDetectedWindow(string Content)
		{
			this.InitializeComponent();
			this.ContentPresenter.Content = Content;
			GAManager.SendEvent("NoPrinterWindow", "Show", "Count", 1L);
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x0008AE45 File Offset: 0x00089045
		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("AddPrintWindow", "AddBtn", "Count", 1L);
			Process.Start("ms-settings:printers");
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x0008AE68 File Offset: 0x00089068
		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (PrinterSettings.InstalledPrinters.Count == 0)
			{
				GAManager.SendEvent("NoPrinterWindow", "OkBtn", "NoPrinter", 1L);
				base.DialogResult = new bool?(false);
				return;
			}
			GAManager.SendEvent("NoPrinterWindow", "OkBtn", "HasPrinter", 1L);
			base.DialogResult = new bool?(true);
		}
	}
}
