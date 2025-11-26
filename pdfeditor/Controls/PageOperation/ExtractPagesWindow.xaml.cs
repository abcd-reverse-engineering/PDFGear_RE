using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Patagames.Pdf.Net;
using pdfeditor.Models;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.PageOperation
{
	// Token: 0x0200023C RID: 572
	public partial class ExtractPagesWindow : Window
	{
		// Token: 0x06002081 RID: 8321 RVA: 0x0009483C File Offset: 0x00092A3C
		public ExtractPagesWindow(string path, MainViewModel mainViewModel)
		{
			this.InitializeComponent();
			this.SelectedPages.IsEnabled = false;
			this.AllPages.IsChecked = new bool?(true);
			this.FilePath = path;
			this.FilePath = path;
			this.CustomPages.IsChecked = new bool?(true);
			this.pageOpeations = ExtractPagesWindow.PageOpeations.CustomPages;
			this.CustomTextBox.IsEnabled = true;
			this.CustomTextBox.Focus();
			this.vm = mainViewModel;
			base.Owner = Application.Current.MainWindow;
			this.InitMenu();
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x000948D0 File Offset: 0x00092AD0
		public ExtractPagesWindow(int[] pages, string range, string path, MainViewModel mainViewModel)
		{
			this.InitializeComponent();
			this.Pages = pages;
			this.SelectedPages.IsChecked = new bool?(true);
			if (range.Length >= 30)
			{
				string[] array = range.Split(new char[] { ',' });
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
				this.range.Text = string.Format(" {0}{1} ({2})", pages.Length, pdfeditor.Properties.Resources.PageWinSelectedPages.Replace("\"XXX\"", " "), range.Replace(",", ", "));
			}
			this.Range = range;
			this.FilePath = path;
			this.pageOpeations = ExtractPagesWindow.PageOpeations.SelectedPages;
			this.vm = mainViewModel;
			base.Owner = Application.Current.MainWindow;
			this.InitMenu();
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x000949FC File Offset: 0x00092BFC
		private void InitMenu()
		{
			this.btnOk.Click += async delegate(object o, RoutedEventArgs e)
			{
				if (this.Check())
				{
					string text = this.FilePath;
					string text2 = "Extract.pdf";
					DocumentWrapper documentWrapper = this.vm.DocumentWrapper;
					if (!string.IsNullOrEmpty((documentWrapper != null) ? documentWrapper.DocumentPath : null))
					{
						ExtractPagesWindow.<>c__DisplayClass10_0 CS$<>8__locals1 = new ExtractPagesWindow.<>c__DisplayClass10_0();
						CS$<>8__locals1.<>4__this = this;
						DocumentWrapper documentWrapper2 = this.vm.DocumentWrapper;
						FileInfo fileInfo = new FileInfo((documentWrapper2 != null) ? documentWrapper2.DocumentPath : null);
						text = fileInfo.DirectoryName.FullPathWithoutPrefix;
						text2 = fileInfo.Name;
						ComboBoxItem comboBoxItem = this.SubsetComboBox.SelectedItem as ComboBoxItem;
						if (this.OddEvenRange(comboBoxItem.Content.ToString()))
						{
							int num;
							PdfObjectExtensions.TryParsePageRange(this.Range, out CS$<>8__locals1.Ranges, out num);
							if (this.IsDelete && CS$<>8__locals1.Ranges.Length == this.vm.Document.Pages.Count)
							{
								ModernMessageBox.Show(pdfeditor.Properties.Resources.DeletePageCheckMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							else
							{
								if (!string.IsNullOrEmpty(fileInfo.Extension))
								{
									text2 = text2.Substring(0, text2.Length - fileInfo.Extension.Length);
								}
								if (text2.Length > 48)
								{
									text2 = text2 + " [" + this.Range + "].pdf";
								}
								else
								{
									text2 = text2 + " Extract[" + this.Range + "].pdf";
								}
								if (text2.Length > 128)
								{
									string text3 = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
									string text4 = text3 + " Extract.pdf";
									int num2 = 0;
									try
									{
										while (File.Exists(Path.Combine(text, text4)))
										{
											num2++;
											text4 = text3 + string.Format(" Extract ({0}).pdf", num2);
										}
										text2 = text4;
									}
									catch
									{
										text2 = text3 + " Extract.pdf";
									}
								}
								CS$<>8__locals1.saveFileDialog = new SaveFileDialog
								{
									Filter = "pdf|*.pdf",
									CreatePrompt = false,
									OverwritePrompt = true,
									InitialDirectory = text,
									FileName = text2
								};
								CS$<>8__locals1.result = false;
								if (this.IsSplit)
								{
									CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
									{
										IsFolderPicker = true,
										InitialDirectory = CS$<>8__locals1.saveFileDialog.InitialDirectory
									};
									if (commonOpenFileDialog.ShowDialog(this) == CommonFileDialogResult.Ok)
									{
										string text5 = commonOpenFileDialog.FileName + "/Extract " + Path.GetFileNameWithoutExtension(fileInfo.Name);
										int num3 = 0;
										while (Directory.Exists(text5))
										{
											if (num3 == 0)
											{
												text5 += " Copy";
											}
											else
											{
												text5 += string.Format("{0}", num3);
											}
											num3++;
										}
										Directory.CreateDirectory(text5);
										CS$<>8__locals1.saveFileDialog.InitialDirectory = text5;
										CS$<>8__locals1.saveFileDialog.FileName = Path.Combine(text5, "Extract.pdf");
										CS$<>8__locals1.result = true;
									}
								}
								else
								{
									CS$<>8__locals1.result = CS$<>8__locals1.saveFileDialog.ShowDialog().Value;
								}
								PdfDocument document = this.vm.DocumentWrapper.Document;
								await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
								{
									ExtractPagesWindow.<>c__DisplayClass10_0.<<InitMenu>b__2>d <<InitMenu>b__2>d;
									<<InitMenu>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
									<<InitMenu>b__2>d.<>4__this = CS$<>8__locals1;
									<<InitMenu>b__2>d.<>1__state = -1;
									<<InitMenu>b__2>d.<>t__builder.Start<ExtractPagesWindow.<>c__DisplayClass10_0.<<InitMenu>b__2>d>(ref <<InitMenu>b__2>d);
									return <<InitMenu>b__2>d.<>t__builder.Task;
								}));
								if (this.IsDelete && CS$<>8__locals1.result)
								{
									PdfDocument tmpDoc = PdfDocument.CreateNew(null);
									tmpDoc.Pages.ImportPages(this.vm.Document, this.Range, 0);
									int[] arr;
									CS$<>8__locals1.Ranges.ConvertToRange(out arr);
									PageDisposeHelper.TryFixResource(this.vm.Document, CS$<>8__locals1.Ranges.Min(), CS$<>8__locals1.Ranges.Max());
									if (!arr.Contains(this.vm.CurrnetPageIndex - 1))
									{
										this.vm.LastViewPage = this.vm.Document.Pages[this.vm.CurrnetPageIndex - 1];
									}
									else if (arr.Count<int>() == 1)
									{
										if (this.vm.CurrnetPageIndex == this.vm.Document.Pages.Count<PdfPage>())
										{
											int num4 = Math.Max(0, this.vm.CurrnetPageIndex - 2);
											this.vm.LastViewPage = this.vm.Document.Pages[num4];
										}
										else
										{
											this.vm.LastViewPage = this.vm.Document.Pages[this.vm.CurrnetPageIndex];
										}
									}
									else
									{
										int num5 = arr.Max();
										if (num5 + 1 < this.vm.Document.Pages.Count<PdfPage>())
										{
											this.vm.LastViewPage = this.vm.Document.Pages[num5 + 1];
										}
										else
										{
											int num6 = Math.Max(0, arr.Min() - 1);
											this.vm.LastViewPage = this.vm.Document.Pages[num6];
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
										CS$<>8__locals1.<>4__this.vm.UpdateDocumentCore();
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
										CS$<>8__locals1.<>4__this.vm.UpdateDocumentCore();
									}, "");
									StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(new global::System.ValueTuple<int, int>(this.vm.SelectedPageIndex, this.vm.SelectedPageIndex)), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
								}
								if (CS$<>8__locals1.result)
								{
									await new FileInfo(CS$<>8__locals1.saveFileDialog.FileName).ShowInExplorerAsync(default(CancellationToken));
									base.Close();
								}
								CS$<>8__locals1 = null;
							}
						}
					}
				}
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				base.Close();
			};
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x00094A2C File Offset: 0x00092C2C
		private bool Check()
		{
			bool flag;
			try
			{
				if (this.pageOpeations == ExtractPagesWindow.PageOpeations.AllPages)
				{
					int count = this.vm.Document.Pages.Count;
					List<int> list = new List<int>();
					for (int i = 0; i < count; i++)
					{
						list.Add(i);
					}
					this.Pages = list.ToArray();
				}
				else if (this.pageOpeations == ExtractPagesWindow.PageOpeations.CustomPages)
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
				this.IsDelete = this.DeletePages.IsChecked.Value;
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

		// Token: 0x06002085 RID: 8325 RVA: 0x00094B5C File Offset: 0x00092D5C
		private void SelectedPages_Click(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if (radioButton.IsChecked.Value)
			{
				string name = radioButton.Name;
				if (name == "SelectedPages")
				{
					this.CustomTextBox.IsEnabled = false;
					this.pageOpeations = ExtractPagesWindow.PageOpeations.SelectedPages;
					this.AllpagesSubset.Visibility = Visibility.Visible;
					this.AllpagesSubset.IsSelected = true;
					return;
				}
				if (name == "AllPages")
				{
					this.CustomTextBox.IsEnabled = false;
					this.pageOpeations = ExtractPagesWindow.PageOpeations.AllPages;
					this.AllpagesSubset.Visibility = Visibility.Collapsed;
					this.EvenpagesSubset.IsSelected = true;
					return;
				}
				if (name == "CustomPages")
				{
					this.CustomTextBox.IsEnabled = true;
					this.CustomTextBox.Focus();
					this.pageOpeations = ExtractPagesWindow.PageOpeations.CustomPages;
					this.AllpagesSubset.Visibility = Visibility.Visible;
					this.AllpagesSubset.IsSelected = true;
					return;
				}
				this.pageOpeations = ExtractPagesWindow.PageOpeations.AllPages;
			}
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x00094C50 File Offset: 0x00092E50
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

		// Token: 0x06002087 RID: 8327 RVA: 0x00094D6C File Offset: 0x00092F6C
		private void OnePDF_Click(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if (radioButton.IsChecked.Value)
			{
				string name = radioButton.Name;
				if (name == "OnePDF")
				{
					this.IsSplit = false;
					return;
				}
				if (!(name == "EveryPDF"))
				{
					return;
				}
				this.IsSplit = true;
			}
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x00094DC1 File Offset: 0x00092FC1
		private void CustomTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
		}

		// Token: 0x04000D0E RID: 3342
		private int[] Pages;

		// Token: 0x04000D0F RID: 3343
		private string FilePath;

		// Token: 0x04000D10 RID: 3344
		private string Range;

		// Token: 0x04000D11 RID: 3345
		private bool IsSplit;

		// Token: 0x04000D12 RID: 3346
		private bool IsDelete;

		// Token: 0x04000D13 RID: 3347
		private ExtractPagesWindow.PageOpeations pageOpeations;

		// Token: 0x04000D14 RID: 3348
		private MainViewModel vm;

		// Token: 0x020006E5 RID: 1765
		private enum PageOpeations
		{
			// Token: 0x04002378 RID: 9080
			SelectedPages,
			// Token: 0x04002379 RID: 9081
			AllPages,
			// Token: 0x0400237A RID: 9082
			CustomPages
		}
	}
}
