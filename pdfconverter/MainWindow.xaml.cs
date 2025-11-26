using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Common;
using NSOCR_NameSpace;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.Utils;
using pdfconverter.Views;
using SautinSoft;
using Syncfusion.Presentation;

namespace pdfconverter
{
	// Token: 0x02000023 RID: 35
	public partial class MainWindow : Window
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000475C File Offset: 0x0000295C
		public MainWindow()
		{
			this.InitializeComponent();
			this.InitializePDFConvert();
			this.convertFilesView.ItemsSource = this.convertFilesList;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000478C File Offset: 0x0000298C
		private void InitializePDFConvert()
		{
			ConvFromPDFType convFromPDFType = (ConvFromPDFType)App.convertType;
			this.titleTB.Text = ConvertManager.getTitle(convFromPDFType);
			this.ocrGrid.Visibility = Visibility.Visible;
			switch (convFromPDFType)
			{
			case ConvFromPDFType.PDFToWord:
				this.outputCBItem_docx.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_docx;
				break;
			case ConvFromPDFType.PDFToExcel:
				this.outputCBItem_xls.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_xls;
				this.ocrGrid.Visibility = Visibility.Hidden;
				this.excelGrid.Visibility = Visibility.Visible;
				break;
			case ConvFromPDFType.PDFToPng:
				this.outputCBItem_png.Visibility = Visibility.Visible;
				this.outputCBItem_jpeg.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_png;
				this.ocrGrid.Visibility = Visibility.Hidden;
				break;
			case ConvFromPDFType.PDFToJpg:
				this.outputCBItem_png.Visibility = Visibility.Visible;
				this.outputCBItem_jpeg.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_jpeg;
				this.ocrGrid.Visibility = Visibility.Hidden;
				break;
			case ConvFromPDFType.PDFToTxt:
				this.outputCBItem_text.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_text;
				break;
			case ConvFromPDFType.PDFToHtml:
				this.outputCBItem_html.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_html;
				break;
			case ConvFromPDFType.PDFToXml:
				this.outputCBItem_xml.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_xml;
				this.ocrGrid.Visibility = Visibility.Hidden;
				break;
			case ConvFromPDFType.PDFToRTF:
				this.outputCBItem_rtf.Visibility = Visibility.Visible;
				this.outputFormatCB.SelectedItem = this.outputCBItem_rtf;
				break;
			case ConvFromPDFType.PDFToPPT:
				this.outputCBItem_pptx.Visibility = Visibility.Visible;
				this.ocrGrid.Visibility = Visibility.Hidden;
				this.outputFormatCB.SelectedItem = this.outputCBItem_pptx;
				break;
			}
			LongPathDirectory longPathDirectory = ConfigManager.GetConvertPath();
			try
			{
				if (!Directory.Exists(longPathDirectory))
				{
					longPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PDFgear");
				}
			}
			catch
			{
			}
			if (!string.IsNullOrEmpty(longPathDirectory))
			{
				this.outputPathTB.Text = longPathDirectory.FullPathWithoutPrefix;
			}
			if (App.selectedFile != null && App.selectedFile.Length != 0)
			{
				this.AddPDFFilesToConvertList(App.selectedFile, App.seletedPassword);
			}
			string ocrlanguageL10N = ConvertManager.getOCRLanguageL10N();
			this.curLang.Text = ocrlanguageL10N;
			GAManager.SendEvent("ConvertMainWin", "Show", convFromPDFType.ToString(), 1L);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00004A28 File Offset: 0x00002C28
		private void AddPDFFilesToConvertList(string[] files, string[] passwords = null)
		{
			if (files != null && files.Length != 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string text = files[i];
					string text2 = "";
					if (passwords != null && passwords.Length != 0)
					{
						text2 = passwords[i];
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool? flag = ConToPDFUtils.CheckAccess(text);
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							flag = new FileOccupation(text).ShowDialog();
							flag2 = false;
							if ((flag.GetValueOrDefault() == flag2) & (flag != null))
							{
								goto IL_016A;
							}
						}
						if (UtilsManager.IsPDFFile(text))
						{
							if (!ConvertManager.IsFileAdded(this.convertFilesList, text))
							{
								if (ConToPDFUtils.CheckPassword(text, ref text2, this))
								{
									ConvertFileItem convertFileItem = new ConvertFileItem(text);
									this.convertFilesList.Add(convertFileItem);
									convertFileItem.parseFile(text2);
									this.updateFileItemListCBStatus();
								}
								else
								{
									ConvertFileItem convertFileItem2 = new ConvertFileItem(text);
									this.convertFilesList.Add(convertFileItem2);
									convertFileItem2.FileSelected = new bool?(false);
									convertFileItem2.ConvertStatus = FileCovertStatus.ConvertLoadedFailed;
									this.updateFileItemListCBStatus();
								}
							}
							else
							{
								MessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgFileAdded, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
							}
						}
						else if (!ConvertManager.IsFileAdded(this.convertFilesList, text))
						{
							ConvertFileItem convertFileItem3 = new ConvertFileItem(text);
							convertFileItem3.FileSelected = new bool?(false);
							convertFileItem3.ConvertStatus = FileCovertStatus.ConvertUnsupport;
							this.convertFilesList.Add(convertFileItem3);
							this.updateFileItemListCBStatus();
						}
						else
						{
							MessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgFileAdded, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
						}
					}
					IL_016A:;
				}
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00004BAC File Offset: 0x00002DAC
		private void AddFileBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "AddFileBtn", App.convertType.ToString(), 1L);
			string[] array = ConvertManager.selectMultiPDFFiles();
			if (array != null && array.Length != 0)
			{
				this.AddPDFFilesToConvertList(array, null);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00004BEC File Offset: 0x00002DEC
		private void Grid_Drop(object sender, DragEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "DragDrop", App.convertType.ToString(), 1L);
			if (this.convertFilesList == null)
			{
				return;
			}
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (array != null && array.Length != 0)
				{
					this.AddPDFFilesToConvertList(array, null);
				}
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00004C54 File Offset: 0x00002E54
		private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "OpenFile", App.convertType.ToString(), 1L);
			ConvertFileItem convertFileItem = (ConvertFileItem)((Button)sender).DataContext;
			if (convertFileItem != null && !string.IsNullOrWhiteSpace(convertFileItem.outputFile) && convertFileItem.ConvertStatus == FileCovertStatus.ConvertSucc)
			{
				UtilsManager.OpenFile(convertFileItem.outputFile);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00004CB4 File Offset: 0x00002EB4
		private void OpenFileInExploreBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "OpenInExplore", App.convertType.ToString(), 1L);
			ConvertFileItem convertFileItem = (ConvertFileItem)((Button)sender).DataContext;
			if (convertFileItem != null && !string.IsNullOrWhiteSpace(convertFileItem.outputFile) && convertFileItem.ConvertStatus == FileCovertStatus.ConvertSucc)
			{
				UtilsManager.OpenFileInExplore(convertFileItem.outputFile, true);
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00004D14 File Offset: 0x00002F14
		private void DeleteFileBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "DeleteFile", App.convertType.ToString(), 1L);
			ConvertFileItem convertFileItem = (ConvertFileItem)((Button)sender).DataContext;
			if (convertFileItem != null)
			{
				try
				{
					this.convertFilesList.Remove(convertFileItem);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00004D74 File Offset: 0x00002F74
		private void ChangeDestPathBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "ChangeOutputPathBtn", App.convertType.ToString(), 1L);
			string text = ConvertManager.selectOutputFolder(this.outputPathTB.Text);
			if (!string.IsNullOrWhiteSpace(text) && this.outputPathTB != null)
			{
				this.outputPathTB.Text = text;
				ConfigManager.SetConvertPath(this.outputPathTB.Text);
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00004DD9 File Offset: 0x00002FD9
		private void OutputPathCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00004DDB File Offset: 0x00002FDB
		private void OutputPathHL_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00004DE0 File Offset: 0x00002FE0
		private void ClearFilesBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				GAManager.SendEvent("ConvertMainWin", "ClearFilesBtnClick", App.convertType.ToString(), 1L);
				this.convertFilesList.Clear();
				this.updateFileItemListCBStatus();
			}
			catch
			{
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00004E30 File Offset: 0x00003030
		private async void ConvertBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ConvertMainWin", "ConvertBtnClick", App.convertType.ToString(), 1L);
			if (this.convertFilesList == null || this.convertFilesList.Count <= 0)
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgAddFileNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				OutputFormat format = this.getOutputFormat();
				if (format == OutputFormat.Invalid)
				{
					ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgInvalidForat, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					bool? viewFile = this.viewFileAfterConvertCB.IsChecked;
					bool? isChecked = this.OCRCB.IsChecked;
					bool? isChecked2 = this.ConvertOnlineCB.IsChecked;
					bool? isChecked3 = this.SingleSheetBtn.IsChecked;
					ConvertFileItem fileItemToView = null;
					if (isChecked.GetValueOrDefault())
					{
						this.OCRInit();
					}
					int selectCount = 0;
					int convertSucc = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ConvertFileItem> enumerator = this.convertFilesList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ConvertFileItem it = enumerator.Current;
							bool? fileSelected = it.FileSelected;
							bool flag = false;
							if (!((fileSelected.GetValueOrDefault() == flag) & (fileSelected != null)) && !string.IsNullOrWhiteSpace(it.convertFile))
							{
								int num = selectCount;
								selectCount = num + 1;
								it.WithOCR = isChecked;
								it.SingleSheet = isChecked3;
								it.ConvertStatus = FileCovertStatus.ConvertCoverting;
								string destDir = this.getUserSetOutputPath(it.convertFile);
								if (string.IsNullOrWhiteSpace(destDir))
								{
									it.ConvertStatus = FileCovertStatus.ConvertFail;
								}
								else
								{
									bool IsOnline = this.ConvertOnlineCB.IsChecked.Value;
									Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
									{
										GAManager.SendEvent("PDFConvert", "ConvertFile", App.convertType.ToString(), 1L);
										bool flag2 = false;
										try
										{
											flag2 = this.doConvert(it, format, destDir, IsOnline);
										}
										catch
										{
										}
										if (flag2)
										{
											GAManager.SendEvent("PDFConvert", "ConvertSucc", App.convertType.ToString(), 1L);
											it.ConvertStatus = FileCovertStatus.ConvertSucc;
											int convertSucc2 = convertSucc;
											convertSucc = convertSucc2 + 1;
											if (viewFile.GetValueOrDefault() && fileItemToView == null)
											{
												fileItemToView = it;
											}
										}
										else
										{
											GAManager.SendEvent("PDFConvert", "ConvertFail", App.convertType.ToString(), 1L);
											it.ConvertStatus = FileCovertStatus.ConvertFail;
										}
										this.actionAfterConvert(it, viewFile);
									}));
									list.Add(task);
								}
							}
						}
					}
					await Task.WhenAll(list).ConfigureAwait(true);
					if (selectCount <= 0)
					{
						ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgAddFileOneFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					else
					{
						GAManager.SendEvent("ConvertMainWin", "ConvertFileCount", selectCount.ToString(), 1L);
						if (viewFile.GetValueOrDefault() && fileItemToView != null)
						{
							if (!fileItemToView.outputFileIsDir)
							{
								UtilsManager.OpenFileInExplore(fileItemToView.outputFile, true);
							}
							else
							{
								UtilsManager.OpenFileInExplore(fileItemToView.outputFile, false);
							}
						}
						if (convertSucc != selectCount && MessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgConvertFailSupport, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
						{
							FeedbackWindow feedbackWindow = new FeedbackWindow();
							if (feedbackWindow != null)
							{
								try
								{
									foreach (ConvertFileItem convertFileItem in this.convertFilesList)
									{
										if (convertFileItem.ConvertStatus == FileCovertStatus.ConvertFail)
										{
											feedbackWindow.flist.Add(convertFileItem.convertFile);
										}
									}
								}
								catch
								{
								}
								feedbackWindow.source = "2";
								feedbackWindow.Owner = (MainWindow)Application.Current.MainWindow;
								feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
								feedbackWindow.showAttachmentCB(true);
								feedbackWindow.ShowDialog();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004E68 File Offset: 0x00003068
		private OutputFormat getOutputFormat()
		{
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_docx)
			{
				return OutputFormat.Docx;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_rtf)
			{
				return OutputFormat.Rtf;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_xls)
			{
				return OutputFormat.Xls;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_html)
			{
				return OutputFormat.Html;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_xml)
			{
				return OutputFormat.Xml;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_text)
			{
				return OutputFormat.Text;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_png)
			{
				return OutputFormat.Png;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_jpeg)
			{
				return OutputFormat.Jpeg;
			}
			if (this.outputFormatCB.SelectedItem == this.outputCBItem_pptx)
			{
				return OutputFormat.Ppt;
			}
			return OutputFormat.Invalid;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00004F34 File Offset: 0x00003134
		private string getUserSetOutputPath(string srcPdf)
		{
			if (string.IsNullOrWhiteSpace(srcPdf))
			{
				return null;
			}
			if (!string.IsNullOrWhiteSpace(this.outputPathTB.Text) && Directory.Exists(this.outputPathTB.Text))
			{
				ConfigManager.SetConvertPath(this.outputPathTB.Text);
				return this.outputPathTB.Text;
			}
			if (!string.IsNullOrWhiteSpace(this.outputPathTB.Text))
			{
				LongPathDirectory longPathDirectory = this.outputPathTB.Text;
				if (!Directory.Exists(longPathDirectory))
				{
					Directory.CreateDirectory(longPathDirectory);
				}
				ConfigManager.SetConvertPath(longPathDirectory.FullPathWithoutPrefix);
				return longPathDirectory.FullPathWithoutPrefix;
			}
			return null;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004FDC File Offset: 0x000031DC
		private string getVaildOutputFile(string fileNameWithoutExt, OutputFormat format, string destDir)
		{
			if (string.IsNullOrWhiteSpace(fileNameWithoutExt) || string.IsNullOrWhiteSpace(destDir))
			{
				return null;
			}
			string outputExt = ConvertManager.getOutputExt(format);
			if (string.IsNullOrWhiteSpace(outputExt))
			{
				return null;
			}
			LongPathFile longPathFile = Path.Combine(destDir, fileNameWithoutExt + " conv" + outputExt);
			if (!File.Exists(longPathFile) && !Directory.Exists(longPathFile))
			{
				return longPathFile.FullPathWithoutPrefix;
			}
			for (int i = 1; i < 100; i++)
			{
				longPathFile = Path.Combine(destDir, string.Format("{0} conv {1:0}{2}", fileNameWithoutExt, i, outputExt));
				if (!File.Exists(longPathFile) && !Directory.Exists(longPathFile))
				{
					return longPathFile.FullPathWithoutPrefix;
				}
			}
			return null;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005094 File Offset: 0x00003294
		private string getValidOutputDir(string dirName, string destDir)
		{
			if (string.IsNullOrWhiteSpace(dirName) || string.IsNullOrWhiteSpace(destDir))
			{
				return null;
			}
			string text = dirName + " conv";
			for (int i = 1; i < 100; i++)
			{
				LongPathDirectory longPathDirectory = Path.Combine(destDir, text);
				if (!Directory.Exists(longPathDirectory) && !File.Exists(longPathDirectory))
				{
					return longPathDirectory.FullPathWithoutPrefix;
				}
				text = dirName + " conv" + string.Format(" {0}", i);
			}
			return null;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005118 File Offset: 0x00003318
		private string getVaildSubFile(string fileNameWithoutExt, OutputFormat format, string destDir, int index)
		{
			if (string.IsNullOrWhiteSpace(fileNameWithoutExt) || string.IsNullOrWhiteSpace(destDir))
			{
				return null;
			}
			string outputExt = ConvertManager.getOutputExt(format);
			if (string.IsNullOrWhiteSpace(outputExt))
			{
				return null;
			}
			string text = string.Concat(new string[]
			{
				destDir,
				"\\",
				fileNameWithoutExt,
				" conv",
				string.Format(" {0}", index),
				outputExt
			});
			if (!File.Exists(text) && !Directory.Exists(text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005197 File Offset: 0x00003397
		private void actionAfterConvert(ConvertFileItem item, bool? viewFile)
		{
			if (item == null)
			{
				return;
			}
			item.FileSelected = new bool?(false);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000051AC File Offset: 0x000033AC
		private bool doConvert(ConvertFileItem item, OutputFormat format, string destDir, bool Online)
		{
			if (string.IsNullOrWhiteSpace(destDir))
			{
				return false;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.convertFile);
			if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
			{
				return false;
			}
			FileInfo fileInfo = new FileInfo(item.convertFile);
			if (fileInfo != null && fileInfo.Exists && fileInfo.Length > 0L)
			{
				GAManager.SendEvent("PDFFSize", "ConvertFromPDF", ((int)(fileInfo.Length / 1024L / 1024L / 10L)).ToString(), 1L);
			}
			if (format == OutputFormat.Docx || format == OutputFormat.Rtf || format == OutputFormat.Xls || format == OutputFormat.Text || format == OutputFormat.Html || format == OutputFormat.Xml || format == OutputFormat.Ppt)
			{
				LongPathFile longPathFile = this.getVaildOutputFile(fileNameWithoutExtension, format, destDir);
				if (string.IsNullOrWhiteSpace(longPathFile))
				{
					return false;
				}
				bool flag;
				if (!Online)
				{
					flag = this.doConvert_OutputSingleFile(format, item.convertFile, longPathFile, item.PageFrom, item.PageTo, item.WithOCR, item.PassWord, item.SingleSheet);
					if (!flag && MessageBox.Show(pdfconverter.Properties.Resources.ConvertFailedByLocalTipMsg, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
					{
						this.ConvertOnlineCB.Dispatcher.Invoke(delegate
						{
							this.ConvertOnlineCB.IsChecked = new bool?(true);
						});
						flag = this.doConvertOnline_SingleFile(item, format, longPathFile, "").Result;
					}
				}
				else
				{
					flag = this.doConvertOnline_SingleFile(item, format, longPathFile, "").Result;
					if (!flag)
					{
						flag = this.doConvert_OutputSingleFile(format, item.convertFile, longPathFile, item.PageFrom, item.PageTo, item.WithOCR, item.PassWord, item.SingleSheet);
					}
				}
				if (flag)
				{
					item.outputFile = longPathFile;
					item.outputFileIsDir = false;
					return true;
				}
				if (format == OutputFormat.Xls)
				{
					MessageBox.Show(pdfconverter.Properties.Resources.MainWinConvertToExcelFaildSuggest);
				}
				return false;
			}
			else
			{
				if (format != OutputFormat.Png && format != OutputFormat.Jpeg)
				{
					return false;
				}
				string validOutputDir = this.getValidOutputDir(fileNameWithoutExtension.Trim(), destDir);
				if (string.IsNullOrWhiteSpace(validOutputDir))
				{
					return false;
				}
				try
				{
					Directory.CreateDirectory(validOutputDir);
				}
				catch
				{
					return false;
				}
				bool flag2;
				if (!Online)
				{
					flag2 = this.doConvert_OutputMultiFiles(format, item.convertFile, fileNameWithoutExtension, validOutputDir, item.PageFrom, item.PageTo, item.PassWord);
					if (!flag2 && MessageBox.Show(pdfconverter.Properties.Resources.ConvertFailedByLocalTipMsg, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
					{
						this.ConvertOnlineCB.Dispatcher.Invoke(delegate
						{
							this.ConvertOnlineCB.IsChecked = new bool?(true);
						});
						flag2 = this.doConvertOnline_SingleFile(item, format, validOutputDir, fileNameWithoutExtension).Result;
					}
				}
				else
				{
					flag2 = this.doConvertOnline_SingleFile(item, format, validOutputDir, fileNameWithoutExtension).Result;
					if (!flag2)
					{
						flag2 = this.doConvert_OutputMultiFiles(format, item.convertFile, fileNameWithoutExtension, validOutputDir, item.PageFrom, item.PageTo, item.PassWord);
					}
				}
				if (flag2)
				{
					item.outputFile = validOutputDir;
					item.outputFileIsDir = true;
					return true;
				}
				return false;
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00005498 File Offset: 0x00003698
		private async Task<bool> doConvertOnline_SingleFile(ConvertFileItem item, OutputFormat format, string destDir, string fileNameWithOutExtension = "")
		{
			bool flag;
			try
			{
				if (string.IsNullOrWhiteSpace(destDir) || !string.IsNullOrEmpty(item.PassWord))
				{
					flag = false;
				}
				else
				{
					bool debugFlag = ConfigManager.GetDebugMode();
					ConnectModel model = this.OCRInitParaOnline(item);
					string text = await this.SplitFileIfNeed(model, item);
					string splitFilePath = text;
					GAManager.SendEvent("OnlineConvert", "RequestService", format.ToString(), 1L);
					OnlineAuthResponsModel onlineAuthResponsModel = await OnlineConvertUtils.IsServiceOnline(model);
					if (onlineAuthResponsModel.Success)
					{
						if (debugFlag)
						{
							MessageBox.Show("Debug Message:Connect Service successful.");
						}
						string temPath = item.convertFile;
						if (!string.IsNullOrEmpty(splitFilePath))
						{
							temPath = splitFilePath;
						}
						GAManager.SendEvent("OnlineConvert", "RequestConvert", format.ToString(), 1L);
						bool flag2 = await OnlineConvertUtils.RequestConvertFile(temPath, destDir, onlineAuthResponsModel.Token, model, fileNameWithOutExtension);
						ConfigManager.AddOnlineRequestCount();
						if (!flag2)
						{
							GAManager.SendEvent("OnlineConvert", "RequestConvertFail", format.ToString(), 1L);
							if (debugFlag)
							{
								MessageBox.Show("Debug Message:Convert file online fail.");
							}
							if (Directory.Exists(destDir))
							{
								Directory.Delete(destDir, true);
								Directory.CreateDirectory(destDir);
							}
						}
						if (model.pageFrom != 1 || model.pageTo != model.pageCount)
						{
							File.Delete(temPath);
						}
						flag = flag2;
					}
					else
					{
						if (debugFlag)
						{
							MessageBox.Show("Debug Message:Connect Service fail.");
						}
						flag = false;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000054FC File Offset: 0x000036FC
		private bool doConvert_OutputSingleFile(OutputFormat format, string srcFile, string outputFile, int fromPage, int toPage, bool? withOCR, string password, bool? singleSheet)
		{
			PdfFocus pdfFocus = new PdfFocus();
			pdfFocus.Serial = ConvertSDK.SautinSoftSerial;
			if (!string.IsNullOrEmpty(password))
			{
				pdfFocus.Password = password;
			}
			pdfFocus.OpenPdf(srcFile);
			int pageCount = pdfFocus.PageCount;
			if (pageCount <= 0)
			{
				pdfFocus.ClosePdf();
				return false;
			}
			if (withOCR.GetValueOrDefault())
			{
				pdfFocus.OCROptions.Method = new PdfFocus.COCROptions.OCRMethod(this.PerformOCRNicomsoft);
				pdfFocus.OCROptions.Mode = PdfFocus.COCROptions.eOCRMode.AllImages;
				pdfFocus.WordOptions.KeepCharScaleAndSpacing = false;
			}
			int num = -1;
			if (format == OutputFormat.Docx)
			{
				pdfFocus.WordOptions.Format = PdfFocus.CWordOptions.eWordDocument.Docx;
				num = pdfFocus.ToWord(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Rtf)
			{
				pdfFocus.WordOptions.Format = PdfFocus.CWordOptions.eWordDocument.Rtf;
				num = pdfFocus.ToWord(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Xls)
			{
				pdfFocus.ExcelOptions.SingleSheet = singleSheet.Value;
				num = pdfFocus.ToExcel(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Html)
			{
				num = pdfFocus.ToHtml(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Xml)
			{
				num = pdfFocus.ToXml(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Text)
			{
				num = pdfFocus.ToText(outputFile, fromPage, toPage);
			}
			else if (format == OutputFormat.Ppt)
			{
				SizeF sizeF = default(SizeF);
				bool flag = false;
				bool flag2 = false;
				for (int i = fromPage; i <= toPage; i++)
				{
					SizeF pageSize = pdfFocus.GetPageSize(i);
					if (pageSize.Width > sizeF.Width && !flag2)
					{
						if ((double)pageSize.Width > MainWindow.PPTMaximum)
						{
							sizeF.Width = (float)MainWindow.PPTMaximum;
							flag2 = true;
						}
						else
						{
							sizeF.Width = pageSize.Width;
						}
					}
					if (pageSize.Height > sizeF.Height && !flag)
					{
						if ((double)pageSize.Height > MainWindow.PPTMaximum)
						{
							sizeF.Height = (float)MainWindow.PPTMaximum;
							flag = true;
						}
						else
						{
							sizeF.Height = pageSize.Height;
						}
					}
					if (flag && flag2)
					{
						break;
					}
				}
				IPresentation presentation = Presentation.Create();
				presentation.Masters[0].SlideSize.Width = (double)sizeF.Width;
				presentation.Masters[0].SlideSize.Height = (double)sizeF.Height;
				try
				{
					pdfFocus.ImageOptions.ImageFormat = ImageFormat.Png;
					pdfFocus.ImageOptions.Dpi = MainWindow.PDFToImageDPI;
					int num2 = 0;
					for (int j = fromPage; j <= toPage; j++)
					{
						try
						{
							ISlide slide = presentation.Slides.Add(SlideLayoutType.Blank);
							byte[] array = pdfFocus.ToImage(j);
							SizeF pageSize2 = pdfFocus.GetPageSize(j);
							double num3 = (presentation.Masters[0].SlideSize.Width - (double)pageSize2.Width) / 2.0;
							double num4 = (presentation.Masters[0].SlideSize.Height - (double)pageSize2.Height) / 2.0;
							using (Stream stream = new MemoryStream(array))
							{
								slide.Pictures.AddPicture(stream, num3, num4, (double)pageSize2.Width, (double)pageSize2.Height);
							}
							num2++;
						}
						catch
						{
						}
					}
					num = ((num2 == 0) ? (-1) : 0);
				}
				catch
				{
					num = -1;
				}
				finally
				{
					presentation.Save(outputFile);
					presentation.Close();
					presentation.Dispose();
					presentation = null;
				}
			}
			pdfFocus.ClosePdf();
			GAManager.SendEvent("ConvertPages", "PageCount", (toPage - fromPage + 1).ToString(), 1L);
			if (fromPage == 1 && toPage == pageCount)
			{
				GAManager.SendEvent("ConvertPages", "WholePages", format.ToString(), 1L);
			}
			else
			{
				GAManager.SendEvent("ConvertPages", "PartPages", format.ToString(), 1L);
			}
			if (num == 0 && File.Exists(outputFile))
			{
				GAManager.SendEvent("ConvertApi", "Succ", format.ToString(), 1L);
				return true;
			}
			GAManager.SendEvent("ConvertApi", "Fail", format.ToString(), 1L);
			GAManager.SendEvent("SDKConvertError", format.ToString(), num.ToString(), 1L);
			return false;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00005998 File Offset: 0x00003B98
		private bool doConvert_OutputMultiFiles(OutputFormat format, string srcFile, string fileNameWithoutExt, string outputDir, int fromPage, int toPage, string password)
		{
			PdfFocus pdfFocus = new PdfFocus();
			pdfFocus.Serial = ConvertSDK.SautinSoftSerial;
			if (!string.IsNullOrEmpty(password))
			{
				pdfFocus.Password = password;
			}
			pdfFocus.OpenPdf(srcFile);
			int pageCount = pdfFocus.PageCount;
			if (pageCount <= 0)
			{
				pdfFocus.ClosePdf();
				return false;
			}
			bool flag = false;
			if (format == OutputFormat.Png)
			{
				pdfFocus.ImageOptions.ImageFormat = ImageFormat.Png;
				pdfFocus.ImageOptions.Dpi = MainWindow.PDFToImageDPI;
			}
			else if (format == OutputFormat.Jpeg)
			{
				pdfFocus.ImageOptions.ImageFormat = ImageFormat.Jpeg;
				pdfFocus.ImageOptions.Dpi = MainWindow.PDFToImageDPI;
			}
			for (int i = fromPage; i <= toPage; i++)
			{
				string vaildSubFile = this.getVaildSubFile(fileNameWithoutExt, format, outputDir, i);
				if (!string.IsNullOrWhiteSpace(vaildSubFile))
				{
					int num = pdfFocus.ToImage(vaildSubFile, i);
					if (num == 0 && File.Exists(vaildSubFile))
					{
						flag = true;
					}
					else
					{
						GAManager.SendEvent("SDKConvertError", format.ToString(), num.ToString(), 1L);
					}
				}
			}
			pdfFocus.ClosePdf();
			GAManager.SendEvent("ConvertPages", "PageCount", (toPage - fromPage + 1).ToString(), 1L);
			if (fromPage == 1 && toPage == pageCount)
			{
				GAManager.SendEvent("ConvertPages", "WholePages", format.ToString(), 1L);
			}
			else
			{
				GAManager.SendEvent("ConvertPages", "PartPages", format.ToString(), 1L);
			}
			if (flag)
			{
				GAManager.SendEvent("ConvertApi", "Succ", format.ToString(), 1L);
			}
			else
			{
				GAManager.SendEvent("ConvertApi", "Fail", format.ToString(), 1L);
			}
			return flag;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00005B44 File Offset: 0x00003D44
		private void updateFileItemListCBStatus()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool? flag = new bool?(false);
			foreach (ConvertFileItem convertFileItem in this.convertFilesList)
			{
				if (convertFileItem.ConvertStatus != FileCovertStatus.ConvertUnsupport)
				{
					num3++;
					if (convertFileItem.FileSelected.GetValueOrDefault())
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
			}
			if (num3 <= 0)
			{
				flag = new bool?(false);
			}
			else if (num3 == num)
			{
				flag = new bool?(true);
			}
			else if (num3 == num2)
			{
				flag = new bool?(false);
			}
			else
			{
				flag = null;
			}
			if (this.fileItemListCB != null)
			{
				this.fileItemListCB.IsChecked = flag;
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00005C10 File Offset: 0x00003E10
		private void FileItemCB_Checked(object sender, RoutedEventArgs e)
		{
			this.updateFileItemListCBStatus();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00005C18 File Offset: 0x00003E18
		private void FileItemCB_Unchecked(object sender, RoutedEventArgs e)
		{
			this.updateFileItemListCBStatus();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00005C20 File Offset: 0x00003E20
		private void FileItemListCB_Click(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			bool? isChecked = checkBox.IsChecked;
			if (isChecked == null)
			{
				checkBox.IsChecked = new bool?(false);
				isChecked = new bool?(false);
			}
			foreach (ConvertFileItem convertFileItem in this.convertFilesList)
			{
				if (convertFileItem.ConvertStatus != FileCovertStatus.ConvertUnsupport && convertFileItem.ConvertStatus != FileCovertStatus.ConvertLoadedFailed)
				{
					if (isChecked.GetValueOrDefault())
					{
						bool? fileSelected = convertFileItem.FileSelected;
						bool flag = false;
						if ((fileSelected.GetValueOrDefault() == flag) & (fileSelected != null))
						{
							convertFileItem.FileSelected = new bool?(true);
						}
					}
					else if (convertFileItem.FileSelected.GetValueOrDefault())
					{
						convertFileItem.FileSelected = new bool?(false);
					}
				}
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00005CF8 File Offset: 0x00003EF8
		private void FileItemListCB_Loaded(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			this.fileItemListCB = checkBox;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00005D14 File Offset: 0x00003F14
		private void OCRInit()
		{
			TNSOCR.Engine_SetLicenseKey("AB2A4DD5FF2A");
			TNSOCR.Engine_InitializeAdvanced(out this.CfgObj, out this.OcrObj, out this.ImgObj);
			this.NsOCRLanguage = ConvertManager.getOCRLanguage();
			if (this.ConvertOnlineCB.IsChecked.GetValueOrDefault())
			{
				this.NsOCROnlineLanguage = ConvertManager.GetOCROnlineLanguage();
			}
			if (!string.IsNullOrWhiteSpace(this.NsOCRLanguage))
			{
				TNSOCR.Cfg_SetOption(this.CfgObj, 0, this.NsOCRLanguage, "1");
				return;
			}
			TNSOCR.Cfg_SetOption(this.CfgObj, 0, "Languages/English", "1");
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00005DAC File Offset: 0x00003FAC
		private ConnectModel OCRInitParaOnline(ConvertFileItem item)
		{
			ConnectModel connectModel = new ConnectModel();
			try
			{
				connectModel.appVersion = UtilManager.GetAppVersion();
				connectModel.utcTimestamp = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
				connectModel.convertType = MapOnlineConverttype.GetOnlineConvertStr((ConvFromPDFType)App.convertType);
				connectModel.uuid = UtilManager.GetUUID();
				connectModel.fileName = Path.GetFileName(item.convertFile);
				connectModel.convertCountToday = ConfigManager.GetOnlineRequestCount() + 1;
				connectModel.fileSize = new FileInfo(item.convertFile).Length;
				connectModel.pageCount = item.PageCount;
				connectModel.pageFrom = item.PageFrom;
				connectModel.pageTo = item.PageTo;
				connectModel.needOcr = item.WithOCR.Value;
				connectModel.OcrLang = this.NsOCROnlineLanguage;
			}
			catch
			{
				return null;
			}
			return connectModel;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00005E94 File Offset: 0x00004094
		private async Task<string> SplitFileIfNeed(ConnectModel model, ConvertFileItem item)
		{
			string text2;
			if ((model.pageFrom != 1 || model.pageTo != model.pageCount) && model.pageFrom != 0)
			{
				string text = await TempFileUtils.SplitFileInRangeAsync(item.convertFile, model.pageFrom, model.pageTo);
				model.fileSize = text.FileInfo.Length;
				text2 = text;
			}
			else
			{
				text2 = null;
			}
			return text2;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00005EE0 File Offset: 0x000040E0
		public byte[] PerformOCRNicomsoft(byte[] image)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			TNSOCR.Engine_SetLicenseKey("AB2A4DD5FF2A");
			TNSOCR.Engine_InitializeAdvanced(out num, out num2, out num3);
			TNSOCR.Cfg_SetOption(num, 0, "ImgAlizer/AutoScale", "0");
			TNSOCR.Cfg_SetOption(num, 0, "ImgAlizer/ScaleFactor", "4.0");
			this.NsOCRLanguage = ConvertManager.getOCRLanguage();
			if (!string.IsNullOrWhiteSpace(this.NsOCRLanguage))
			{
				TNSOCR.Cfg_SetOption(num, 0, this.NsOCRLanguage, "1");
			}
			else
			{
				TNSOCR.Cfg_SetOption(num, 0, "Languages/English", "1");
			}
			Array array = null;
			using (MemoryStream memoryStream = new MemoryStream(image))
			{
				memoryStream.Flush();
				array = memoryStream.ToArray();
			}
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			int num5 = TNSOCR.Img_LoadFromMemory(num3, intPtr, array.Length);
			gchandle.Free();
			if (num5 > 1879048192)
			{
				if (num3 != 0)
				{
					TNSOCR.Img_Unload(num3);
					TNSOCR.Img_Destroy(num3);
					num3 = 0;
				}
				return null;
			}
			TNSOCR.Svr_Create(num, 1, out num4);
			TNSOCR.Svr_NewDocument(num4);
			if (TNSOCR.Img_OCR(num3, 0, 255, 0) > 1879048192)
			{
				if (num3 != 0)
				{
					TNSOCR.Img_Unload(num3);
					TNSOCR.Img_Destroy(num3);
					num3 = 0;
				}
				return null;
			}
			if (TNSOCR.Svr_AddPage(num4, num3, 1) > 1879048192)
			{
				if (num3 != 0)
				{
					TNSOCR.Img_Unload(num3);
					TNSOCR.Img_Destroy(num3);
					num3 = 0;
				}
				return null;
			}
			if (num3 != 0)
			{
				TNSOCR.Img_Unload(num3);
				TNSOCR.Img_Destroy(num3);
				num3 = 0;
			}
			int num6 = TNSOCR.Svr_SaveToMemory(num4, (IntPtr)0, 0);
			if (num6 <= 0)
			{
				if (num4 != 0)
				{
					TNSOCR.Svr_Destroy(num4);
					num4 = 0;
				}
				return null;
			}
			byte[] array2 = new byte[num6];
			GCHandle gchandle2 = GCHandle.Alloc(array2, GCHandleType.Pinned);
			IntPtr intPtr2 = gchandle2.AddrOfPinnedObject();
			int num7 = TNSOCR.Svr_SaveToMemory(num4, intPtr2, num6);
			gchandle2.Free();
			if (num4 != 0)
			{
				TNSOCR.Svr_Destroy(num4);
				num4 = 0;
			}
			if (num7 > 1879048192)
			{
				return null;
			}
			return array2;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000060C4 File Offset: 0x000042C4
		private bool showOCRSettingsWindow()
		{
			OCRSettingsWindow ocrsettingsWindow = new OCRSettingsWindow();
			ocrsettingsWindow.Owner = this;
			ocrsettingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			ocrsettingsWindow.ShowDialog();
			return ocrsettingsWindow.ret;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000060E8 File Offset: 0x000042E8
		private void OCRCB_Checked(object sender, RoutedEventArgs e)
		{
			if (this.showOCRSettingsWindow())
			{
				string ocrlanguageL10N = ConvertManager.getOCRLanguageL10N();
				this.curLang.Text = ocrlanguageL10N;
				this.curLangLink.Foreground = new SolidColorBrush(Colors.Red);
				this.curLangLink.IsEnabled = true;
				this.OCRCB.IsChecked = new bool?(true);
				return;
			}
			this.OCRCB.IsChecked = new bool?(false);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00006153 File Offset: 0x00004353
		private void OCRCB_Unchecked(object sender, RoutedEventArgs e)
		{
			this.curLangLink.Foreground = new SolidColorBrush(Colors.Gray);
			this.curLangLink.IsEnabled = false;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006178 File Offset: 0x00004378
		private void CurLang_Click(object sender, RoutedEventArgs e)
		{
			if (this.OCRCB.IsChecked.GetValueOrDefault() && this.showOCRSettingsWindow())
			{
				string ocrlanguageL10N = ConvertManager.getOCRLanguageL10N();
				this.curLang.Text = ocrlanguageL10N;
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000646C File Offset: 0x0000466C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 5:
				((CheckBox)target).Loaded += this.FileItemListCB_Loaded;
				((CheckBox)target).Click += this.FileItemListCB_Click;
				return;
			case 6:
				((Button)target).Click += this.ClearFilesBtn_Click;
				return;
			case 7:
				((CheckBox)target).Checked += this.FileItemCB_Checked;
				((CheckBox)target).Unchecked += this.FileItemCB_Unchecked;
				return;
			case 8:
				((Button)target).Click += this.OpenFileBtn_Click;
				return;
			case 9:
				((Button)target).Click += this.OpenFileInExploreBtn_Click;
				return;
			case 10:
				((Button)target).Click += this.DeleteFileBtn_Click;
				return;
			default:
				return;
			}
		}

		// Token: 0x04000104 RID: 260
		internal int CfgObj;

		// Token: 0x04000105 RID: 261
		internal int OcrObj;

		// Token: 0x04000106 RID: 262
		internal int ImgObj;

		// Token: 0x04000107 RID: 263
		internal int ScanObj;

		// Token: 0x04000108 RID: 264
		internal int SvrObj;

		// Token: 0x04000109 RID: 265
		internal bool OCRCreated;

		// Token: 0x0400010A RID: 266
		private string NsOCROnlineLanguage;

		// Token: 0x0400010B RID: 267
		private static readonly double PPTMaximum = 4032.0;

		// Token: 0x0400010C RID: 268
		private static readonly int PDFToImageDPI = 200;

		// Token: 0x0400010D RID: 269
		private string NsOCRLanguage;

		// Token: 0x0400010E RID: 270
		private ObservableCollection<ConvertFileItem> convertFilesList = new ObservableCollection<ConvertFileItem>();

		// Token: 0x0400010F RID: 271
		private CheckBox fileItemListCB;
	}
}
