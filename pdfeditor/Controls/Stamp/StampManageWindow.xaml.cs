using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001EB RID: 491
	public partial class StampManageWindow : Window
	{
		// Token: 0x06001BE2 RID: 7138 RVA: 0x000733F4 File Offset: 0x000715F4
		public StampManageWindow()
		{
			this.InitializeComponent();
			GAManager.SendEvent("CustomStampManageWindow", "Show", "Count", 1L);
		}

		// Token: 0x04000A18 RID: 2584
		public StampManageViewModel ViewModel = new StampManageViewModel();
	}
}
