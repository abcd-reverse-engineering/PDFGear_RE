using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Properties;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001F3 RID: 499
	public partial class SignatureApplyPageWin : Window
	{
		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x000753A1 File Offset: 0x000735A1
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x000753AD File Offset: 0x000735AD
		public SignatureApplyPageWin(int currentPageIndex)
		{
			this.InitializeComponent();
			this.CurrentPageIdx = currentPageIndex;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x000753C2 File Offset: 0x000735C2
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000753D8 File Offset: 0x000735D8
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			int[] array = this.GetImportPageRange();
			if (array == null)
			{
				return;
			}
			if (!array.Contains(this.CurrentPageIdx))
			{
				List<int> list = array.ToList<int>();
				list.Add(this.CurrentPageIdx);
				array = list.ToArray();
			}
			this.ApplyPageIndex = array;
			base.DialogResult = new bool?(true);
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x0007542C File Offset: 0x0007362C
		private bool CheckPageRange()
		{
			PdfDocument document = this.VM.Document;
			int[] array = this.RangeBox.PageIndexes.ToArray<int>();
			bool? isChecked = this.AllPagesRadioButton.IsChecked;
			bool flag = false;
			if (((isChecked.GetValueOrDefault() == flag) & (isChecked != null)) && array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignaturePageRangeIncorrect, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return false;
			}
			return true;
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x00075494 File Offset: 0x00073694
		private int[] GetImportPageRange()
		{
			PdfDocument doc = this.VM.Document;
			int[] array;
			if (this.AllPagesRadioButton.IsChecked.GetValueOrDefault())
			{
				if (doc.Pages.Count == 0)
				{
					return null;
				}
				array = Enumerable.Range(0, doc.Pages.Count).ToArray<int>();
			}
			else
			{
				array = this.RangeBox.PageIndexes.ToArray<int>();
			}
			if (array.Any((int c) => c < 0 || c >= doc.Pages.Count))
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignaturePageRangeIncorrect, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			if (this.applyToComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.applyToComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignaturePageRangeIncorrect, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			return array;
		}

		// Token: 0x04000A3D RID: 2621
		public int[] ApplyPageIndex;

		// Token: 0x04000A3E RID: 2622
		private int CurrentPageIdx;
	}
}
