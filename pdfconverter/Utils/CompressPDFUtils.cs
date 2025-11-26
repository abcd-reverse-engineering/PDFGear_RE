using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Exporting;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace pdfconverter.Utils
{
	// Token: 0x0200003B RID: 59
	public static class CompressPDFUtils
	{
		// Token: 0x060004C3 RID: 1219 RVA: 0x00013128 File Offset: 0x00011328
		public static async Task<bool> ComputePDFSize(string inputFile, int compressMode, string outputfileName, string password, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					cancellationToken.ThrowIfCancellationRequested();
					IProgress<double> progress2 = progress;
					if (progress2 != null)
					{
						progress2.Report(0.0);
					}
					double imageOffset = CompressPDFUtils.GetImageOffset(compressMode);
					PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(inputFile, password);
					pdfLoadedDocument.FileStructure.IncrementalUpdate = false;
					if (compressMode == 0)
					{
						pdfLoadedDocument.Compression = PdfCompressionLevel.Best;
					}
					else if (compressMode == 1)
					{
						pdfLoadedDocument.Compression = PdfCompressionLevel.Normal;
					}
					else if (compressMode == 2)
					{
						pdfLoadedDocument.Compression = PdfCompressionLevel.BelowNormal;
					}
					pdfLoadedDocument.CompressionOptions = new PdfCompressionOptions
					{
						OptimizePageContents = true,
						RemoveMetadata = true,
						OptimizeFont = true,
						CompressImages = true
					};
					for (int i = 0; i < pdfLoadedDocument.Pages.Count; i++)
					{
						PdfPageBase pdfPageBase = pdfLoadedDocument.Pages[i];
						PdfImageInfo[] imagesInfo = pdfPageBase.ImagesInfo;
						cancellationToken.ThrowIfCancellationRequested();
						for (int j = 0; j < imagesInfo.Length; j++)
						{
							cancellationToken.ThrowIfCancellationRequested();
							PdfImageInfo pdfImageInfo = imagesInfo[j];
							if (pdfImageInfo.Bounds.Width >= 0f && pdfImageInfo.Bounds.Height >= 0f)
							{
								double num = (double)pdfImageInfo.Bounds.Width * 2.54 / 72.0;
								double num2 = (double)pdfImageInfo.Bounds.Height * 2.54 / 72.0;
								double num3 = 0.0;
								double num4 = 0.0;
								try
								{
									num3 = (double)pdfImageInfo.Image.Width * 2.54 / num;
									num4 = (double)pdfImageInfo.Image.Height * 2.54 / num2;
								}
								catch
								{
								}
								if (num3 > 0.0 && num4 > 0.0)
								{
									PdfBitmap pdfBitmap = null;
									Image image = null;
									try
									{
										image = CompressPDFUtils.ResizeImage(imagesInfo[j].Image, pdfImageInfo.Bounds, imageOffset);
										if (image != null)
										{
											pdfBitmap = new PdfBitmap(image);
											pdfBitmap.Quality = 50L;
											pdfPageBase.ReplaceImage(j, pdfBitmap);
											pdfBitmap.Dispose();
											pdfBitmap = null;
											image.Dispose();
											image = null;
										}
									}
									finally
									{
										try
										{
											PdfImageInfo pdfImageInfo2 = imagesInfo[j];
											if (pdfImageInfo2 != null)
											{
												Image image2 = pdfImageInfo2.Image;
												if (image2 != null)
												{
													image2.Dispose();
												}
											}
										}
										catch
										{
										}
										try
										{
											if (pdfBitmap != null)
											{
												pdfBitmap.Dispose();
											}
										}
										catch
										{
										}
										try
										{
											if (image != null)
											{
												image.Dispose();
											}
										}
										catch
										{
										}
										pdfImageInfo = null;
									}
								}
								IProgress<double> progress3 = progress;
								if (progress3 != null)
								{
									progress3.Report(0.5 * ((double)i + ((double)j + 1.0) / (double)imagesInfo.Length) / (double)pdfLoadedDocument.Pages.Count);
								}
							}
						}
						IProgress<double> progress4 = progress;
						if (progress4 != null)
						{
							progress4.Report(0.5 * ((double)i + 1.0) / (double)pdfLoadedDocument.Pages.Count);
						}
					}
					try
					{
						double curProgress = 0.5;
						pdfLoadedDocument.SaveProgress += delegate(object s, ProgressEventArgs a)
						{
							cancellationToken.ThrowIfCancellationRequested();
							double num5 = 0.5 + (double)a.Progress * 0.5;
							if (num5 > curProgress)
							{
								curProgress = num5;
								IProgress<double> progress5 = progress;
								if (progress5 == null)
								{
									return;
								}
								progress5.Report(curProgress);
							}
						};
						pdfLoadedDocument.Save(outputfileName);
						pdfLoadedDocument.Close(true);
					}
					catch (Exception ex2) when (!(ex2 is OperationCanceledException))
					{
					}
				}), cancellationToken);
			}
			catch (Exception ex) when (!(ex is OperationCanceledException))
			{
				return false;
			}
			return true;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00013195 File Offset: 0x00011395
		private static double GetImageOffset(int mode)
		{
			if (mode == 0)
			{
				return 75.0;
			}
			if (mode == 1)
			{
				return 110.0;
			}
			return 220.0;
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x000131BC File Offset: 0x000113BC
		private static Image ResizeImage(Image imgToResize, RectangleF bounds, double maxPPI)
		{
			int width = imgToResize.Width;
			int height = imgToResize.Height;
			double num = (double)bounds.Width * 2.54 / 72.0;
			double num2 = (double)bounds.Height * 2.54 / 72.0;
			double num3 = (double)imgToResize.Width * 2.54 / num;
			double num4 = (double)imgToResize.Height * 2.54 / num2;
			if (num3 - maxPPI < 2.0 && num4 - maxPPI < 2.0)
			{
				return null;
			}
			int num5 = (int)(num * maxPPI / 2.54);
			int num6 = (int)(num2 * maxPPI / 2.54);
			if (num5 <= 0 || num6 <= 0)
			{
				return null;
			}
			Bitmap bitmap = new Bitmap(num5, num6);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.DrawImage(imgToResize, 0, 0, num5, num6);
			graphics.Dispose();
			return bitmap;
		}
	}
}
