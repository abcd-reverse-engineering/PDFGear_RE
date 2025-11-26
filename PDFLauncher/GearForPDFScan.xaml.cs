using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using CommonLib.Common;

namespace PDFLauncher
{
	// Token: 0x02000008 RID: 8
	public partial class GearForPDFScan : Window
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002217 File Offset: 0x00000417
		public GearForPDFScan()
		{
			this.InitializeComponent();
			GAManager.SendEvent("Ads", "ScanPDF", "Show", 1L);
		}
	}
}
