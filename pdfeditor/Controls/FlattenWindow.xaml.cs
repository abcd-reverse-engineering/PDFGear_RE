using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Controls
{
	// Token: 0x020001B8 RID: 440
	public partial class FlattenWindow : Window
	{
		// Token: 0x0600190F RID: 6415 RVA: 0x00060DB4 File Offset: 0x0005EFB4
		public FlattenWindow(string range, int[] SelectedIndex, PdfDocument pdfDocument)
		{
			this.InitializeComponent();
			this.Document = pdfDocument;
			this.selectedIndex = SelectedIndex;
			if (!string.IsNullOrEmpty(range))
			{
				if (range.Length >= 30)
				{
					string[] array = range.Split(new char[] { ',' });
					this.Range.Text = string.Format("{0}{1} ({2}, {3}, {4}, {5}......{6}, {7})", new object[]
					{
						SelectedIndex.Length,
						pdfeditor.Properties.Resources.PageWinSelectedPages.Replace("\"XXX\"", " "),
						array[0],
						array[1],
						array[2],
						array[3],
						array[array.Length - 2],
						array[array.Length - 1]
					});
				}
				else
				{
					this.Range.Text = string.Format(" {0}{1} ({2})", SelectedIndex.Length, pdfeditor.Properties.Resources.PageWinSelectedPages.Replace("\"XXX\"", " "), range.Replace(",", ", "));
				}
			}
			if (this.selectedIndex.Count<int>() < 1)
			{
				this.selectedIndex = new int[1];
				this.selectedIndex[0] = pdfDocument.Pages.CurrentIndex;
				int[] array2;
				range = this.selectedIndex.ConvertToRange(out array2);
				this.Range.Text = string.Format(" {0}{1} ({2})", this.selectedIndex.Length, pdfeditor.Properties.Resources.PageWinSelectedPages.Replace("\"XXX\"", " "), range.Replace(",", ", "));
			}
			GAManager.SendEvent("FlattenWindow", "Show", "Count", 1L);
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00060F46 File Offset: 0x0005F146
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x00060F48 File Offset: 0x0005F148
		private int GetImportPageCount()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < this.Document.Pages.Count; i++)
				{
					array[i] = i;
				}
			}
			else if (this.SelectedPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.selectedIndex.Count<int>()];
				if (this.selectedIndex.Count<int>() >= 0)
				{
					array = this.selectedIndex;
				}
				else
				{
					array[0] = this.Document.Pages.CurrentIndex;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.CustomTextBox.Text))
				{
					return 0;
				}
				int[] array2;
				int num;
				PdfObjectExtensions.TryParsePageRange(this.CustomTextBox.Text, out array2, out num);
				if (array2 == null)
				{
					return 0;
				}
				if (array2.Length != 0)
				{
					array = array2;
				}
			}
			if (array.Any((int c) => c < 0 || c >= this.Document.Pages.Count))
			{
				return 0;
			}
			if (array.Length == 0)
			{
				return 0;
			}
			return array.Count<int>();
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00061050 File Offset: 0x0005F250
		private int[] GetImportPageRange()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < this.Document.Pages.Count; i++)
				{
					array[i] = i;
				}
			}
			else if (this.SelectedPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.selectedIndex.Count<int>()];
				if (this.selectedIndex.Count<int>() >= 0)
				{
					array = this.selectedIndex;
				}
				else
				{
					array[0] = this.Document.Pages.CurrentIndex;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.CustomTextBox.Text))
				{
					return null;
				}
				int[] array2;
				int num;
				PdfObjectExtensions.TryParsePageRange(this.CustomTextBox.Text, out array2, out num);
				if (array2 == null)
				{
					return null;
				}
				if (array2.Length != 0)
				{
					array = array2;
				}
			}
			if (array.Any((int c) => c < 0 || c >= this.Document.Pages.Count))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x00061154 File Offset: 0x0005F354
		private void PrimaryButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("FlattenWindow", "OkBtn", "Count", 1L);
			if (this.CustomTextBox.IsFocused)
			{
				this.PrimaryButton.Focus();
			}
			if (this.GetImportPageCount() > this.Document.Pages.Count)
			{
				return;
			}
			this.Indexs = this.GetImportPageRange();
			if (this.Indexs == null)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			base.DialogResult = new bool?(true);
			base.Close();
		}

		// Token: 0x04000867 RID: 2151
		private PdfDocument Document;

		// Token: 0x04000868 RID: 2152
		private int[] selectedIndex;

		// Token: 0x04000869 RID: 2153
		public int[] Indexs;
	}
}
