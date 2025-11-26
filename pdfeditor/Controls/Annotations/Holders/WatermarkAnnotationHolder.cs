using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;
using PDFKit.Utils.WatermarkUtils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002C0 RID: 704
	public class WatermarkAnnotationHolder : BaseAnnotationHolder<PdfWatermarkAnnotation>
	{
		// Token: 0x06002877 RID: 10359 RVA: 0x000BE5CC File Offset: 0x000BC7CC
		public WatermarkAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x06002878 RID: 10360 RVA: 0x000BE5D5 File Offset: 0x000BC7D5
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002879 RID: 10361 RVA: 0x000BE5D8 File Offset: 0x000BC7D8
		public override void OnPageClientBoundsChanged()
		{
		}

		// Token: 0x0600287A RID: 10362 RVA: 0x000BE5DA File Offset: 0x000BC7DA
		public override bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x0600287B RID: 10363 RVA: 0x000BE5DD File Offset: 0x000BC7DD
		protected override void OnCancel()
		{
			this.startPoint = default(Point);
		}

		// Token: 0x0600287C RID: 10364 RVA: 0x000BE5EC File Offset: 0x000BC7EC
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfWatermarkAnnotation>> OnCompleteCreateNewAsync()
		{
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			this.watermarkParam = vm.AnnotationToolbar.WatermarkParam;
			global::System.Collections.Generic.IReadOnlyList<PdfWatermarkAnnotation> readOnlyList;
			if (this.watermarkParam.PageRange == null || this.watermarkParam.PageRange.Length == 0)
			{
				readOnlyList = null;
			}
			else
			{
				for (int p = 0; p < this.watermarkParam.PageRange.Length; p++)
				{
					PdfPage pdfPage = vm.Document.Pages[p];
					if (pdfPage.Annots == null)
					{
						pdfPage.CreateAnnotations();
					}
					float num = 72f;
					if (pdfPage.Dictionary.ContainsKey("UserUnit"))
					{
						num = pdfPage.Dictionary["UserUnit"].As<PdfTypeNumber>(true).FloatValue / 72f;
					}
					float num2 = 96f;
					float num3 = 96f;
					int num4 = (int)(pdfPage.Width / num * num2);
					int num5 = (int)(pdfPage.Height / num * num3);
					WatermarkAnnonationModel watermarkModel = vm.AnnotationToolbar.WatermarkModel;
					if (watermarkModel != WatermarkAnnonationModel.Text)
					{
						if (watermarkModel == WatermarkAnnonationModel.Image)
						{
							WatermarkImageModel imageWatermarkModel = vm.AnnotationToolbar.ImageWatermarkModel;
							WriteableBitmap writeableBitmap = null;
							if (((imageWatermarkModel != null) ? imageWatermarkModel.WatermarkImageSource : null) != null)
							{
								if (imageWatermarkModel.WatermarkImageSource.Format == PixelFormats.Bgra32)
								{
									writeableBitmap = new WriteableBitmap(imageWatermarkModel.WatermarkImageSource);
								}
								else
								{
									writeableBitmap = new WriteableBitmap(new FormatConvertedBitmap(imageWatermarkModel.WatermarkImageSource, PixelFormats.Bgra32, null, 0.0));
								}
							}
							using (PdfBitmap pdfBitmap = new PdfBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, true, false))
							{
								int num6 = pdfBitmap.Stride * pdfBitmap.Height;
								writeableBitmap.CopyPixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), pdfBitmap.Buffer, num6, pdfBitmap.Stride);
								PdfImageObject pdfImageObject = PdfImageObject.Create(pdfPage.Document, pdfBitmap, 0f, 0f);
								float num7 = (((float)writeableBitmap.PixelWidth <= pdfPage.Width / 2f && (float)writeableBitmap.PixelHeight <= pdfPage.Height / 2f) ? 0.5f : 0.1f);
								if (this.watermarkParam.IsTile)
								{
									float num8 = (float)pdfBitmap.Width * num7;
									float num9 = (float)pdfBitmap.Height * num7;
									int num10 = Convert.ToInt32((double)num4 / ((double)num8 + 50.0));
									int num11 = Convert.ToInt32((double)num5 / ((double)num9 + 50.0));
									for (int i = 0; i < num11; i++)
									{
										for (int j = 0; j < num10; j++)
										{
											float num12 = (float)((double)j * ((double)num8 + 50.0));
											float num13 = (float)((double)pdfPage.Height - (double)i * ((double)num9 + 50.0));
											PdfImageObject pdfImageObject2 = PdfImageObject.Create(pdfPage.Document, pdfBitmap, num12, num13);
											this.SetImageObjectSoftMask(pdfImageObject2, this.watermarkParam.Opacity);
											this.SetImageRotate(pdfImageObject2, this.watermarkParam.Rotation, num7, true, num12, num13);
											PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(pdfPage, pdfImageObject2, PdfContentAlignment.MiddleCenter, 0f, 0f, 0f, 1f, true, true);
											pdfWatermarkAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
											pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
											pdfPage.Annots.Add(pdfWatermarkAnnotation);
										}
									}
								}
								else
								{
									this.SetImageObjectSoftMask(pdfImageObject, this.watermarkParam.Opacity);
									FS_POINTF fs_POINTF = this.SeImgObjectLocation(pdfImageObject, this.watermarkParam.Alignment, (double)pdfPage.Width, (double)pdfPage.Height, num7);
									this.SetImageRotate(pdfImageObject, this.watermarkParam.Rotation, num7, false, fs_POINTF.X, fs_POINTF.Y);
									PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(pdfPage, pdfImageObject, PdfContentAlignment.MiddleCenter, 0f, 0f, 0f, 1f, true, true);
									pdfWatermarkAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
									pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
									pdfPage.Annots.Add(pdfWatermarkAnnotation);
								}
								await pdfPage.TryRedrawPageAsync(default(CancellationToken));
							}
							PdfBitmap pdfBitmap = null;
						}
					}
					else
					{
						WatermarkTextModel textWatermarkModel = vm.AnnotationToolbar.TextWatermarkModel;
						PdfFont pdfFont = PdfFont.CreateStock(pdfPage.Document, FontStockNames.Arial);
						Color color = (Color)ColorConverter.ConvertFromString(textWatermarkModel.Foreground);
						FS_COLOR fs_COLOR = new FS_COLOR((int)((byte)((float)color.A * this.watermarkParam.Opacity)), (int)color.R, (int)color.G, (int)color.B);
						PdfTextObject pdfTextObject = PdfTextObject.Create(textWatermarkModel.Content, 0f, 0f, pdfFont, textWatermarkModel.FontSize);
						pdfTextObject.FillColor = fs_COLOR;
						float width = pdfTextObject.BoundingBox.Width;
						float height = pdfTextObject.BoundingBox.Height;
						if (this.watermarkParam.IsTile)
						{
							int num14 = Convert.ToInt32((double)num4 / ((double)width + 50.0));
							int num15 = Convert.ToInt32((double)num5 / ((double)height + 50.0));
							for (int k = 0; k < num15; k++)
							{
								for (int l = 0; l < num14; l++)
								{
									List<PdfTextObject> list = WatermarkUtil.CreateWatermarkTextObjects(pdfPage.Document, textWatermarkModel.Content, fs_COLOR, pdfFont, textWatermarkModel.FontSize);
									for (int m = 0; m < list.Count; m++)
									{
										PdfTextObject pdfTextObject2 = list[m];
										FS_MATRIX fs_MATRIX = pdfTextObject2.Matrix;
										if (fs_MATRIX == null)
										{
											fs_MATRIX = new FS_MATRIX();
										}
										fs_MATRIX.SetIdentity();
										pdfTextObject2.FillColor = fs_COLOR;
										fs_MATRIX.Rotate(this.watermarkParam.Rotation * 3.14f / 180f, false);
										pdfTextObject2.Matrix = fs_MATRIX;
										double num16 = (double)((float)l) * ((double)width + 50.0);
										float num17 = (float)((double)pdfPage.Height - (double)k * 50.0);
										pdfTextObject2.Location = new FS_POINTF(num16, (double)num17);
										PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(pdfPage, pdfTextObject2, PdfContentAlignment.MiddleCenter, 0f, 0f, 0f, 1f, true, true);
										pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
										pdfPage.Annots.Add(pdfWatermarkAnnotation);
									}
								}
							}
						}
						else
						{
							List<PdfTextObject> list2 = WatermarkUtil.CreateWatermarkTextObjects(pdfPage.Document, textWatermarkModel.Content, fs_COLOR, pdfFont, textWatermarkModel.FontSize);
							for (int n = 0; n < list2.Count; n++)
							{
								pdfTextObject = list2[n];
								FS_MATRIX fs_MATRIX2 = pdfTextObject.Matrix;
								if (fs_MATRIX2 == null)
								{
									fs_MATRIX2 = new FS_MATRIX();
								}
								fs_MATRIX2.SetIdentity();
								fs_MATRIX2.Rotate(this.watermarkParam.Rotation * 3.14f / 180f, false);
								pdfTextObject.Matrix = fs_MATRIX2;
								this.SeTextObjectLocation(pdfTextObject, this.watermarkParam.Alignment, (double)pdfPage.Width, (double)pdfPage.Height);
								PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(pdfPage, pdfTextObject, this.watermarkParam.Alignment, 0f, 0f, 0f, 1f, true, true);
								pdfWatermarkAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
								pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
								pdfPage.Annots.Add(pdfWatermarkAnnotation);
							}
						}
						await pdfPage.TryRedrawPageAsync(default(CancellationToken));
					}
				}
				readOnlyList = null;
			}
			return readOnlyList;
		}

		// Token: 0x0600287D RID: 10365 RVA: 0x000BE630 File Offset: 0x000BC830
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (pagePoint.X >= 0f && pagePoint.X <= page.Width && pagePoint.Y >= 0f && pagePoint.Y <= page.Height)
			{
				this.startPoint = pagePoint.ToPoint();
			}
		}

		// Token: 0x0600287E RID: 10366 RVA: 0x000BE683 File Offset: 0x000BC883
		protected override bool OnSelecting(PdfWatermarkAnnotation annotation, bool afterCreate)
		{
			return false;
		}

		// Token: 0x0600287F RID: 10367 RVA: 0x000BE688 File Offset: 0x000BC888
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.startPoint = point;
				return true;
			}
			return false;
		}

		// Token: 0x06002880 RID: 10368 RVA: 0x000BE6C0 File Offset: 0x000BC8C0
		private void SeTextObjectLocation(PdfTextObject text, PdfContentAlignment alignment, double pageWidth, double pageHeight)
		{
			float width = text.BoundingBox.Width;
			float height = text.BoundingBox.Height;
			if (alignment <= PdfContentAlignment.MiddleCenter)
			{
				switch (alignment)
				{
				case PdfContentAlignment.TopLeft:
					text.Location = new FS_POINTF(5.0, pageHeight - (double)height);
					return;
				case PdfContentAlignment.TopCenter:
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), pageHeight - (double)height);
					return;
				case (PdfContentAlignment)3:
					break;
				case PdfContentAlignment.TopRight:
					text.Location = new FS_POINTF(pageWidth - (double)width - 5.0, pageHeight - (double)height);
					break;
				default:
					if (alignment == PdfContentAlignment.MiddleLeft)
					{
						text.Location = new FS_POINTF(5.0, pageHeight / 2.0 - (double)(height / 2f));
						return;
					}
					if (alignment != PdfContentAlignment.MiddleCenter)
					{
						return;
					}
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), pageHeight / 2.0 - (double)(height / 2f));
					return;
				}
				return;
			}
			if (alignment <= PdfContentAlignment.BottomLeft)
			{
				if (alignment == PdfContentAlignment.MiddleRight)
				{
					text.Location = new FS_POINTF(pageWidth - (double)width, pageHeight / 2.0 - (double)(height / 2f));
					return;
				}
				if (alignment != PdfContentAlignment.BottomLeft)
				{
					return;
				}
				text.Location = new FS_POINTF(5f, 0f);
				return;
			}
			else
			{
				if (alignment == PdfContentAlignment.BottomCenter)
				{
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), 0.0);
					return;
				}
				if (alignment != PdfContentAlignment.BottomRight)
				{
					return;
				}
				text.Location = new FS_POINTF(pageWidth - (double)width, 0.0);
				return;
			}
		}

		// Token: 0x06002881 RID: 10369 RVA: 0x000BE87C File Offset: 0x000BCA7C
		private FS_POINTF SeImgObjectLocation(PdfImageObject img, PdfContentAlignment alignment, double pageWidth, double pageHeight, float scale)
		{
			float num = img.BoundingBox.Width * scale;
			float num2 = img.BoundingBox.Height * scale;
			FS_POINTF fs_POINTF = default(FS_POINTF);
			if (alignment <= PdfContentAlignment.MiddleCenter)
			{
				switch (alignment)
				{
				case PdfContentAlignment.TopLeft:
					fs_POINTF = new FS_POINTF(5.0, pageHeight - (double)num2);
					break;
				case PdfContentAlignment.TopCenter:
					fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), pageHeight - (double)num2);
					break;
				case (PdfContentAlignment)3:
					break;
				case PdfContentAlignment.TopRight:
					fs_POINTF = new FS_POINTF(pageWidth - (double)num - 5.0, pageHeight - (double)num2);
					break;
				default:
					if (alignment != PdfContentAlignment.MiddleLeft)
					{
						if (alignment == PdfContentAlignment.MiddleCenter)
						{
							fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), pageHeight / 2.0 - (double)(num2 / 2f));
						}
					}
					else
					{
						fs_POINTF = new FS_POINTF(5.0, pageHeight / 2.0 - (double)(num2 / 2f));
					}
					break;
				}
			}
			else if (alignment <= PdfContentAlignment.BottomLeft)
			{
				if (alignment != PdfContentAlignment.MiddleRight)
				{
					if (alignment == PdfContentAlignment.BottomLeft)
					{
						fs_POINTF = new FS_POINTF(5f, num / 2f);
					}
				}
				else
				{
					fs_POINTF = new FS_POINTF(pageWidth - (double)num, pageHeight / 2.0 - (double)(num2 / 2f));
				}
			}
			else if (alignment != PdfContentAlignment.BottomCenter)
			{
				if (alignment == PdfContentAlignment.BottomRight)
				{
					fs_POINTF = new FS_POINTF(pageWidth - (double)num, 0.0);
				}
			}
			else
			{
				fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), 0.0);
			}
			return fs_POINTF;
		}

		// Token: 0x06002882 RID: 10370 RVA: 0x000BEA48 File Offset: 0x000BCC48
		private void SetImageRotate(PdfImageObject img, float rotation, float scale, bool isTile, float x, float y)
		{
			FS_MATRIX fs_MATRIX = img.Matrix;
			if (fs_MATRIX == null)
			{
				fs_MATRIX = new FS_MATRIX();
			}
			float num = (float)img.Bitmap.Width * scale;
			float num2 = (float)img.Bitmap.Height * scale;
			float num3 = rotation * 3.14f / 180f;
			fs_MATRIX.SetIdentity();
			fs_MATRIX.Scale(num, num2, false);
			fs_MATRIX.Translate(-num / 2f, -num2 / 2f, false);
			fs_MATRIX.Rotate(num3, false);
			fs_MATRIX.Translate(num / 2f, num2 / 2f, false);
			fs_MATRIX.Translate(x, y, false);
			img.Matrix = fs_MATRIX;
		}

		// Token: 0x06002883 RID: 10371 RVA: 0x000BEAE8 File Offset: 0x000BCCE8
		private void SetImageObjectSoftMask(PdfImageObject img, float Opacity)
		{
			PdfTypeStream pdfTypeStream = null;
			PdfTypeBase pdfTypeBase;
			if (img.SoftMask != null && img.SoftMask.Is<PdfTypeStream>())
			{
				pdfTypeStream = img.SoftMask.As<PdfTypeStream>(true);
			}
			else if (img.Stream.Dictionary != null && img.Stream.Dictionary.TryGetValue("SMask", out pdfTypeBase) && pdfTypeBase != null && pdfTypeBase.Is<PdfTypeStream>())
			{
				pdfTypeStream = pdfTypeBase.As<PdfTypeStream>(true);
			}
			byte[] array = new byte[pdfTypeStream.Content.Length];
			for (int i = 0; i < pdfTypeStream.Content.Length; i++)
			{
				array[i] = (byte)((float)pdfTypeStream.Content[i] * Opacity);
			}
			pdfTypeStream.SetContent(array, false);
			img.SoftMask = pdfTypeStream;
		}

		// Token: 0x0400115E RID: 4446
		private WatermarkParam watermarkParam;

		// Token: 0x0400115F RID: 4447
		private const double HTileSpan = 50.0;

		// Token: 0x04001160 RID: 4448
		private const double VTileSpan = 50.0;

		// Token: 0x04001161 RID: 4449
		private Point startPoint;
	}
}
