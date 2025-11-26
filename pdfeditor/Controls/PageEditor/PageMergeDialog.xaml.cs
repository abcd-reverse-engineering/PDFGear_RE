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
using CommonLib.Controls;
using CommonLib.ConvertUtils;
using CommonLib.Models;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Exceptions;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Views;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024E RID: 590
	public partial class PageMergeDialog : Window
	{
		// Token: 0x06002205 RID: 8709 RVA: 0x0009D5CC File Offset: 0x0009B7CC
		public PageMergeDialog(string sourceFile, PdfDocument doc, IEnumerable<int> selectedPages, bool fromSinglePageCmd = false, InsertSourceFileType type = InsertSourceFileType.PDF)
		{
			PageMergeDialog.<>c__DisplayClass6_0 CS$<>8__locals1 = new PageMergeDialog.<>c__DisplayClass6_0();
			CS$<>8__locals1.sourceFile = sourceFile;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.InitializeComponent();
			this.sourceFile = CS$<>8__locals1.sourceFile;
			this.doc = doc;
			this.sourceFileType = type;
			if (this.sourceFileType == InsertSourceFileType.Doc)
			{
				this.LocationTextBox.Filter = "Microsoft Office Word|" + UtilManager.WordExtention;
			}
			if (!string.IsNullOrEmpty(CS$<>8__locals1.sourceFile))
			{
				try
				{
					FileInfo fileInfo = new FileInfo(CS$<>8__locals1.sourceFile);
					this.LocationTextBox.Text = fileInfo.FullName;
				}
				catch
				{
				}
			}
			this.LocationTextBox.TextChanged += this.LocationTextBox_TextChanged;
			List<int> list = selectedPages.ToList<int>();
			list.Sort();
			if (list.Count > 0)
			{
				this.PageindexNumbox.Text = selectedPages.ConvertToRange();
			}
			else
			{
				this.PageindexNumbox.Text = "";
			}
			if (doc != null)
			{
				this.PageNumber.Text = doc.Pages.Count.ToString();
			}
			if (doc != null)
			{
				this.PageNumber.Text = doc.Pages.Count.ToString();
			}
			this.InitInsertPositionRadioButtons(selectedPages, fromSinglePageCmd);
			if (this.sourceFileType != InsertSourceFileType.PDF)
			{
				this.SetProcessingState(true);
				if (this.sourceFileType == InsertSourceFileType.Doc)
				{
					Task.Run(delegate
					{
						PageMergeDialog.<>c__DisplayClass6_0.<<-ctor>b__0>d <<-ctor>b__0>d;
						<<-ctor>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<-ctor>b__0>d.<>4__this = CS$<>8__locals1;
						<<-ctor>b__0>d.<>1__state = -1;
						<<-ctor>b__0>d.<>t__builder.Start<PageMergeDialog.<>c__DisplayClass6_0.<<-ctor>b__0>d>(ref <<-ctor>b__0>d);
						return <<-ctor>b__0>d.<>t__builder.Task;
					});
					return;
				}
			}
			else
			{
				try
				{
					using (FileStream fileStream = new FileStream(CS$<>8__locals1.sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						PdfDocument pdfDocument = this.OpenDocument(fileStream);
						if (pdfDocument != null)
						{
							using (pdfDocument)
							{
								if (pdfDocument.Pages.Count == 0)
								{
									this.RangeBox.Text = "";
								}
								else if (pdfDocument.Pages.Count == 1)
								{
									this.RangeBox.Text = "1";
								}
								else
								{
									this.RangeBox.Text = string.Format("1-{0}", pdfDocument.Pages.Count);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x0009D818 File Offset: 0x0009BA18
		private void LocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.sourceFile = this.LocationTextBox.Text;
			if (this.sourceFileType != InsertSourceFileType.PDF)
			{
				this.SetProcessingState(true);
				if (this.sourceFileType == InsertSourceFileType.Doc)
				{
					Task.Run(async delegate
					{
						CancellationToken cancellationToken;
						string text = await ConvertUtils.GetTempPDF(this.sourceFileType, this.sourceFile, cancellationToken);
						string tempFile = text;
						if (!string.IsNullOrWhiteSpace(tempFile))
						{
							base.Dispatcher.Invoke(delegate
							{
								this.SetProcessingState(false);
								try
								{
									this.sourceFile = tempFile;
									this.sourceFile = tempFile;
									using (FileStream fileStream2 = new FileStream(this.sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
									{
										PdfDocument pdfDocument3 = this.OpenDocument(fileStream2);
										if (pdfDocument3 != null)
										{
											using (pdfDocument3)
											{
												if (pdfDocument3.Pages.Count == 0)
												{
													this.RangeBox.Text = "";
												}
												else if (pdfDocument3.Pages.Count == 1)
												{
													this.RangeBox.Text = "1";
												}
												else
												{
													this.RangeBox.Text = string.Format("1-{0}", pdfDocument3.Pages.Count);
												}
											}
										}
									}
								}
								catch
								{
								}
							});
						}
						else
						{
							MessageBox.Show(pdfeditor.Properties.Resources.MergeWindowsLoadfailedMessage, UtilManager.GetProductName());
							base.Dispatcher.Invoke(delegate
							{
								base.Close();
							});
						}
					});
					return;
				}
			}
			else
			{
				try
				{
					using (FileStream fileStream = new FileStream(this.sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						PdfDocument pdfDocument = this.OpenDocument(fileStream);
						if (pdfDocument != null)
						{
							using (pdfDocument)
							{
								if (pdfDocument.Pages.Count == 0)
								{
									this.RangeBox.Text = "";
								}
								else if (pdfDocument.Pages.Count == 1)
								{
									this.RangeBox.Text = "1";
								}
								else
								{
									this.RangeBox.Text = string.Format("1-{0}", pdfDocument.Pages.Count);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x0009D92C File Offset: 0x0009BB2C
		private void SetProcessingState(bool processing)
		{
			if (processing)
			{
				this.ProcessingDismissBorder.Visibility = Visibility.Visible;
				this.ProcessingRing.IsActive = true;
				return;
			}
			this.ProcessingDismissBorder.Visibility = Visibility.Collapsed;
			this.ProcessingRing.IsActive = false;
		}

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x06002208 RID: 8712 RVA: 0x0009D962 File Offset: 0x0009BB62
		// (set) Token: 0x06002209 RID: 8713 RVA: 0x0009D96A File Offset: 0x0009BB6A
		public string MergeTempFilePath { get; private set; }

		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x0600220A RID: 8714 RVA: 0x0009D973 File Offset: 0x0009BB73
		// (set) Token: 0x0600220B RID: 8715 RVA: 0x0009D97B File Offset: 0x0009BB7B
		public int MergePageCount { get; private set; }

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x0600220C RID: 8716 RVA: 0x0009D984 File Offset: 0x0009BB84
		// (set) Token: 0x0600220D RID: 8717 RVA: 0x0009D98C File Offset: 0x0009BB8C
		public int InsertPageIndex { get; private set; }

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x0600220E RID: 8718 RVA: 0x0009D995 File Offset: 0x0009BB95
		// (set) Token: 0x0600220F RID: 8719 RVA: 0x0009D99D File Offset: 0x0009BB9D
		public bool InsertBefore { get; private set; }

		// Token: 0x06002210 RID: 8720 RVA: 0x0009D9A6 File Offset: 0x0009BBA6
		private void InitInsertPositionRadioButtons(IEnumerable<int> selectedPages, bool fromSinglePageCmd)
		{
			if (fromSinglePageCmd)
			{
				this.InitInsertPositionRadioButtonsFromSinglePage(selectedPages);
				return;
			}
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x0009D9BC File Offset: 0x0009BBBC
		private void InitInsertPositionRadioButtonsFromSinglePage(IEnumerable<int> selectedPages)
		{
			int count = this.doc.Pages.Count;
			int[] array = ((selectedPages != null) ? selectedPages.ToArray<int>() : null);
			if (array != null && array.Length == 1 && array[0] >= 0 && array[0] < count)
			{
				this.firstSelectedPage = array[0];
				this.lastSelectedPage = array[0];
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.PageNumber.Text = count.ToString();
				return;
			}
			this.EndRadioButton.IsChecked = new bool?(true);
			this.InitInsertPositionRadioButtonsFromMultiPages(selectedPages);
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x0009DA58 File Offset: 0x0009BC58
		private void InitInsertPositionRadioButtonsFromMultiPages(IEnumerable<int> selectedPages)
		{
			int count = this.doc.Pages.Count;
			int[] array = ((selectedPages != null) ? selectedPages.ToArray<int>() : null);
			if (array != null)
			{
				int num = int.MaxValue;
				int num2 = int.MinValue;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] >= 0 && array[i] < count)
					{
						num = Math.Min(array[i], num);
						num2 = Math.Max(array[i], num2);
					}
				}
				if (num != 2147483647 && num >= 0)
				{
					this.firstSelectedPage = num;
				}
				if (num2 != -2147483648 && num2 <= count - 1)
				{
					this.lastSelectedPage = num2;
				}
			}
			if (this.firstSelectedPage != -1)
			{
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.InsertPosition.SelectedIndex = 1;
				this.PageNumber.Text = count.ToString();
			}
			if (this.lastSelectedPage != -1)
			{
				this.PageRadioButton.IsEnabled = true;
				this.PageRadioButton.IsChecked = new bool?(true);
				this.InsertPosition.SelectedIndex = 0;
				this.PageNumber.Text = count.ToString();
			}
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x0009DB78 File Offset: 0x0009BD78
		private async void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.RangeBox.IsFocused)
			{
				this.btnOk.Focus();
			}
			if (!IAPUtils.IsPaidUserWrapper())
			{
				if (this.sourceFileType == InsertSourceFileType.PDF)
				{
					IAPUtils.ShowPurchaseWindows("insertpages", ".pdf");
				}
				else
				{
					IAPUtils.ShowPurchaseWindows("insertword", ".pdf");
				}
			}
			else
			{
				base.IsEnabled = false;
				if (this.BeginingRadioButton.IsChecked.GetValueOrDefault())
				{
					this.InsertPageIndex = 0;
					this.InsertBefore = true;
				}
				else if (this.EndRadioButton.IsChecked.GetValueOrDefault())
				{
					this.InsertPageIndex = this.doc.Pages.Count - 1;
					this.InsertBefore = false;
				}
				else if (this.PageRadioButton.IsChecked.GetValueOrDefault())
				{
					if (this.InsertPosition.SelectedIndex == 0)
					{
						this.InsertBefore = false;
					}
					else
					{
						this.InsertBefore = true;
					}
					this.InsertPageIndex = this.GetInsertPageIndex(this.InsertBefore);
					if (this.InsertPageIndex == -1)
					{
						ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						base.IsEnabled = true;
						return;
					}
				}
				try
				{
					FileInfo fileInfo = this.TryGetFileInfo();
					if (fileInfo != null)
					{
						using (FileStream stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							PdfDocument pdfDocument = this.OpenDocument(stream);
							if (pdfDocument != null)
							{
								using (pdfDocument)
								{
									int[] range = this.GetImportPageRange(pdfDocument);
									if (range != null && range.Length != 0)
									{
										string text = await TempFileUtils.SaveMergeSourceFile(pdfDocument, range);
										if (!string.IsNullOrEmpty(text))
										{
											this.MergeTempFilePath = text;
											this.MergePageCount = range.Length;
											base.IsEnabled = true;
											base.DialogResult = new bool?(true);
										}
										else
										{
											MessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeInsertfailedMsg, UtilManager.GetProductName());
										}
									}
									range = null;
								}
								PdfDocument pdfDocument2 = null;
							}
							else
							{
								MessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeOpenfailedMsg, UtilManager.GetProductName());
							}
						}
						FileStream stream = null;
					}
					else
					{
						MessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeOpenfailedMsg, UtilManager.GetProductName());
					}
				}
				catch
				{
				}
				base.IsEnabled = true;
			}
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x0009DBB0 File Offset: 0x0009BDB0
		private int GetInsertPageIndex(bool InsertBefore)
		{
			int count = this.doc.Pages.Count;
			int[] importPageRange = this.GetImportPageRange();
			if (importPageRange == null)
			{
				return -1;
			}
			int num = int.MaxValue;
			int num2 = int.MinValue;
			for (int i = 0; i < importPageRange.Length; i++)
			{
				if (importPageRange[i] >= 0 && importPageRange[i] < count)
				{
					num = Math.Min(importPageRange[i], num);
					num2 = Math.Max(importPageRange[i], num2);
				}
			}
			if (!InsertBefore)
			{
				if (num2 != -2147483648 && num2 <= count - 1)
				{
					return num2;
				}
			}
			else if (num != 2147483647 && num >= 0)
			{
				return num;
			}
			return this.InsertPageIndex;
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x0009DC44 File Offset: 0x0009BE44
		private int[] GetImportPageRange()
		{
			int[] array = null;
			if (string.IsNullOrEmpty(this.PageindexNumbox.Text))
			{
				return null;
			}
			int[] array2;
			int num;
			PdfObjectExtensions.TryParsePageRange(this.PageindexNumbox.Text, out array2, out num);
			if (array2 == null)
			{
				return null;
			}
			if (array2.Length != 0)
			{
				array = array2;
			}
			if (array.Any((int c) => c < 0 || c >= this.doc.Pages.Count))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x0009DCA4 File Offset: 0x0009BEA4
		private PdfDocument OpenDocument(Stream stream)
		{
			PdfDocument pdfDocument = null;
			string text = this.password;
			bool flag = false;
			do
			{
				try
				{
					pdfDocument = PdfDocument.Load(stream, null, text, true);
					this.password = text;
				}
				catch (InvalidPasswordException)
				{
					if (flag)
					{
						ModernMessageBox.Show(pdfeditor.Properties.Resources.OpenDocByIncorrectPwdMsg, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					EnterPasswordDialog enterPasswordDialog = new EnterPasswordDialog();
					enterPasswordDialog.Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
					enterPasswordDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					flag = enterPasswordDialog.ShowDialog().GetValueOrDefault();
					text = enterPasswordDialog.Password;
					if (text == "")
					{
						text = null;
					}
					stream.Seek(0L, SeekOrigin.Begin);
				}
			}
			while (flag && pdfDocument == null);
			return pdfDocument;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x0009DD58 File Offset: 0x0009BF58
		private FileInfo TryGetFileInfo()
		{
			try
			{
				return new FileInfo(this.sourceFile);
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x0009DD8C File Offset: 0x0009BF8C
		private int[] GetImportPageRange(PdfDocument doc)
		{
			int[] array;
			if (this.AllPagesRadioButton.IsChecked.GetValueOrDefault())
			{
				if (doc.Pages.Count == 0)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeCorruptedDocMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return null;
				}
				array = Enumerable.Range(0, doc.Pages.Count).ToArray<int>();
			}
			else
			{
				array = this.RangeBox.PageIndexes.ToArray<int>();
			}
			if (array == null || array.Any((int c) => c < 0 || c >= doc.Pages.Count))
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeOpenfailedMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			if (this.PageRangeTypeComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.PageRangeTypeComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeCorrectPageMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			return array;
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x0009DED0 File Offset: 0x0009C0D0
		private int[] GetImportPageRange(int docMaxPage)
		{
			int[] array;
			if (this.AllPagesRadioButton.IsChecked.GetValueOrDefault())
			{
				if (docMaxPage == 0)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeCorruptedDocMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return null;
				}
				array = Enumerable.Range(0, docMaxPage).ToArray<int>();
			}
			else
			{
				array = this.RangeBox.PageIndexes.ToArray<int>();
			}
			if (array == null || array.Any((int c) => c < 0 || c >= docMaxPage))
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeOpenfailedMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			if (this.PageRangeTypeComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.PageRangeTypeComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPageMergeCorrectPageMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			return array;
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x0009E000 File Offset: 0x0009C200
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x0009E008 File Offset: 0x0009C208
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !base.IsEnabled;
			base.OnClosing(e);
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x0009E020 File Offset: 0x0009C220
		private void PageindexNumbox_TextChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.doc == null)
			{
				return;
			}
			int num = (int)(e.NewValue - 1.0);
			if (num < 0)
			{
				this.PageindexNumbox.Text = "0";
				num = 0;
			}
			if (num > this.doc.Pages.Count - 1)
			{
				this.PageindexNumbox.Text = string.Format("{0}", this.doc.Pages.Count - 1);
				num = this.doc.Pages.Count - 1;
			}
			this.InsertPageIndex = num;
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x0009E0B9 File Offset: 0x0009C2B9
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x04000E41 RID: 3649
		private string sourceFile;

		// Token: 0x04000E42 RID: 3650
		private readonly PdfDocument doc;

		// Token: 0x04000E43 RID: 3651
		private int firstSelectedPage = -1;

		// Token: 0x04000E44 RID: 3652
		private int lastSelectedPage = -1;

		// Token: 0x04000E45 RID: 3653
		private string password;

		// Token: 0x04000E46 RID: 3654
		private InsertSourceFileType sourceFileType;
	}
}
