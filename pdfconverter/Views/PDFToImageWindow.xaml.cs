using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Controls;
using CommonLib.Controls.ColorPickers;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Exceptions;
using pdfconverter.Properties;
using pdfconverter.Utils;
using PDFKit.ExtractPdfImage;
using PDFKit.Utils;

namespace pdfconverter.Views
{
	// Token: 0x0200002B RID: 43
	public partial class PDFToImageWindow : Window
	{
		// Token: 0x0600024A RID: 586 RVA: 0x00008FB4 File Offset: 0x000071B4
		public PDFToImageWindow()
		{
			this.InitializeComponent();
			this.InitializePDFConvert();
			base.Closing += this.PDFToImageWindow_Closing;
			GAManager.SendEvent("PDFToImageWindow", "Show", "Count", 1L);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00009194 File Offset: 0x00007394
		private void PDFToImageWindow_Closing(object sender, CancelEventArgs e)
		{
			ObservableCollection<PDFToImageItem> convertFilesList = this.ConvertFilesList;
			if (convertFilesList == null)
			{
				return;
			}
			convertFilesList.Clear();
		}

		// Token: 0x0600024C RID: 588 RVA: 0x000091A8 File Offset: 0x000073A8
		private void InitializePDFConvert()
		{
			this.IsFinishInitializeComponent = true;
			object convertType = App.convertType;
			if (convertType is ConvFromPDFType && (ConvFromPDFType)convertType == ConvFromPDFType.PDFToJpg)
			{
				this.CB_OutputFormat.SelectedIndex = 1;
			}
			else
			{
				this.CB_OutputFormat.SelectedIndex = 0;
			}
			this.CB_Quality.SelectedIndex = ConfigManager.GetPDFToImageDPISelectedIndex();
			if (!ConfigManager.PDFToImageIsEntire())
			{
				this.Rd_SingleImage.IsChecked = new bool?(true);
			}
			else
			{
				this.Rd_EntireImage.IsChecked = new bool?(true);
			}
			this.NB_Zoom.Value = ConfigManager.GetPDFToImageZoom();
			this.Btn_ColorPicker.SelectedColor = ConfigManager.GetPDFToImageBorderColor();
			this.NB_BorderThickness.Value = ConfigManager.GetPDFToImageBorderThickness();
			if (ConfigManager.GetPDFToImageColorModeSelectedIndex() == 0)
			{
				this.Rd_ColorMode_RGB.IsChecked = new bool?(true);
			}
			else
			{
				this.Rd_ColorMode_Gray.IsChecked = new bool?(true);
			}
			this.CB_RenderAnnotation.IsChecked = new bool?(ConfigManager.IsRenderAnnotations());
			string text = string.Empty;
			try
			{
				text = ConfigManager.GetConvertPath();
				if (string.IsNullOrEmpty(text) || !Directory.Exists(text))
				{
					text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\PDFgear";
				}
			}
			catch
			{
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.TB_OutputPath.Text = text;
			}
			if (App.selectedFile != null && App.selectedFile.Length != 0)
			{
				this.AddPDFFilesToConvertList(App.selectedFile, App.seletedPassword);
			}
			this.ListView_Document.ItemsSource = this.ConvertFilesList;
			this.IsFinishInitializePDFConvert = true;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00009330 File Offset: 0x00007530
		private void AddDocument(PDFToImageItem item)
		{
			this.ConvertFilesList.Add(item);
			if (this.ListView_Document.SelectedItem == null)
			{
				this.ListView_Document.SelectedItem = item;
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00009358 File Offset: 0x00007558
		private void AddPDFFilesToConvertList(string[] files, string[] passwords = null)
		{
			if (files != null && files.Length != 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					LongPathFile longPathFile = files[i];
					string text = "";
					if (passwords != null && passwords.Length != 0)
					{
						text = passwords[i];
					}
					if (!string.IsNullOrWhiteSpace(longPathFile))
					{
						bool? flag = ConToPDFUtils.CheckAccess(longPathFile);
						bool flag2 = false;
						if ((flag.GetValueOrDefault() == flag2) & (flag != null))
						{
							flag = new FileOccupation(longPathFile).ShowDialog();
							flag2 = false;
							if ((flag.GetValueOrDefault() == flag2) & (flag != null))
							{
								goto IL_0133;
							}
						}
						if (UtilsManager.IsPDFFile(longPathFile))
						{
							if (!ConvertManager.IsFileAdded(this.ConvertFilesList, longPathFile))
							{
								PDFToImageItem pdftoImageItem = new PDFToImageItem(longPathFile);
								this.AddDocument(pdftoImageItem);
								if (ConToPDFUtils.CheckPassword(longPathFile, ref text, this))
								{
									pdftoImageItem.ParseFile(text, delegate
									{
										Application.Current.Dispatcher.Invoke(delegate
										{
											this.UpdateCB_FileItemListStatus();
										});
									});
								}
								else
								{
									pdftoImageItem.FileSelected = new bool?(false);
									pdftoImageItem.ConvertStatus = FileCovertStatus.ConvertLoadedFailed;
									this.UpdateCB_FileItemListStatus();
								}
							}
							else
							{
								ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgFileAdded, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
						}
						else
						{
							ModernMessageBox.Show(pdfconverter.Properties.Resources.PDFToImageConverterMsgOnlyPDF, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						}
					}
					IL_0133:;
				}
			}
		}

		// Token: 0x0600024F RID: 591 RVA: 0x000094A8 File Offset: 0x000076A8
		private void Btn_AddFiles_Click(object sender, RoutedEventArgs e)
		{
			string[] array = ConvertManager.selectMultiPDFFiles();
			if (array != null && array.Length != 0)
			{
				GAManager.SendEvent("PDFToImageWindow", "AddFileBtn", array.Length.ToString(), 1L);
				this.AddPDFFilesToConvertList(array, null);
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x000094E8 File Offset: 0x000076E8
		private void Btn_ClearFiles_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PDFToImageWindow", "ClearBtn", "Count", 1L);
			if (this.ConvertFilesList.Count((PDFToImageItem f) => f.FileSelected.GetValueOrDefault()) == 0)
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.MainWinOthersToPDFDeleteNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (ModernMessageBox.Show(pdfconverter.Properties.Resources.WinMergeSplitClearFileAskMsg, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
			{
				this.ClearAllSelectedItems();
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00009568 File Offset: 0x00007768
		private void ClearAllSelectedItems()
		{
			try
			{
				ObservableCollection<PDFToImageItem> convertFilesList = this.ConvertFilesList;
				List<PDFToImageItem> list;
				if (convertFilesList == null)
				{
					list = null;
				}
				else
				{
					list = convertFilesList.Where((PDFToImageItem f) => f.FileSelected.GetValueOrDefault()).ToList<PDFToImageItem>();
				}
				foreach (PDFToImageItem pdftoImageItem in list)
				{
					this.ConvertFilesList.Remove(pdftoImageItem);
				}
				this.UpdateCB_FileItemListStatus();
			}
			catch
			{
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00009608 File Offset: 0x00007808
		private void Gd_BatchConvert_Drop(object sender, DragEventArgs e)
		{
			if (this.ConvertFilesList != null && e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (array != null && array.Length != 0)
				{
					GAManager.SendEvent("PDFToImageWindow", "AddFileDrop", array.Length.ToString(), 1L);
					this.AddPDFFilesToConvertList(array, null);
				}
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00009670 File Offset: 0x00007870
		private void Gd_BatchConvert_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000969C File Offset: 0x0000789C
		private void Btn_ChangeOutputPath_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PDFToImageWindow", "ChangeOutputPathBtn", "Count", 1L);
			string text = ConvertManager.selectOutputFolder(this.TB_OutputPath.Text);
			if (!string.IsNullOrWhiteSpace(text) && this.TB_OutputPath != null)
			{
				this.TB_OutputPath.Text = text;
				ConfigManager.SetConvertPath(this.TB_OutputPath.Text);
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000096FC File Offset: 0x000078FC
		private async void Btn_Convert_Click(object sender, RoutedEventArgs e)
		{
			if (this.ConvertFilesList == null || this.ConvertFilesList.Count <= 0)
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgAddFileNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				int convertSucc = 0;
				List<Task> list = new List<Task>();
				List<PDFToImageItem> selectedItems = this.ConvertFilesList.Where((PDFToImageItem t) => t.FileSelected.GetValueOrDefault() && !string.IsNullOrWhiteSpace(t.convertFile)).ToList<PDFToImageItem>();
				if (selectedItems.Count <= 0)
				{
					ModernMessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgAddFileOneFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else if (selectedItems.Any((PDFToImageItem t) => string.IsNullOrEmpty(t.PageRange)))
				{
					ModernMessageBox.Show(pdfconverter.Properties.Resources.PDFToImageConverterMsgPageRangeIsNull, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					PDFToImageItemArgs args = this.GetArgs();
					if (args == null)
					{
						ModernMessageBox.Show(pdfconverter.Properties.Resources.PDFToImageConverterMsgWrongArgs, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					else
					{
						GAManager.SendEvent("PDFToImageWindow", "ConvertBtn", selectedItems.Count.ToString(), 1L);
						GAManager.SendEvent("PDFToImageArgs", "OutputFormat", args.OutputFormat.ToString(), 1L);
						GAManager.SendEvent("PDFToImageArgs", "Quality", args.DPI.ToString(), 1L);
						GAManager.SendEvent("PDFToImageArgs", "OutputOneImage", args.IsEntire.ToString(), 1L);
						GAManager.SendEvent("PDFToImageArgs", "IncludeAnnotation", args.RenderAnnotations.ToString(), 1L);
						GAManager.SendEvent("PDFToImageArgs", "ColorMode", args.ColorMode.ToString(), 1L);
						Button btn = (Button)sender;
						btn.IsEnabled = false;
						using (List<PDFToImageItem>.Enumerator enumerator = selectedItems.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								PDFToImageItem item = enumerator.Current;
								Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
								{
									item.ConvertStatus = FileCovertStatus.ConvertCoverting;
									bool flag = false;
									try
									{
										flag = this.ExtractPages(item, args);
									}
									catch
									{
									}
									if (flag)
									{
										item.ConvertStatus = FileCovertStatus.ConvertSucc;
										int convertSucc2 = convertSucc;
										convertSucc = convertSucc2 + 1;
									}
									else
									{
										item.ConvertStatus = FileCovertStatus.ConvertFail;
									}
									this.ActionAfterConvert(item, new bool?(true));
								}));
								list.Add(task);
							}
						}
						await Task.WhenAll(list).ConfigureAwait(true);
						btn.IsEnabled = true;
						this.SelectItemsInexplorer(selectedItems);
						if (convertSucc != selectedItems.Count)
						{
							GAManager.SendEvent("PDFToImageWindow", "ConvertFail", "Count", 1L);
							if (MessageBox.Show(pdfconverter.Properties.Resources.FileConvertMsgConvertFailSupport, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
							{
								FeedbackWindow feedbackWindow = new FeedbackWindow();
								if (feedbackWindow != null)
								{
									try
									{
										foreach (PDFToImageItem pdftoImageItem in this.ConvertFilesList)
										{
											if (pdftoImageItem.ConvertStatus == FileCovertStatus.ConvertFail)
											{
												feedbackWindow.flist.Add(pdftoImageItem.convertFile);
											}
										}
									}
									catch
									{
									}
									feedbackWindow.source = "2";
									feedbackWindow.Owner = this;
									feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
									feedbackWindow.showAttachmentCB(true);
									feedbackWindow.ShowDialog();
								}
							}
						}
						else
						{
							GAManager.SendEvent("PDFToImageWindow", "ConvertSucc", "Count", 1L);
						}
					}
				}
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000973C File Offset: 0x0000793C
		private bool ExtractPages(PDFToImageItem item, PDFToImageItemArgs args)
		{
			LongPathFile longPathFile = item.convertFile;
			if (!longPathFile.IsExists)
			{
				item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByFileNotExist + " " + item.convertFile.FullPathWithoutPrefix;
				return false;
			}
			int result = PdfiumNetHelper.GetPageCountAsync(longPathFile, item.PassWord).GetAwaiter().GetResult();
			if (result == 0 || result != item.PageCount)
			{
				item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByPageRange;
				return false;
			}
			global::System.Collections.Generic.IReadOnlyList<PdfPageRange> readOnlyList;
			int num;
			if (!PdfPageRange.TryParsePageRange(item.PageRange, new PdfPageRange.PageRangeParseOptions
			{
				PageCount = item.PageCount,
				BaseIndex = 1
			}, out readOnlyList, out num))
			{
				item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByInvalidOutputFormat;
				return false;
			}
			string validOutputDir = this.GetValidOutputDir(item.FileNameWithoutExtenstion, args.OutputPath);
			if (string.IsNullOrEmpty(validOutputDir))
			{
				item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByInvalidOutputFormat;
				Log.WriteLog(item.convertFile + ":outputDir Is Null");
				return false;
			}
			try
			{
				Directory.CreateDirectory(validOutputDir);
			}
			catch
			{
				item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByCreateOutputDir;
				Log.WriteLog("Can't create the outputdir:" + item.convertFile);
				return false;
			}
			item.outputFile = validOutputDir;
			item.outputFileIsDir = true;
			bool flag;
			if (!args.IsEntire)
			{
				flag = this.ExtractSinglePages(item, readOnlyList, args);
			}
			else
			{
				flag = this.ExtractPagesToSingleImage(item, readOnlyList, args);
			}
			return flag;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000098BC File Offset: 0x00007ABC
		private bool ExtractSinglePages(PDFToImageItem item, global::System.Collections.Generic.IReadOnlyList<PdfPageRange> pageRanges, PDFToImageItemArgs args)
		{
			PdfPageImageExtractSettings pdfPageImageExtractSettings = new PdfPageImageExtractSettings
			{
				ImageDpi = args.DPI,
				ColorMode = args.ColorMode,
				RenderAnnotations = args.RenderAnnotations
			};
			try
			{
				LongPathFile longPathFile = item.convertFile;
				if (longPathFile.IsExists)
				{
					using (PdfDocument pdfDocument = PdfDocument.Load(longPathFile, null, item.PassWord))
					{
						int num = 0;
						foreach (PdfPageRange pdfPageRange in pageRanges)
						{
							foreach (int num2 in pdfPageRange)
							{
								using (Bitmap bitmap = PdfPageImageExtractor.ExtractPageImage(pdfDocument, num2, pdfPageImageExtractSettings))
								{
									string vaildSubFile = this.GetVaildSubFile(item, args.FileExtension, num);
									if (!string.IsNullOrEmpty(vaildSubFile))
									{
										using (FileStream fileStream = File.Open(vaildSubFile, FileMode.OpenOrCreate, FileAccess.Write))
										{
											bitmap.Save(fileStream, args.OutputFormat);
										}
										num++;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidPasswordException)
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByWrongPassword;
				}
				else if (ex is FileNotFoundException)
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByFileNotExist + " " + item.convertFile;
				}
				else
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileError;
				}
				Log.WriteLog(item.convertFile + ":ExtractSinglePages Error:" + ex.Message);
				return false;
			}
			return true;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00009AAC File Offset: 0x00007CAC
		private bool ExtractPagesToSingleImage(PDFToImageItem item, global::System.Collections.Generic.IReadOnlyList<PdfPageRange> pageRanges, PDFToImageItemArgs args)
		{
			string tempFileName = Path.GetTempFileName();
			try
			{
				if (File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}
				PdfPageImageExtractSingleImageSettings pdfPageImageExtractSingleImageSettings = new PdfPageImageExtractSingleImageSettings
				{
					ImageDpi = args.DPI,
					ColorMode = args.ColorMode,
					RenderAnnotations = args.RenderAnnotations,
					TempFileFullName = tempFileName,
					ExtractIntoSingleImageOrientation = PdfPageImageExtractOrientation.Vertical,
					BorderThickness = args.BorderThickness,
					BorderColor = args.BorderColor
				};
				LongPathFile longPathFile = item.convertFile;
				if (longPathFile.IsExists)
				{
					using (PdfDocument pdfDocument = PdfDocument.Load(longPathFile, null, item.PassWord))
					{
						global::System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.IReadOnlyList<PdfPageRange>> readOnlyList = PdfPageImageExtractor.SplitPageRangesByImageSize(pdfDocument, pageRanges, pdfPageImageExtractSingleImageSettings, null, null);
						for (int i = 0; i < readOnlyList.Count; i++)
						{
							using (PdfPageImageExtractor.ExtractResult extractResult = PdfPageImageExtractor.ExtractPagesIntoSingleImage(pdfDocument, readOnlyList[i], pdfPageImageExtractSingleImageSettings, null, null))
							{
								if (extractResult != null)
								{
									string vaildSubFile = this.GetVaildSubFile(item, args.FileExtension, i);
									if (!string.IsNullOrEmpty(vaildSubFile))
									{
										extractResult.Bitmap.Save(vaildSubFile, args.OutputFormat);
									}
								}
							}
							if (File.Exists(tempFileName))
							{
								File.Delete(tempFileName);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidPasswordException)
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByWrongPassword;
				}
				else if (ex is FileNotFoundException)
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileErrorByFileNotExist + " " + item.convertFile;
				}
				else
				{
					item.FailedReason = pdfconverter.Properties.Resources.PDFToImageConvertFaileError;
				}
				return false;
			}
			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}
			return true;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00009C88 File Offset: 0x00007E88
		private void ActionAfterConvert(PDFToImageItem item, bool? viewFile)
		{
			if (item != null)
			{
				item.FileSelected = new bool?(false);
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00009C9C File Offset: 0x00007E9C
		private string GetVaildSubFile(PDFToImageItem item, string fileExtension, int num)
		{
			if (string.IsNullOrWhiteSpace(item.FileNameWithoutExtenstion) || string.IsNullOrWhiteSpace(item.outputFile) || string.IsNullOrWhiteSpace(fileExtension))
			{
				return null;
			}
			string text = string.Concat(new string[]
			{
				item.outputFile,
				"\\",
				item.FileNameWithoutExtenstion,
				" conv",
				string.Format(" {0}", num),
				fileExtension
			});
			if (!File.Exists(text) && !Directory.Exists(text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00009D28 File Offset: 0x00007F28
		private string GetValidOutputDir(string dirName, string destDir)
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

		// Token: 0x0600025C RID: 604 RVA: 0x00009DAC File Offset: 0x00007FAC
		private void NB_BorderThickness_LostFocus(object sender, RoutedEventArgs e)
		{
			NumberBox numberBox = sender as NumberBox;
			if (numberBox != null && !this.ValidateNumberBox(numberBox))
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.PDFToImageConverterMsgErrorBorderThickness, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00009DE0 File Offset: 0x00007FE0
		private void NB_Zoom_LostFocus(object sender, RoutedEventArgs e)
		{
			NumberBox numberBox = sender as NumberBox;
			if (numberBox != null && !this.ValidateNumberBox(numberBox))
			{
				ModernMessageBox.Show(pdfconverter.Properties.Resources.PDFToImageConverterMsgErrorZoom, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00009E14 File Offset: 0x00008014
		private bool ValidateNumberBox(NumberBox numberBox)
		{
			int num;
			if (!int.TryParse(numberBox.Text, out num) || (double)num < numberBox.Minimum || (double)num > numberBox.Maximum)
			{
				numberBox.Text = numberBox.Value.ToString();
				return false;
			}
			return true;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00009E5C File Offset: 0x0000805C
		private void Btn_DeleteFile_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PDFToImageWindow", "DeleteFileBtn", "Count", 1L);
			PDFToImageItem pdftoImageItem = (PDFToImageItem)((Button)sender).DataContext;
			if (pdftoImageItem != null)
			{
				try
				{
					this.ConvertFilesList.Remove(pdftoImageItem);
					this.UpdateCB_FileItemListStatus();
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00009EBC File Offset: 0x000080BC
		private void Btn_OpenFileInExplore_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PDFToImageWindow", "OpenFileInExploreBtn", "Count", 1L);
			PDFToImageItem pdftoImageItem = (PDFToImageItem)((Button)sender).DataContext;
			if (pdftoImageItem != null && !string.IsNullOrWhiteSpace(pdftoImageItem.outputFile) && pdftoImageItem.ConvertStatus == FileCovertStatus.ConvertSucc)
			{
				UtilsManager.OpenFolderInExplore(pdftoImageItem.outputFile);
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00009F14 File Offset: 0x00008114
		private void CB_FileItemList_Loaded(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			this.CB_FileItemList = checkBox;
			this.UpdateCB_FileItemListStatus();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00009F38 File Offset: 0x00008138
		private void CB_FileItemList_Click(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			bool? isChecked = checkBox.IsChecked;
			if (isChecked == null)
			{
				checkBox.IsChecked = new bool?(false);
				isChecked = new bool?(false);
			}
			foreach (PDFToImageItem pdftoImageItem in this.ConvertFilesList)
			{
				if (pdftoImageItem.ConvertStatus != FileCovertStatus.ConvertUnsupport && pdftoImageItem.ConvertStatus != FileCovertStatus.ConvertLoadedFailed)
				{
					if (isChecked.GetValueOrDefault())
					{
						bool? fileSelected = pdftoImageItem.FileSelected;
						bool flag = false;
						if ((fileSelected.GetValueOrDefault() == flag) & (fileSelected != null))
						{
							pdftoImageItem.FileSelected = new bool?(true);
						}
					}
					else if (pdftoImageItem.FileSelected.GetValueOrDefault())
					{
						pdftoImageItem.FileSelected = new bool?(false);
					}
				}
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000A010 File Offset: 0x00008210
		private void CB_FileItem_Checked(object sender, RoutedEventArgs e)
		{
			this.UpdateCB_FileItemListStatus();
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000A018 File Offset: 0x00008218
		private void CB_FileItem_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UpdateCB_FileItemListStatus();
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000A020 File Offset: 0x00008220
		private void UpdateCB_FileItemListStatus()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool? flag = new bool?(false);
			foreach (PDFToImageItem pdftoImageItem in this.ConvertFilesList)
			{
				if (pdftoImageItem.ConvertStatus != FileCovertStatus.ConvertUnsupport)
				{
					num3++;
					if (pdftoImageItem.FileSelected.GetValueOrDefault())
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
			if (this.CB_FileItemList != null)
			{
				this.CB_FileItemList.IsChecked = flag;
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000A0EC File Offset: 0x000082EC
		private PDFToImageItemArgs GetArgs()
		{
			PDFToImageItemArgs pdftoImageItemArgs = new PDFToImageItemArgs();
			ComboBoxItem comboBoxItem = this.CB_OutputFormat.SelectedItem as ComboBoxItem;
			if (comboBoxItem != null && comboBoxItem.Content.ToString().ToUpper() == "PNG")
			{
				pdftoImageItemArgs.OutputFormat = ImageFormat.Png;
			}
			else
			{
				pdftoImageItemArgs.OutputFormat = ImageFormat.Jpeg;
			}
			pdftoImageItemArgs.IsEntire = this.Rd_EntireImage.IsChecked.Value;
			if (!pdftoImageItemArgs.IsEntire)
			{
				int num = ((this.CB_Quality.SelectedIndex >= 0) ? this.CB_Quality.SelectedIndex : 0);
				pdftoImageItemArgs.DPI = (float)this.DPIs[num];
			}
			else
			{
				pdftoImageItemArgs.DPI = (float)(72.0 * this.NB_Zoom.Value / 100.0);
				pdftoImageItemArgs.BorderColor = Color.FromArgb((int)this.Btn_ColorPicker.SelectedColor.A, (int)this.Btn_ColorPicker.SelectedColor.R, (int)this.Btn_ColorPicker.SelectedColor.G, (int)this.Btn_ColorPicker.SelectedColor.B);
				pdftoImageItemArgs.BorderThickness = (int)this.NB_BorderThickness.Value;
			}
			if (this.Rd_ColorMode_RGB.IsChecked.Value)
			{
				pdftoImageItemArgs.ColorMode = PdfPageImageExtractColorMode.RGB;
			}
			else
			{
				pdftoImageItemArgs.ColorMode = PdfPageImageExtractColorMode.Gray;
			}
			pdftoImageItemArgs.RenderAnnotations = this.CB_RenderAnnotation.IsChecked.Value;
			pdftoImageItemArgs.OutputPath = this.TB_OutputPath.Text;
			return pdftoImageItemArgs;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000A280 File Offset: 0x00008480
		private void SelectItemsInexplorer(List<PDFToImageItem> selectedItems)
		{
			try
			{
				string[] array = (from t in selectedItems
					where t.ConvertStatus == FileCovertStatus.ConvertSucc && !string.IsNullOrEmpty(t.outputFile)
					select t.outputFile).ToArray<string>();
				ExplorerUtils.OpenFolderAsync(Path.GetDirectoryName(array[0]), array, default(CancellationToken));
			}
			catch (Exception ex)
			{
				Log.WriteLog("SelectItemsInexplorer:" + ex.Message);
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000A320 File Offset: 0x00008520
		private void NB_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (this.PageRangeKeys.Contains(e.Key))
			{
				base.OnPreviewKeyDown(e);
				return;
			}
			e.Handled = true;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000A344 File Offset: 0x00008544
		private void CB_Quality_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageDPISelectedIndex(this.CB_Quality.SelectedIndex);
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000A360 File Offset: 0x00008560
		private void Rd_OutputOption_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is RadioButton && this.Rd_EntireImage != null && this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageIsEntire(this.Rd_EntireImage.IsChecked.Value);
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000A3A2 File Offset: 0x000085A2
		private void Btn_ColorPicker_SelectedColorChanged(object sender, ColorPickerButtonSelectedColorChangedEventArgs e)
		{
			if (this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageBorderColor(this.Btn_ColorPicker.SelectedColor);
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000A3BC File Offset: 0x000085BC
		private void Rd_ColorMode_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is RadioButton && this.Rd_ColorMode_RGB != null && this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageColorModeSelectedIndex((!this.Rd_ColorMode_RGB.IsChecked.GetValueOrDefault()) ? 1 : 0);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000A404 File Offset: 0x00008604
		private void CB_RenderAnnotation_Checked(object sender, RoutedEventArgs e)
		{
			if (this.IsFinishInitializePDFConvert)
			{
				CheckBox cb_RenderAnnotation = this.CB_RenderAnnotation;
				ConfigManager.SetIsRenderAnnotations(((cb_RenderAnnotation != null) ? cb_RenderAnnotation.IsChecked : null).Value);
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000A440 File Offset: 0x00008640
		private void NB_Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.IsFinishInitializeComponent && this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageZoom(this.NB_Zoom.Value);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000A462 File Offset: 0x00008662
		private void NB_BorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.IsFinishInitializeComponent && this.IsFinishInitializePDFConvert)
			{
				ConfigManager.SetPDFToImageBorderThickness(this.NB_BorderThickness.Value);
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000A7C4 File Offset: 0x000089C4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 4:
				((CheckBox)target).Loaded += this.CB_FileItemList_Loaded;
				((CheckBox)target).Click += this.CB_FileItemList_Click;
				return;
			case 5:
				((Button)target).Click += this.Btn_ClearFiles_Click;
				return;
			case 6:
				((CheckBox)target).Checked += this.CB_FileItem_Checked;
				((CheckBox)target).Unchecked += this.CB_FileItem_Unchecked;
				return;
			case 7:
				((Button)target).Click += this.Btn_OpenFileInExplore_Click;
				return;
			case 8:
				((Button)target).Click += this.Btn_DeleteFile_Click;
				return;
			default:
				return;
			}
		}

		// Token: 0x04000160 RID: 352
		public List<int> DPIs = new List<int> { 72, 72, 96, 120, 150, 300, 450, 600 };

		// Token: 0x04000161 RID: 353
		private ObservableCollection<PDFToImageItem> ConvertFilesList = new ObservableCollection<PDFToImageItem>();

		// Token: 0x04000162 RID: 354
		private HashSet<Key> PageRangeKeys = new HashSet<Key>
		{
			Key.Space,
			Key.Return,
			Key.Escape,
			Key.Left,
			Key.Up,
			Key.Right,
			Key.Down,
			Key.Tab,
			Key.Subtract,
			Key.OemMinus,
			Key.OemComma,
			Key.Back,
			Key.Delete,
			Key.D0,
			Key.D1,
			Key.D2,
			Key.D3,
			Key.D4,
			Key.D5,
			Key.D6,
			Key.D7,
			Key.D8,
			Key.D9,
			Key.NumPad0,
			Key.NumPad1,
			Key.NumPad2,
			Key.NumPad3,
			Key.NumPad4,
			Key.NumPad5,
			Key.NumPad6,
			Key.NumPad7,
			Key.NumPad8,
			Key.NumPad9
		};

		// Token: 0x04000163 RID: 355
		private bool IsFinishInitializeComponent;

		// Token: 0x04000164 RID: 356
		private bool IsFinishInitializePDFConvert;

		// Token: 0x04000165 RID: 357
		private CheckBox CB_FileItemList;
	}
}
