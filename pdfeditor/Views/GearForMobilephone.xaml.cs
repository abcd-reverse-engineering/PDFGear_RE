using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfeditor.Views
{
	// Token: 0x0200004B RID: 75
	public partial class GearForMobilephone : Window
	{
		// Token: 0x06000369 RID: 873 RVA: 0x0001092B File Offset: 0x0000EB2B
		public GearForMobilephone()
		{
			GAManager.SendEvent("Ads", "GearForMobile", "Show", 1L);
			this.InitializeComponent();
		}
	}
}
