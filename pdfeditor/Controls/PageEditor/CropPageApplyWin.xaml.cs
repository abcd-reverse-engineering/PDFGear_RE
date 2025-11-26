using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000246 RID: 582
	public partial class CropPageApplyWin : Window
	{
		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06002130 RID: 8496 RVA: 0x00098560 File Offset: 0x00096760
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x0009856C File Offset: 0x0009676C
		public CropPageApplyWin()
		{
			this.InitializeComponent();
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x0009857C File Offset: 0x0009677C
		public CropPageApplyWin(int[] ApplyPageIndex)
		{
			this.InitializeComponent();
			string text = ApplyPageIndex.ConvertToRange();
			this.RangeBox.Text = text;
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x000985A8 File Offset: 0x000967A8
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000985B8 File Offset: 0x000967B8
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			int[] importPageRange = this.GetImportPageRange();
			if (importPageRange == null)
			{
				return;
			}
			this.ApplyPageIndex = importPageRange;
			base.DialogResult = new bool?(true);
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x000985E4 File Offset: 0x000967E4
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

		// Token: 0x04000D88 RID: 3464
		public int[] ApplyPageIndex;
	}
}
