using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Models.Viewer;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002BB RID: 699
	public class StampAnnotationHolder : BaseAnnotationHolder<PdfStampAnnotation>
	{
		// Token: 0x0600284A RID: 10314 RVA: 0x000BD506 File Offset: 0x000BB706
		public StampAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x0600284B RID: 10315 RVA: 0x000BD50F File Offset: 0x000BB70F
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600284C RID: 10316 RVA: 0x000BD512 File Offset: 0x000BB712
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.Selected)
			{
				StampAnnotationDragControl stampAnnotationDragControl = this.dragControl;
				if (stampAnnotationDragControl != null)
				{
					stampAnnotationDragControl.OnPageClientBoundsChanged();
				}
				SignatureDragControl signatureDragControl = this.signatureDragControl;
				if (signatureDragControl == null)
				{
					return;
				}
				signatureDragControl.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x0600284D RID: 10317 RVA: 0x000BD53E File Offset: 0x000BB73E
		public override bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x0600284E RID: 10318 RVA: 0x000BD544 File Offset: 0x000BB744
		protected override void OnCancel()
		{
			IInputElement captured = Mouse.Captured;
			if (captured != null)
			{
				captured.ReleaseMouseCapture();
			}
			if (this.dragControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.dragControl);
				this.dragControl = null;
			}
			if (this.signatureDragControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.signatureDragControl);
				this.signatureDragControl = null;
			}
			this.processingPage = null;
			this.startPoint = default(Point);
		}

		// Token: 0x0600284F RID: 10319 RVA: 0x000BD5C0 File Offset: 0x000BB7C0
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation>> OnCompleteCreateNewAsync()
		{
			global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList;
			try
			{
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				FS_POINTF positionFromDocument = mainViewModel.ViewerOperationModel.PositionFromDocument;
				FS_SIZEF sizeInDocument = mainViewModel.ViewerOperationModel.SizeInDocument;
				FS_RECTF fs_RECTF = new FS_RECTF(positionFromDocument.X, positionFromDocument.Y, positionFromDocument.X + sizeInDocument.Width, positionFromDocument.Y - sizeInDocument.Height);
				if (this.processingPage.Rotation == PageRotate.Rotate90)
				{
					fs_RECTF = new FS_RECTF(positionFromDocument.X, positionFromDocument.Y, positionFromDocument.X + sizeInDocument.Height, positionFromDocument.Y + sizeInDocument.Width);
				}
				else if (this.processingPage.Rotation == PageRotate.Rotate180)
				{
					fs_RECTF = new FS_RECTF(positionFromDocument.X - sizeInDocument.Width, positionFromDocument.Y, positionFromDocument.X, positionFromDocument.Y + sizeInDocument.Height);
				}
				else if (this.processingPage.Rotation == PageRotate.Rotate270)
				{
					fs_RECTF = new FS_RECTF(positionFromDocument.X - sizeInDocument.Height, positionFromDocument.Y - sizeInDocument.Width, positionFromDocument.X, positionFromDocument.Y);
				}
				PdfPage pdfPage = this.processingPage;
				if (mainViewModel.AnnotationMode == AnnotationMode.Signature && !this.ShowIAPWindow())
				{
					readOnlyList = null;
				}
				else
				{
					if (pdfPage.Annots == null)
					{
						pdfPage.CreateAnnotations();
					}
					DataOperationModel viewerOperationModel = mainViewModel.ViewerOperationModel;
					object obj = ((viewerOperationModel != null) ? viewerOperationModel.Data : null);
					IStampTextModel stampTextModel = obj as IStampTextModel;
					if (stampTextModel == null)
					{
						StampImageModel stampImageModel = obj as StampImageModel;
						if (stampImageModel == null)
						{
							StampFormControlModel stampFormControlModel = obj as StampFormControlModel;
							if (stampFormControlModel == null)
							{
								mainViewModel.AnnotationMode = AnnotationMode.None;
								readOnlyList = null;
							}
							else
							{
								readOnlyList = await this.CreateFormControlStampAsync(stampFormControlModel, pdfPage, positionFromDocument, fs_RECTF);
							}
						}
						else
						{
							readOnlyList = await this.CreateImageStampAsync(stampImageModel, pdfPage, positionFromDocument, fs_RECTF);
						}
					}
					else
					{
						readOnlyList = await this.CreateTextStampAsync(stampTextModel, pdfPage, positionFromDocument, fs_RECTF);
					}
				}
			}
			finally
			{
				this.processingPage = null;
			}
			return readOnlyList;
		}

		// Token: 0x06002850 RID: 10320 RVA: 0x000BD604 File Offset: 0x000BB804
		private bool ShowIAPWindow()
		{
			Ioc.Default.GetRequiredService<MainViewModel>();
			bool signatureExistFlag = ConfigManager.GetSignatureExistFlag();
			if (!IAPUtils.IsPaidUserWrapper() && signatureExistFlag)
			{
				IAPUtils.ShowPurchaseWindows("Signature", ".pdf");
				return false;
			}
			return true;
		}

		// Token: 0x06002851 RID: 10321 RVA: 0x000BD640 File Offset: 0x000BB840
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (pagePoint.X >= 0f && pagePoint.X <= page.Width && pagePoint.Y >= 0f && pagePoint.Y <= page.Height)
			{
				this.processingPage = page;
				Point point;
				if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
				{
					double x = point.X;
					double y = point.Y;
					this.startPoint = pagePoint.ToPoint();
				}
			}
		}

		// Token: 0x06002852 RID: 10322 RVA: 0x000BD6CC File Offset: 0x000BB8CC
		protected override bool OnSelecting(PdfStampAnnotation annotation, bool afterCreate)
		{
			if (!string.IsNullOrEmpty(annotation.Subject))
			{
				if (annotation.Subject == "Signature")
				{
					this.signatureDragControl = new SignatureDragControl(annotation, this);
					base.AnnotationCanvas.Children.Add(this.signatureDragControl);
				}
				else
				{
					this.dragControl = new StampAnnotationDragControl(annotation, this);
					base.AnnotationCanvas.Children.Add(this.dragControl);
				}
			}
			else
			{
				this.signatureDragControl = new SignatureDragControl(annotation, this);
				base.AnnotationCanvas.Children.Add(this.signatureDragControl);
			}
			return true;
		}

		// Token: 0x06002853 RID: 10323 RVA: 0x000BD769 File Offset: 0x000BB969
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.processingPage = page;
			return true;
		}

		// Token: 0x06002854 RID: 10324 RVA: 0x000BD774 File Offset: 0x000BB974
		internal static Size GetStampPageSize(double bitmapWidth, double bitmapHeight, FS_RECTF pageBounds)
		{
			Size size = new Size(bitmapWidth * 96.0 / 72.0, bitmapHeight * 96.0 / 72.0);
			if (size.Width > (double)(pageBounds.Width / 2f) || size.Height > (double)(pageBounds.Height / 2f))
			{
				double num = size.Width / (double)(pageBounds.Width / 2f);
				double num2 = size.Height / (double)(pageBounds.Height / 2f);
				double num3 = Math.Max(num, num2);
				size = new Size(size.Width / num3, size.Height / num3);
			}
			else if (size.Width < 10.0 && size.Height < 10.0)
			{
				double num4 = size.Width / 10.0;
				double num5 = size.Height / 10.0;
				double num6 = Math.Min(num4, num5);
				size = new Size(size.Width / num6, size.Height / num6);
			}
			return size;
		}

		// Token: 0x06002855 RID: 10325 RVA: 0x000BD89C File Offset: 0x000BBA9C
		internal static Size GetSignaturePageSize(double bitmapWidth, double bitmapHeight, FS_RECTF pageBounds)
		{
			Size size = new Size(bitmapWidth * 96.0 / 72.0, bitmapHeight * 96.0 / 72.0);
			if (size.Width != 200.0)
			{
				double num = 200.0 / size.Width;
				size = new Size(200.0, size.Height * num);
			}
			if (size.Height > (double)(pageBounds.Height / 2f))
			{
				double num2 = size.Height / (double)(pageBounds.Height / 2f);
				size = new Size(size.Width / num2, (double)(pageBounds.Height / 2f));
			}
			return size;
		}

		// Token: 0x06002856 RID: 10326 RVA: 0x000BD964 File Offset: 0x000BBB64
		private async Task<global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation>> CreateTextStampAsync(IStampTextModel textModel, PdfPage page, FS_POINTF point, FS_RECTF rect)
		{
			global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList;
			if (textModel == null)
			{
				readOnlyList = null;
			}
			else
			{
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				Color color = (Color)ColorConverter.ConvertFromString(textModel.FontColor);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				string text = ((textModel != null) ? textModel.TextContent : null);
				PDFExtStampDictionary pdfextStampDictionary = new PDFExtStampDictionary
				{
					Type = "Stamp",
					Template = "Default"
				};
				PdfStampAnnotation stamptextAnnot;
				if (textModel.IsPreset)
				{
					stamptextAnnot = new PdfStampAnnotation(page, ((PresetStampTextModel)textModel).IconName, point.X, point.Y, fs_COLOR);
					PDFExtStampDictionary pdfextStampDictionary2 = pdfextStampDictionary;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["ContentText"] = text;
					pdfextStampDictionary2.SetContentDictionary(dictionary);
				}
				else
				{
					stamptextAnnot = new PdfStampAnnotation(page, textModel.TextContent, point.X, point.Y, fs_COLOR);
					stamptextAnnot.Contents = null;
					if (((DynamicStampTextModel)textModel).DynamicProperties != null)
					{
						pdfextStampDictionary.Template = ((DynamicStampTextModel)textModel).TemplateName;
						pdfextStampDictionary.SetContentDictionary(((DynamicStampTextModel)textModel).DynamicProperties.Data);
					}
					else
					{
						PDFExtStampDictionary pdfextStampDictionary3 = pdfextStampDictionary;
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						dictionary2["ContentText"] = text;
						pdfextStampDictionary3.SetContentDictionary(dictionary2);
					}
				}
				StampUtil.SetPDFXStampDictionary(stamptextAnnot, pdfextStampDictionary);
				FS_RECTF rotatedRect = PdfRotateUtils.GetRotatedRect(rect, point, page.Rotation);
				if (page.Rotation != PageRotate.Normal)
				{
					stamptextAnnot.Dictionary["Rotate"] = PdfTypeNumber.Create((int)(page.Rotation * (PageRotate)90));
				}
				stamptextAnnot.Rectangle = rotatedRect;
				stamptextAnnot.ExtendedIconName = textModel.TextContent;
				stamptextAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				stamptextAnnot.CreationDate = stamptextAnnot.ModificationDate;
				stamptextAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
				stamptextAnnot.Subject = "Stamp";
				stamptextAnnot.Flags |= AnnotationFlags.Print;
				page.Annots.Add(stamptextAnnot);
				stamptextAnnot.TryRedrawAnnotation(false);
				await mainViewModel.OperationManager.TraceAnnotationInsertAsync(stamptextAnnot, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				if (stamptextAnnot != null)
				{
					this.startPoint = default(Point);
					readOnlyList = new PdfStampAnnotation[] { stamptextAnnot };
				}
				else
				{
					readOnlyList = null;
				}
			}
			return readOnlyList;
		}

		// Token: 0x06002857 RID: 10327 RVA: 0x000BD9C8 File Offset: 0x000BBBC8
		private async Task<global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation>> CreateImageStampAsync(StampImageModel imageModel, PdfPage page, FS_POINTF point, FS_RECTF rect)
		{
			global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList;
			if (imageModel == null)
			{
				readOnlyList = null;
			}
			else
			{
				MainViewModel vm = base.AnnotationCanvas.DataContext as MainViewModel;
				PdfStampAnnotation imgStamp = null;
				WriteableBitmap writeableBitmap = null;
				if (imageModel.ImageStampControlModel.StampImageSource != null)
				{
					if (imageModel.ImageStampControlModel.StampImageSource.Format == PixelFormats.Bgra32)
					{
						writeableBitmap = new WriteableBitmap(imageModel.ImageStampControlModel.StampImageSource);
					}
					else
					{
						writeableBitmap = new WriteableBitmap(new FormatConvertedBitmap(imageModel.ImageStampControlModel.StampImageSource, PixelFormats.Bgra32, null, 0.0));
					}
				}
				Size size = new Size((double)imageModel.ImageStampControlModel.PageSize.Width, (double)imageModel.ImageStampControlModel.PageSize.Height);
				using (PdfBitmap pdfBitmap = new PdfBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, true, false))
				{
					int num = pdfBitmap.Stride * pdfBitmap.Height;
					writeableBitmap.CopyPixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), pdfBitmap.Buffer, num, pdfBitmap.Stride);
					PdfImageObject pdfImageObject = PdfImageObject.Create(vm.Document, pdfBitmap, point.X, (float)((double)point.Y - size.Height));
					FS_MATRIX fs_MATRIX = new FS_MATRIX(size.Width, 0.0, 0.0, size.Height, (double)point.X, (double)((float)((double)point.Y - size.Height)));
					pdfImageObject.Matrix = fs_MATRIX;
					imgStamp = new PdfStampAnnotation(page, "", point.X, point.Y, FS_COLOR.SteelBlue);
					bool flag = imageModel.RemoveBackground;
					if (!flag)
					{
						flag = ConfigManager.IsRemoveSignatureBg(imageModel.ImageFilePath);
					}
					if (flag)
					{
						pdfImageObject.BlendMode = BlendTypes.FXDIB_BLEND_MULTIPLY;
						imgStamp.Dictionary["IsRemoveBg"] = PdfTypeBoolean.Create(true);
					}
					imgStamp.Flags |= AnnotationFlags.Print;
					imgStamp.NormalAppearance.Clear();
					imgStamp.NormalAppearance.Add(pdfImageObject);
				}
				if (imageModel.IsSignature)
				{
					imgStamp.Subject = "Signature";
				}
				else
				{
					imgStamp.Subject = "Stamp";
				}
				imgStamp.GenerateAppearance(AppearanceStreamModes.Normal);
				rect = imgStamp.GetRECT();
				FS_RECTF rotatedRect = PdfRotateUtils.GetRotatedRect(rect, point, page.Rotation);
				if (page.Rotation != PageRotate.Normal)
				{
					imgStamp.Dictionary["Rotate"] = PdfTypeNumber.Create((int)(page.Rotation * (PageRotate)90));
				}
				imgStamp.Rectangle = rotatedRect;
				imgStamp.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				imgStamp.CreationDate = imgStamp.ModificationDate;
				imgStamp.Text = AnnotationAuthorUtil.GetAuthorName();
				imgStamp.RegenerateAppearancesAdvance();
				page.Annots.Add(imgStamp);
				await vm.OperationManager.TraceAnnotationInsertAsync(imgStamp, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				if (imgStamp != null)
				{
					this.startPoint = default(Point);
					if (imageModel.IsSignature && !string.IsNullOrEmpty(imageModel.ImageFilePath) && File.Exists(imageModel.ImageFilePath))
					{
						ConfigManager.SetSignatureExistFlag(true);
						vm.MaybeHaveUnembeddedSignature = true;
						imgStamp.Dictionary["ImgSource"] = PdfTypeString.Create(imageModel.ImageFilePath, false, false);
					}
					else if (!string.IsNullOrEmpty(imageModel.ImageFilePath) && File.Exists(imageModel.ImageFilePath))
					{
						imgStamp.Dictionary["ImgSource"] = PdfTypeString.Create(imageModel.ImageFilePath, false, false);
					}
					vm.AnnotationMode = AnnotationMode.None;
					readOnlyList = new PdfStampAnnotation[] { imgStamp };
				}
				else
				{
					readOnlyList = null;
				}
			}
			return readOnlyList;
		}

		// Token: 0x06002858 RID: 10328 RVA: 0x000BDA2C File Offset: 0x000BBC2C
		private async Task<global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation>> CreateFormControlStampAsync(StampFormControlModel formControlModel, PdfPage page, FS_POINTF point, FS_RECTF rect)
		{
			global::System.Collections.Generic.IReadOnlyList<PdfStampAnnotation> readOnlyList;
			if (formControlModel == null)
			{
				readOnlyList = null;
			}
			else
			{
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				PdfStampAnnotation stampAnnot = new PdfStampAnnotation(page, StampIconNames.Draft, point.X, point.Y, FS_COLOR.Black);
				FS_RECTF rotatedRect = PdfRotateUtils.GetRotatedRect(rect, point, page.Rotation);
				if (page.Rotation != PageRotate.Normal)
				{
					stampAnnot.Dictionary["Rotate"] = PdfTypeNumber.Create((int)(page.Rotation * (PageRotate)90));
				}
				stampAnnot.Rectangle = rotatedRect;
				stampAnnot.ExtendedIconName = formControlModel.Name;
				stampAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				stampAnnot.CreationDate = stampAnnot.ModificationDate;
				stampAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
				stampAnnot.Subject = "FormControl";
				stampAnnot.Flags |= AnnotationFlags.Print;
				StampUtil.SetPDFXStampDictionary(stampAnnot, new PDFExtStampDictionary
				{
					Type = "FormControl"
				});
				page.Annots.Add(stampAnnot);
				stampAnnot.TryRedrawAnnotation(false);
				await mainViewModel.OperationManager.TraceAnnotationInsertAsync(stampAnnot, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				if (stampAnnot != null)
				{
					this.startPoint = default(Point);
					readOnlyList = new PdfStampAnnotation[] { stampAnnot };
				}
				else
				{
					readOnlyList = null;
				}
			}
			return readOnlyList;
		}

		// Token: 0x06002859 RID: 10329 RVA: 0x000BDA90 File Offset: 0x000BBC90
		public static int[] bytesToInt(byte[] src, int offset)
		{
			int[] array = new int[src.Length / 4 + 1];
			for (int i = 0; i < src.Length / 4; i++)
			{
				int num = (int)(src[offset] & byte.MaxValue) | ((int)(src[offset + 1] & byte.MaxValue) << 8) | ((int)(src[offset + 2] & byte.MaxValue) << 16) | ((int)(src[offset + 3] & byte.MaxValue) << 24);
				array[i] = num;
				offset += 4;
			}
			if (offset + 3 == src.Length - 1)
			{
				array[array.Length - 1] |= (int)src[offset + 3] << 24;
			}
			if (offset + 2 == src.Length - 1)
			{
				array[array.Length - 1] |= (int)src[offset + 2] << 16;
			}
			if (offset + 1 == src.Length - 1)
			{
				array[array.Length - 1] |= (int)src[offset + 1] << 8;
			}
			if (offset == src.Length - 1)
			{
				array[array.Length - 1] |= (int)src[offset];
			}
			return array;
		}

		// Token: 0x0600285A RID: 10330 RVA: 0x000BDB78 File Offset: 0x000BBD78
		public static byte[] intToBytes(int[] src, int offset)
		{
			byte[] array = new byte[src.Length * 4];
			for (int i = 0; i < src.Length; i++)
			{
				array[offset + 3] = (byte)((src[i] >> 24) & 255);
				array[offset + 2] = (byte)((src[i] >> 16) & 255);
				array[offset + 1] = (byte)((src[i] >> 8) & 255);
				array[offset] = (byte)(src[i] & 255);
				offset += 4;
			}
			return array;
		}

		// Token: 0x0600285B RID: 10331 RVA: 0x000BDBE8 File Offset: 0x000BBDE8
		public static void SetDefaultTextStampContent(PdfStampAnnotation stampAnnot, string content)
		{
			if (stampAnnot == null)
			{
				return;
			}
			content = content ?? "";
			if (stampAnnot.ExtendedIconName != content)
			{
				stampAnnot.ExtendedIconName = content;
			}
			PDFExtStampDictionary pdfextStampDictionary = StampUtil.GetPDFExtStampDictionary(stampAnnot);
			if (pdfextStampDictionary != null)
			{
				Dictionary<string, string> contentDictionary = pdfextStampDictionary.GetContentDictionary();
				if (contentDictionary != null && contentDictionary.ContainsKey("ContentText"))
				{
					contentDictionary["ContentText"] = content;
					pdfextStampDictionary.SetContentDictionary(contentDictionary);
					StampUtil.SetPDFXStampDictionary(stampAnnot, pdfextStampDictionary);
				}
			}
		}

		// Token: 0x04001154 RID: 4436
		private StampAnnotationDragControl dragControl;

		// Token: 0x04001155 RID: 4437
		private SignatureDragControl signatureDragControl;

		// Token: 0x04001156 RID: 4438
		private Point startPoint;

		// Token: 0x04001157 RID: 4439
		private PdfPage processingPage;
	}
}
