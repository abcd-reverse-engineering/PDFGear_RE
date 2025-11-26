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
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.PageOperation
{
	// Token: 0x0200023B RID: 571
	public partial class DeletePagesWindow : Window
	{
		// Token: 0x06002077 RID: 8311 RVA: 0x000941AC File Offset: 0x000923AC
		public DeletePagesWindow(MainViewModel mainViewModel, int[] pages)
		{
			this.InitializeComponent();
			this.vm = mainViewModel;
			this.Pages = pages;
			this.origalPages = pages;
			this.SelectedPages.IsChecked = new bool?(true);
			this.Range = pages.ConvertToRange();
			if (this.Range.Length >= 30)
			{
				string[] array = this.Range.Split(new char[] { ',' });
				this.range.Text = string.Format("{0}{1} ({2}, {3}, {4}, {5}......{6}, {7})", new object[]
				{
					pages.Length,
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
				this.range.Text = string.Format(" {0}{1} ({2})", pages.Length, pdfeditor.Properties.Resources.PageWinSelectedPages.Replace("\"XXX\"", " "), this.Range.Replace(",", ", "));
			}
			this.pageOpeations = DeletePagesWindow.PageOpeations.SelectedPages;
			this.InitMenu();
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x000942E0 File Offset: 0x000924E0
		public DeletePagesWindow(MainViewModel mainViewModel)
		{
			this.InitializeComponent();
			this.vm = mainViewModel;
			this.SelectedPages.IsEnabled = false;
			this.CustomPages.IsChecked = new bool?(true);
			this.pageOpeations = DeletePagesWindow.PageOpeations.CustomPages;
			this.CustomTextBox.IsEnabled = true;
			this.CustomTextBox.Focus();
			this.InitMenu();
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x00094344 File Offset: 0x00092544
		private void SelectedPages_Click(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if (radioButton.IsChecked.Value)
			{
				string name = radioButton.Name;
				if (name == "SelectedPages")
				{
					this.CustomTextBox.IsEnabled = false;
					this.pageOpeations = DeletePagesWindow.PageOpeations.SelectedPages;
					this.AllpagesSubset.Visibility = Visibility.Visible;
					this.AllpagesSubset.IsSelected = true;
					return;
				}
				if (name == "AllPages")
				{
					this.CustomTextBox.IsEnabled = false;
					this.pageOpeations = DeletePagesWindow.PageOpeations.AllPages;
					this.AllpagesSubset.Visibility = Visibility.Collapsed;
					this.EvenpagesSubset.IsSelected = true;
					return;
				}
				if (name == "CustomPages")
				{
					this.CustomTextBox.IsEnabled = true;
					this.CustomTextBox.Focus();
					this.pageOpeations = DeletePagesWindow.PageOpeations.CustomPages;
					this.AllpagesSubset.Visibility = Visibility.Visible;
					this.AllpagesSubset.IsSelected = true;
					return;
				}
				this.pageOpeations = DeletePagesWindow.PageOpeations.AllPages;
			}
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x00094435 File Offset: 0x00092635
		private void InitMenu()
		{
			this.btnOk.Click += async delegate(object o, RoutedEventArgs e)
			{
				if (this.Check())
				{
					ComboBoxItem comboBoxItem = this.SubsetComboBox.SelectedItem as ComboBoxItem;
					if (this.OddEvenRange(comboBoxItem.Content.ToString()))
					{
						int[] array;
						int num;
						PdfObjectExtensions.TryParsePageRange(this.Range, out array, out num);
						if (this.vm.Document.Pages.Count - array.Length <= 0)
						{
							ModernMessageBox.Show(pdfeditor.Properties.Resources.DeletePageCheckMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
						else
						{
							if (array.Length == 1)
							{
								if (ModernMessageBox.Show(pdfeditor.Properties.Resources.DeleteSinglePageAskMsg, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
								{
									return;
								}
							}
							else if (ModernMessageBox.Show(pdfeditor.Properties.Resources.DeletePageAskMsg, "PDFgear", MessageBoxButton.YesNo, MessageBoxResult.None, null, false) != MessageBoxResult.Yes)
							{
								return;
							}
							PdfDocument tmpDoc = PdfDocument.CreateNew(null);
							tmpDoc.Pages.ImportPages(this.vm.Document, this.Range, 0);
							int[] arr;
							array.ConvertToRange(out arr);
							PageDisposeHelper.TryFixResource(this.vm.Document, array.Min(), array.Max());
							if (!arr.Contains(this.vm.CurrnetPageIndex - 1))
							{
								this.vm.LastViewPage = this.vm.Document.Pages[this.vm.CurrnetPageIndex - 1];
							}
							else if (arr.Count<int>() == 1)
							{
								if (this.vm.CurrnetPageIndex == this.vm.Document.Pages.Count<PdfPage>())
								{
									int num2 = Math.Max(0, this.vm.CurrnetPageIndex - 2);
									this.vm.LastViewPage = this.vm.Document.Pages[num2];
								}
								else
								{
									this.vm.LastViewPage = this.vm.Document.Pages[this.vm.CurrnetPageIndex];
								}
							}
							else
							{
								int num3 = arr.Max();
								if (num3 + 1 < this.vm.Document.Pages.Count<PdfPage>())
								{
									this.vm.LastViewPage = this.vm.Document.Pages[num3 + 1];
								}
								else
								{
									int num4 = arr.Min();
									int num5 = Math.Max(0, num4 - 1);
									this.vm.LastViewPage = this.vm.Document.Pages[num5];
								}
							}
							for (int i = arr.Length - 1; i >= 0; i--)
							{
								this.vm.Document.Pages.DeleteAt(arr[i]);
							}
							global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.vm.Document);
							if (pdfControl != null && pdfControl.Document != null)
							{
								pdfControl.UpdateDocLayout();
							}
							this.vm.UpdateDocumentCore();
							await this.vm.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
							{
								for (int j = 0; j < arr.Length; j++)
								{
									doc.Pages.ImportPages(tmpDoc, string.Format("{0}", j + 1), arr[j]);
								}
								global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(doc);
								if (pdfControl2 != null && pdfControl2.Document != null)
								{
									pdfControl2.UpdateDocLayout();
								}
								this.vm.UpdateDocumentCore();
							}, delegate(PdfDocument doc)
							{
								for (int k = arr.Length - 1; k >= 0; k--)
								{
									doc.Pages.DeleteAt(arr[k]);
								}
								global::PDFKit.PdfControl pdfControl3 = global::PDFKit.PdfControl.GetPdfControl(doc);
								if (pdfControl3 != null && pdfControl3.Document != null)
								{
									pdfControl3.UpdateDocLayout();
								}
								this.vm.UpdateDocumentCore();
							}, "");
							base.DialogResult = new bool?(true);
						}
					}
				}
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				base.DialogResult = new bool?(false);
			};
		}

		// Token: 0x0600207B RID: 8315 RVA: 0x00094468 File Offset: 0x00092668
		private bool OddEvenRange(string ranges)
		{
			List<int> list = new List<int>();
			if (ranges == pdfeditor.Properties.Resources.SelectPageAllEvenPagesItem)
			{
				for (int k = 0; k < this.Pages.Length; k++)
				{
					if ((this.Pages[k] + 1) % 2 == 0)
					{
						list.Add(this.Pages[k]);
					}
				}
				list.ToArray();
				this.Range = list.ConvertToRange();
			}
			else if (ranges == pdfeditor.Properties.Resources.SelectPageAllOddPagesItem)
			{
				for (int j = 0; j < this.Pages.Length; j++)
				{
					if ((this.Pages[j] + 1) % 2 != 0)
					{
						list.Add(this.Pages[j]);
					}
				}
				list.ToArray();
				this.Range = list.ConvertToRange();
			}
			else
			{
				list = this.Pages.Select((int i) => i).ToList<int>();
				this.Range = this.Pages.ConvertToRange();
			}
			if (list.Count <= 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return false;
			}
			return true;
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x00094584 File Offset: 0x00092784
		private bool Check()
		{
			bool flag;
			try
			{
				if (this.pageOpeations == DeletePagesWindow.PageOpeations.AllPages)
				{
					int count = this.vm.Document.Pages.Count;
					List<int> list = new List<int>();
					for (int i = 0; i < count; i++)
					{
						list.Add(i);
					}
					this.Pages = list.ToArray();
				}
				else if (this.pageOpeations == DeletePagesWindow.PageOpeations.CustomPages)
				{
					int[] array;
					int num;
					PdfObjectExtensions.TryParsePageRange(this.CustomTextBox.Text, out array, out num);
					if (array == null)
					{
						ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return false;
					}
					this.Pages = array;
				}
				else
				{
					this.Pages = this.origalPages;
				}
				if (this.Pages.Max() >= this.vm.Document.Pages.Count || this.Pages.Min() < 0)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				flag = false;
			}
			return flag;
		}

		// Token: 0x04000CFD RID: 3325
		private MainViewModel vm;

		// Token: 0x04000CFE RID: 3326
		private int[] Pages;

		// Token: 0x04000CFF RID: 3327
		private string Range;

		// Token: 0x04000D00 RID: 3328
		private int[] origalPages;

		// Token: 0x04000D01 RID: 3329
		private DeletePagesWindow.PageOpeations pageOpeations;

		// Token: 0x020006E1 RID: 1761
		private enum PageOpeations
		{
			// Token: 0x0400236B RID: 9067
			SelectedPages,
			// Token: 0x0400236C RID: 9068
			AllPages,
			// Token: 0x0400236D RID: 9069
			CustomPages
		}
	}
}
