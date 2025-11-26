using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfeditor.Controls
{
	// Token: 0x020001DA RID: 474
	public partial class SetDefaultAppDialog : Window
	{
		// Token: 0x06001AC9 RID: 6857 RVA: 0x0006B6F4 File Offset: 0x000698F4
		public SetDefaultAppDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x0006B702 File Offset: 0x00069902
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06001ACB RID: 6859 RVA: 0x0006B710 File Offset: 0x00069910
		// (set) Token: 0x06001ACC RID: 6860 RVA: 0x0006B718 File Offset: 0x00069918
		public string Action { get; private set; }

		// Token: 0x06001ACD RID: 6861 RVA: 0x0006B724 File Offset: 0x00069924
		protected override void OnClosed(EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.DontShowAgainCheckbox.IsChecked.GetValueOrDefault())
			{
				stringBuilder.Append("Silence_");
			}
			if (base.DialogResult.GetValueOrDefault())
			{
				stringBuilder.Append("SetDefault");
			}
			else
			{
				stringBuilder.Append("Exit");
			}
			this.Action = stringBuilder.ToString();
			GAManager.SendEvent("ExtDefaultAppDialog", this.Action, "Count", 1L);
			base.OnClosed(e);
		}
	}
}
