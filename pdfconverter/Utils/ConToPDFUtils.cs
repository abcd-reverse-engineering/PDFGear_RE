using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.Views;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.OfficeChart;
using Syncfusion.OfficeChartToImageConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Presentation;
using Syncfusion.PresentationToPdfConverter;
using Syncfusion.XlsIO;

namespace pdfconverter.Utils
{
	// Token: 0x0200003C RID: 60
	public static class ConToPDFUtils
	{
		// Token: 0x060004C6 RID: 1222 RVA: 0x000132A8 File Offset: 0x000114A8
		public static async Task<bool> WordToPDFByRangeAsync(string inputFile, string outfullName, int start, int end, string password, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			bool flag;
			if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outfullName))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						WordDocument wordDocument = new WordDocument();
						string extension = Path.GetExtension(inputFile);
						if (!string.IsNullOrWhiteSpace(extension) && extension.Equals(".doc", StringComparison.CurrentCultureIgnoreCase))
						{
							wordDocument.Settings.SkipIncrementalSaveValidation = true;
						}
						wordDocument.Open(inputFile, Syncfusion.DocIO.FormatType.Automatic, password);
						wordDocument.ChartToImageConverter = new ChartToImageConverter();
						wordDocument.ChartToImageConverter.ScalingMode = Syncfusion.OfficeChart.ScalingMode.Normal;
						int num = end - start + 1;
						for (int i = 1; i < start; i++)
						{
							wordDocument.ChildEntities.RemoveAt(0);
						}
						for (int j = num; j < end; j++)
						{
							wordDocument.ChildEntities.RemoveAt(wordDocument.ChildEntities.Count - 1);
						}
						DocToPDFConverter docToPDFConverter = new DocToPDFConverter();
						PdfDocument pdfDocument = docToPDFConverter.ConvertToPDF(wordDocument);
						pdfDocument.Save(outfullName);
						pdfDocument.Close(true);
						pdfDocument.Dispose();
						wordDocument.Close();
						wordDocument.Dispose();
						docToPDFConverter.Dispose();
						GC.Collect();
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00013320 File Offset: 0x00011520
		public static async Task<bool> RTFToPDFByRangeAsync(string inputFile, string outfullName, int start, int end, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			bool flag;
			if (string.IsNullOrEmpty(inputFile))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						WordDocument wordDocument = new WordDocument(inputFile, Syncfusion.DocIO.FormatType.Txt);
						PdfDocument pdfDocument = new DocToPDFConverter
						{
							Settings = 
							{
								EmbedCompleteFonts = true
							}
						}.ConvertToPDF(wordDocument);
						pdfDocument.Save(outfullName);
						pdfDocument.Close(true);
						wordDocument.Close();
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00013380 File Offset: 0x00011580
		public static async Task<bool> PPTToPDFByRangeAsync(string inputFile, string outfullName, int start, int end, string password, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			bool flag;
			if (string.IsNullOrEmpty(inputFile))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						IPresentation presentation = Presentation.Open(inputFile, password);
						presentation.ChartToImageConverter = new ChartToImageConverter();
						PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation);
						pdfDocument.Save(outfullName);
						pdfDocument.Close(true);
						presentation.Close();
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x000133E8 File Offset: 0x000115E8
		public static async Task<bool> ExcelToPDFByRangeAsync(string inputFile, string outfullName, int start, int end, string password, IProgress<double> progress, CancellationToken cancellationToken, bool fitWitdh = true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			bool flag;
			if (string.IsNullOrEmpty(inputFile))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						using (ExcelEngine excelEngine = new ExcelEngine())
						{
							IApplication excel = excelEngine.Excel;
							excel.DefaultVersion = ExcelVersion.Excel2013;
							IWorkbook workbook = excel.Workbooks.Open(inputFile, ExcelParseOptions.Default, false, password, ExcelOpenType.Automatic);
							foreach (IWorksheet worksheet in workbook.Worksheets)
							{
								worksheet.PageSetup.PrintComments = ExcelPrintLocation.PrintSheetEnd;
							}
							ExcelToPdfConverter excelToPdfConverter = new ExcelToPdfConverter(workbook);
							new PdfDocument();
							ExcelToPdfConverterSettings excelToPdfConverterSettings = new ExcelToPdfConverterSettings();
							if (fitWitdh)
							{
								excelToPdfConverterSettings.LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage;
							}
							else
							{
								excelToPdfConverterSettings.LayoutOptions = LayoutOptions.NoScaling;
							}
							excelToPdfConverter.Convert(excelToPdfConverterSettings).Save(outfullName);
						}
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00013458 File Offset: 0x00011658
		public static async Task<bool> ImageToSiglePDFByRangeAsync(List<ToPDFFileItem> inputFiles, string outfullName, PageSizeItem sizeItem, PdfMargins margins, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			SizeF size = ParaConvert.GetPdfPagesize(sizeItem);
			bool MapSource = size.Width == 0f || size.Height == 0f;
			bool HadSetPageSetting = false;
			bool flag;
			if (inputFiles.Count == 0 || string.IsNullOrEmpty(outfullName))
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						PdfDocument pdfDocument = new PdfDocument();
						foreach (ToPDFFileItem toPDFFileItem in inputFiles)
						{
							using (FileStream fileStream = File.OpenRead(toPDFFileItem.FilePath))
							{
								using (PdfBitmap pdfBitmap = new PdfBitmap(fileStream))
								{
									float num = pdfBitmap.PhysicalDimension.Height;
									float num2 = pdfBitmap.PhysicalDimension.Width;
									if (MapSource)
									{
										PdfSection pdfSection = pdfDocument.Sections.Add();
										pdfSection.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle0;
										float num3 = num2 + margins.Left + margins.Right;
										float num4 = num + margins.Top + margins.Bottom;
										pdfSection.PageSettings.Size = new SizeF(num3, num4);
										pdfSection.PageSettings.Margins = margins;
										pdfSection.Pages.Add().Graphics.DrawImage(pdfBitmap, 0f, 0f, num2, num);
									}
									else
									{
										if (!HadSetPageSetting)
										{
											pdfDocument.PageSettings.Margins = margins;
											pdfDocument.PageSettings.Size = size;
											HadSetPageSetting = true;
										}
										PdfPage pdfPage = pdfDocument.Pages.Add();
										PdfGraphics graphics = pdfPage.Graphics;
										float num3 = pdfPage.GetClientSize().Width;
										float num4 = pdfPage.GetClientSize().Height;
										float num5 = Math.Min(num3 / num2, num4 / num);
										if (num5 < 1f)
										{
											num *= num5;
											num2 *= num5;
										}
										float num6 = (num3 - num2) / 2f;
										float num7 = (num4 - num) / 2f;
										graphics.DrawImage(pdfBitmap, num6, num7, num2, num);
									}
								}
							}
						}
						pdfDocument.Save(outfullName);
						pdfDocument.Close(true);
						pdfDocument = null;
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x000134C8 File Offset: 0x000116C8
		public static async Task<bool> ImageToMultiPDFByRangeAsync(ToPDFFileItem item, PageSizeItem sizeItem, PdfMargins margins, IProgress<double> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (progress != null)
			{
				progress.Report(0.0);
			}
			SizeF size = ParaConvert.GetPdfPagesize(sizeItem);
			bool MapSource = size.Width == 0f || size.Height == 0f;
			LongPathFile fileFullPath = item.FilePath;
			bool flag;
			if (!fileFullPath.IsExists)
			{
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			else
			{
				try
				{
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						PdfDocument pdfDocument = new PdfDocument();
						PdfBitmap pdfBitmap = new PdfBitmap(fileFullPath);
						float num = pdfBitmap.PhysicalDimension.Height;
						float num2 = pdfBitmap.PhysicalDimension.Width;
						if (MapSource)
						{
							PdfSection pdfSection = pdfDocument.Sections.Add();
							pdfSection.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle0;
							float num3 = num2 + margins.Left + margins.Right;
							float num4 = num + margins.Top + margins.Bottom;
							pdfSection.PageSettings.Size = new SizeF(num3, num4);
							pdfSection.PageSettings.Margins = margins;
							pdfSection.Pages.Add().Graphics.DrawImage(pdfBitmap, 0f, 0f, num2, num);
						}
						else
						{
							pdfDocument.PageSettings.Margins = margins;
							pdfDocument.PageSettings.Size = size;
							PdfPage pdfPage = pdfDocument.Pages.Add();
							PdfGraphics graphics = pdfPage.Graphics;
							float num3 = pdfPage.GetClientSize().Width;
							float num4 = pdfPage.GetClientSize().Height;
							float num5 = Math.Min(num3 / num2, num4 / num);
							if (num5 < 1f)
							{
								num *= num5;
								num2 *= num5;
							}
							float num6 = (num3 - num2) / 2f;
							float num7 = (num4 - num) / 2f;
							graphics.DrawImage(pdfBitmap, num6, num7, num2, num);
						}
						pdfBitmap.Dispose();
						pdfDocument.Save(item.OutputPath);
						pdfDocument.Close(true);
					}));
					return true;
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				if (progress != null)
				{
					progress.Report(1.0);
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0001352C File Offset: 0x0001172C
		public static bool? CheckAccess(string inputeFile)
		{
			bool? flag;
			try
			{
				FileStream fileStream = new FileStream(inputeFile, FileMode.Open, FileAccess.Read);
				fileStream.Close();
				fileStream.Dispose();
				flag = new bool?(true);
			}
			catch (Exception ex)
			{
				if (ex.HResult == -2147024864)
				{
					flag = new bool?(false);
				}
				else
				{
					flag = null;
				}
			}
			return flag;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00013594 File Offset: 0x00011794
		public static bool CheckPassword(string inputeFile, ref string password, Window window)
		{
			bool flag = false;
			string fileName = Path.GetFileName(inputeFile);
			do
			{
				try
				{
					new PdfLoadedDocument(inputeFile, password, true).Dispose();
					return true;
				}
				catch (Exception ex)
				{
					if (!(ex.Message == "Can't open an encrypted document. The password is invalid."))
					{
						ModernMessageBox.Show(Resources.WinPwdLoadFailed.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return false;
					}
					if (flag)
					{
						ModernMessageBox.Show(Resources.OpenDocByIncorrectPwdMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					flag = ConToPDFUtils.OnPasswordRequested(window, fileName, out password);
				}
			}
			while (flag);
			return false;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00013638 File Offset: 0x00011838
		public static bool CheckWordPassword(string inputeFile, out string password, Window window)
		{
			bool flag = false;
			string fileName = Path.GetFileName(inputeFile);
			password = "";
			do
			{
				try
				{
					new WordDocument(inputeFile, Syncfusion.DocIO.FormatType.Automatic, password).Dispose();
					return true;
				}
				catch (Exception ex)
				{
					if (!(ex.Message == "Specified password \"" + password + "\" is incorrect!"))
					{
						ModernMessageBox.Show(Resources.WinPwdLoadFailed.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return false;
					}
					if (flag)
					{
						ModernMessageBox.Show(Resources.OpenDocByIncorrectPwdMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					flag = ConToPDFUtils.OnPasswordRequested(window, fileName, out password);
				}
			}
			while (flag);
			return false;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x000136F0 File Offset: 0x000118F0
		public static bool CheckExcelPassword(string inputeFile, out string password, Window window)
		{
			bool flag = false;
			password = "";
			string fileName = Path.GetFileName(inputeFile);
			do
			{
				try
				{
					ExcelEngine excelEngine = new ExcelEngine();
					IApplication excel = excelEngine.Excel;
					excel.DefaultVersion = ExcelVersion.Excel2013;
					excel.Workbooks.Open(inputeFile, ExcelParseOptions.Default, false, password, ExcelOpenType.Automatic);
					excelEngine.Dispose();
					return true;
				}
				catch (Exception ex)
				{
					if (!ex.Message.StartsWith("Wrong password"))
					{
						ModernMessageBox.Show(Resources.WinPwdLoadFailed.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return false;
					}
					if (flag)
					{
						ModernMessageBox.Show(Resources.OpenDocByIncorrectPwdMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					flag = ConToPDFUtils.OnPasswordRequested(window, fileName, out password);
				}
			}
			while (flag);
			return false;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x000137B8 File Offset: 0x000119B8
		public static bool CheckPPTPassword(string inputeFile, out string password, Window window)
		{
			bool flag = false;
			password = "";
			string fileName = Path.GetFileName(inputeFile);
			do
			{
				try
				{
					Presentation.Open(inputeFile, password).Dispose();
					return true;
				}
				catch (Exception ex)
				{
					if (!(ex.Message == "Specified password \"" + password + "\" is incorrect!"))
					{
						ModernMessageBox.Show(Resources.WinPwdLoadFailed.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return false;
					}
					if (flag)
					{
						ModernMessageBox.Show(Resources.OpenDocByIncorrectPwdMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					flag = ConToPDFUtils.OnPasswordRequested(window, fileName, out password);
				}
			}
			while (flag);
			return false;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00013870 File Offset: 0x00011A70
		private static bool OnPasswordRequested(Window window, string fileName, out string password)
		{
			EnterPasswordDialog enterPasswordDialog = new EnterPasswordDialog(fileName);
			if (window != null && window.IsVisible)
			{
				enterPasswordDialog.Owner = window;
				enterPasswordDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
			else
			{
				enterPasswordDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}
			bool? flag = enterPasswordDialog.ShowDialog();
			password = enterPasswordDialog.Password;
			return flag.GetValueOrDefault();
		}
	}
}
