using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Speech
{
	// Token: 0x020001F2 RID: 498
	public partial class SpeechMessage : Window
	{
		// Token: 0x06001C24 RID: 7204 RVA: 0x00075274 File Offset: 0x00073474
		public SpeechMessage(string cul, int pageindex)
		{
			this.InitializeComponent();
			this.Textblock.Text = this.Textblock.Text.Replace("xxxx", pageindex.ToString()).Replace("xxx", cul);
			this.InitMenu();
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x000752C5 File Offset: 0x000734C5
		private void InitMenu()
		{
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				base.DialogResult = new bool?(true);
				base.Close();
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				base.DialogResult = new bool?(false);
				base.Close();
			};
		}
	}
}
