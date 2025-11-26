using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000247 RID: 583
	public partial class EditTextToolTipWin : Window
	{
		// Token: 0x06002139 RID: 8505 RVA: 0x0009880A File Offset: 0x00096A0A
		public EditTextToolTipWin()
		{
			GAManager.SendEvent("PromoteWindow", "TEPDFtoWord", "Show", 1L);
			this.InitializeComponent();
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x0009882E File Offset: 0x00096A2E
		private void Btn_Converter_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PromoteWindow", "TEPDFtoWord", "BtnClick", 1L);
			Ioc.Default.GetRequiredService<MainViewModel>().ConverterCommands.DoPDFToWord("TEPDFtoWord");
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x0009885F File Offset: 0x00096A5F
		private void ckb_ShowMsg_Checked(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PromoteWindow", "TEPDFtoWord", "Checked", 1L);
			ConfigManager.SetEditTextToolTipMsgFlag(true);
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x0009887D File Offset: 0x00096A7D
		private void ckb_ShowMsg_Unchecked(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PromoteWindow", "TEPDFtoWord", "UnChecked", 1L);
			ConfigManager.SetEditTextToolTipMsgFlag(false);
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x0009889B File Offset: 0x00096A9B
		private void Btn_EditAnyway_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}
}
