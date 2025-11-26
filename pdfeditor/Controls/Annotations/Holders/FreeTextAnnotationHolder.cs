using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;
using PDFKit.Utils.PdfRichTextStrings;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B4 RID: 692
	public class FreeTextAnnotationHolder : BaseAnnotationHolder<PdfFreeTextAnnotation>
	{
		// Token: 0x0600280B RID: 10251 RVA: 0x000BBA0D File Offset: 0x000B9C0D
		public FreeTextAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x0600280C RID: 10252 RVA: 0x000BBA16 File Offset: 0x000B9C16
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600280D RID: 10253 RVA: 0x000BBA1C File Offset: 0x000B9C1C
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (base.CurrentPage == null)
				{
					throw new ArgumentException("CurrentPage");
				}
				Point point;
				Point point2;
				if (this.newRectControl != null && base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createStartPoint.ToPoint(), out point) && base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createEndPoint.ToPoint(), out point2))
				{
					Rect rect = new Rect(point, point2);
					this.newRectControl.Width = rect.Width;
					this.newRectControl.Height = rect.Height;
					Canvas.SetLeft(this.newRectControl, rect.Left);
					Canvas.SetTop(this.newRectControl, rect.Top);
					return;
				}
			}
			else if (base.State == AnnotationHolderState.Selected)
			{
				AnnotationFreeTextEditor annotationFreeTextEditor = this.editFreeTextControl;
				if (annotationFreeTextEditor == null)
				{
					return;
				}
				annotationFreeTextEditor.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x0600280E RID: 10254 RVA: 0x000BBB18 File Offset: 0x000B9D18
		protected override async void OnCancel()
		{
			if (this.newRectControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newRectControl);
				this.newRectControl = null;
			}
			if (this.editFreeTextControl != null)
			{
				AnnotationFreeTextEditor eftc = this.editFreeTextControl;
				this.editFreeTextControl = null;
				if (base.State == AnnotationHolderState.Selected)
				{
					PdfPage page = eftc.Annotation.Page;
					eftc.Apply();
					await page.TryRedrawPageAsync(default(CancellationToken));
				}
				base.AnnotationCanvas.Children.Remove(eftc);
				eftc = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x000BBB50 File Offset: 0x000B9D50
		private static PdfRichTextString CreateRichTextString(string str, PdfRichTextStyle ds)
		{
			PdfRichTextString pdfRichTextString = new PdfRichTextString
			{
				DefaultStyle = ds
			};
			if (string.IsNullOrEmpty(str))
			{
				pdfRichTextString.Text = new PdfRichTextElement
				{
					Tag = PdfRichTextElementTag.Body,
					Style = new PdfRichTextStyle?(ds)
				};
			}
			else
			{
				pdfRichTextString.Text = new PdfRichTextElement
				{
					Tag = PdfRichTextElementTag.Body,
					Style = new PdfRichTextStyle?(ds),
					Children = 
					{
						new PdfRichTextElement
						{
							Tag = PdfRichTextElementTag.P,
							Children = 
							{
								new PdfRichTextElement
								{
									Tag = PdfRichTextElementTag.Span,
									Children = 
									{
										new PdfRichTextRawElement(str)
									}
								}
							}
						}
					}
				};
			}
			return pdfRichTextString;
		}

		// Token: 0x06002810 RID: 10256 RVA: 0x000BBBF0 File Offset: 0x000B9DF0
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfFreeTextAnnotation>> OnCompleteCreateNewAsync()
		{
			if (this.newRectControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newRectControl);
			}
			PdfFreeTextAnnotation freeTextAnnot = null;
			MainViewModel vm = Ioc.Default.GetRequiredService<MainViewModel>();
			AnnotationMode annotationMode = vm.AnnotationMode;
			if (annotationMode == AnnotationMode.TextBox)
			{
				vm.AnnotationMode = AnnotationMode.None;
			}
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			global::System.Collections.Generic.IReadOnlyList<PdfFreeTextAnnotation> readOnlyList;
			try
			{
				bool flag;
				if (annotationMode == AnnotationMode.TextBox)
				{
					flag = true;
				}
				else
				{
					if (annotationMode != AnnotationMode.Text)
					{
						return null;
					}
					flag = false;
				}
				freeTextAnnot = new PdfFreeTextAnnotation(page);
				freeTextAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
				Color color = (Color)ColorConverter.ConvertFromString(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxStroke);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				PdfDefaultAppearance @default = PdfDefaultAppearance.Default;
				PdfRichTextStyle pdfRichTextStyle = PdfRichTextStyle.Default;
				if (flag)
				{
					color = (Color)ColorConverter.ConvertFromString(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFill);
					FS_COLOR fs_COLOR2 = color.ToPdfColor();
					freeTextAnnot.Color = fs_COLOR2;
					color = (Color)ColorConverter.ConvertFromString(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontColor);
					FS_COLOR fs_COLOR3 = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					@default = new PdfDefaultAppearance(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontName, vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontSize, fs_COLOR, fs_COLOR3);
					pdfRichTextStyle = @default.ToRichTextStyle();
					pdfRichTextStyle.Color = new Color?(color);
				}
				else
				{
					color = (Color)ColorConverter.ConvertFromString(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextFontColor);
					FS_COLOR fs_COLOR4 = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					@default = new PdfDefaultAppearance(vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontName, vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextFontSize, fs_COLOR, fs_COLOR4);
					pdfRichTextStyle = @default.ToRichTextStyle();
					freeTextAnnot.Intent = AnnotationIntent.FreeTextTypeWriter;
					@default.StrokeColor = new FS_COLOR(255, 0, 0, 0);
					pdfRichTextStyle.Color = new Color?(color);
				}
				freeTextAnnot.DefaultAppearance = @default.ToString();
				freeTextAnnot.DefaultStyle = pdfRichTextStyle.ToString();
				PdfRichTextString pdfRichTextString = FreeTextAnnotationHolder.CreateRichTextString("", pdfRichTextStyle);
				freeTextAnnot.RichText = pdfRichTextString.ToString();
				freeTextAnnot.Contents = pdfRichTextString.ContentText;
				if (page.Rotation != PageRotate.Normal)
				{
					freeTextAnnot.Dictionary["Rotate"] = PdfTypeNumber.Create((int)(page.Rotation * (PageRotate)90));
				}
				if (flag)
				{
					freeTextAnnot.BorderStyle = new PdfBorderStyle();
					freeTextAnnot.BorderStyle.Width = vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxThickness;
					float num = Math.Min(this.createStartPoint.X, this.createEndPoint.X);
					float num2 = Math.Max(this.createStartPoint.Y, this.createEndPoint.Y);
					float num3 = Math.Max(this.createStartPoint.X, this.createEndPoint.X) - num;
					float num4 = num2 - Math.Min(this.createStartPoint.Y, this.createEndPoint.Y);
					FS_RECTF rotatedRect = new FS_RECTF(num, num2, num + num3, num2 - num4);
					if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) < 5f && Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) < 5f)
					{
						FS_RECTF originalRectangle = PdfRotateUtils.GetOriginalRectangle(rotatedRect, page.Rotation);
						if (originalRectangle.Width < 90f)
						{
							originalRectangle.right = originalRectangle.left + 90f;
						}
						if (originalRectangle.Height < 48f)
						{
							originalRectangle.bottom = originalRectangle.top - 48f;
						}
						FS_MATRIX fs_MATRIX = new FS_MATRIX();
						fs_MATRIX.SetIdentity();
						PdfRotateUtils.RotateMatrix(page.Rotation, fs_MATRIX);
						fs_MATRIX.TransformRect(ref originalRectangle);
						rotatedRect = PdfRotateUtils.GetRotatedRect(originalRectangle, this.createStartPoint, page.Rotation);
					}
					freeTextAnnot.Rectangle = rotatedRect;
				}
				else
				{
					Typeface typeface = new Typeface(new FontFamily(PdfRichTextStyle.Default.FontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
					new FormattedText(freeTextAnnot.Contents, CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight, typeface, (double)PdfRichTextStyle.Default.FontSize.Value, Brushes.Black, 1.0);
					float x = this.createEndPoint.X;
					float num5 = this.createEndPoint.Y + vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontSize / 2f;
					float num6 = x + 70f + 10f;
					float num7 = num5 - vm.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontSize;
					FS_RECTF fs_RECTF = new FS_RECTF(x, num5, num6, num7);
					if (page.Rotation != PageRotate.Normal)
					{
						FS_MATRIX fs_MATRIX2 = new FS_MATRIX();
						fs_MATRIX2.SetIdentity();
						fs_MATRIX2.Translate(-fs_RECTF.left, -fs_RECTF.bottom - fs_RECTF.Height / 2f, false);
						fs_MATRIX2.Rotate((float)((double)(page.Rotation * (PageRotate)90) * 3.1415926535897931 / 180.0), false);
						fs_MATRIX2.Translate(fs_RECTF.left, fs_RECTF.bottom + fs_RECTF.Height / 2f, false);
						fs_MATRIX2.TransformRect(ref fs_RECTF);
					}
					freeTextAnnot.Rectangle = fs_RECTF;
				}
				freeTextAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				freeTextAnnot.CreationDate = freeTextAnnot.ModificationDate;
				freeTextAnnot.Flags |= AnnotationFlags.Print;
				page.Annots.Add(freeTextAnnot);
				await freeTextAnnot.RegenerateAppearancesWithRichTextAsync();
				await vm.OperationManager.TraceAnnotationInsertAsync(freeTextAnnot, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				if (freeTextAnnot != null)
				{
					readOnlyList = new PdfFreeTextAnnotation[] { freeTextAnnot };
				}
				else
				{
					readOnlyList = null;
				}
			}
			finally
			{
				this.newRectControl = null;
				this.createStartPoint = default(FS_POINTF);
				this.createEndPoint = default(FS_POINTF);
			}
			return readOnlyList;
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x000BBC34 File Offset: 0x000B9E34
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (page != base.CurrentPage)
			{
				return;
			}
			this.createEndPoint = pagePoint;
			Point point;
			Point point2;
			if (this.newRectControl != null && base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createStartPoint.ToPoint(), out point) && base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createEndPoint.ToPoint(), out point2))
			{
				Rect rect = new Rect(point, point2);
				this.newRectControl.Width = rect.Width;
				this.newRectControl.Height = rect.Height;
				Canvas.SetLeft(this.newRectControl, rect.Left);
				Canvas.SetTop(this.newRectControl, rect.Top);
			}
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x000BBD04 File Offset: 0x000B9F04
		private Rectangle CreateRectangle(Point startPoint)
		{
			object dataContext = base.AnnotationCanvas.DataContext;
			SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 24, 146, byte.MaxValue));
			Rectangle rectangle = new Rectangle();
			rectangle.Stroke = solidColorBrush;
			rectangle.StrokeThickness = 2.0;
			rectangle.Fill = new SolidColorBrush(Color.FromArgb(127, byte.MaxValue, byte.MaxValue, byte.MaxValue));
			rectangle.IsHitTestVisible = false;
			rectangle.Width = 0.0;
			rectangle.Height = 0.0;
			Canvas.SetLeft(rectangle, startPoint.X);
			Canvas.SetTop(rectangle, startPoint.Y);
			return rectangle;
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x000BBDB4 File Offset: 0x000B9FB4
		protected override bool OnSelecting(PdfFreeTextAnnotation annotation, bool afterCreate)
		{
			if (this.editFreeTextControl != null)
			{
				return false;
			}
			this.editFreeTextControl = new AnnotationFreeTextEditor(annotation, this);
			base.AnnotationCanvas.Children.Add(this.editFreeTextControl);
			if (afterCreate)
			{
				this.editFreeTextControl.GoToEditing();
				this.editFreeTextControl.GetRichTextBox().SelectAll();
			}
			return true;
		}

		// Token: 0x06002814 RID: 10260 RVA: 0x000BBE10 File Offset: 0x000BA010
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				if (mainViewModel != null && mainViewModel.AnnotationMode == AnnotationMode.TextBox)
				{
					this.newRectControl = this.CreateRectangle(point);
					base.AnnotationCanvas.Children.Add(this.newRectControl);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002815 RID: 10261 RVA: 0x000BBE90 File Offset: 0x000BA090
		public override bool OnPropertyChanged(string propertyName)
		{
			if (!(propertyName == "TextBoxStroke") && !(propertyName == "TextBoxThickness") && !(propertyName == "TextBoxFill") && !(propertyName == "TextFontColor") && !(propertyName == "TextFontSize") && !(propertyName == "TextBoxFontColor") && !(propertyName == "TextBoxFontSize"))
			{
				return false;
			}
			PdfFreeTextAnnotation pdfFreeTextAnnotation = (PdfFreeTextAnnotation)base.SelectedAnnotation;
			if (propertyName == "TextFontColor" || propertyName == "TextFontSize" || propertyName == "TextBoxFontColor" || propertyName == "TextBoxFontSize")
			{
				return this.editFreeTextControl.OnPropertyChanged(propertyName);
			}
			this.Cancel();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			string text = pdfFreeTextAnnotation.DefaultAppearance;
			if (propertyName == "TextBoxStroke" || propertyName == "TextBoxFill")
			{
				if (propertyName == "TextBoxStroke")
				{
					FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxStroke)).ToPdfColor();
					PdfDefaultAppearance @default;
					if (!PdfDefaultAppearance.TryParse(pdfFreeTextAnnotation.DefaultAppearance, out @default))
					{
						@default = PdfDefaultAppearance.Default;
					}
					@default.StrokeColor = fs_COLOR;
					text = @default.ToString();
				}
				else if (propertyName == "TextBoxFill")
				{
					FS_COLOR fs_COLOR2 = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFill)).ToPdfColor();
					pdfFreeTextAnnotation.Color = fs_COLOR2;
				}
			}
			using (requiredService.OperationManager.TraceAnnotationChange(pdfFreeTextAnnotation.Page, ""))
			{
				pdfFreeTextAnnotation.DefaultAppearance = text;
				if (propertyName == "TextBoxThickness")
				{
					if (pdfFreeTextAnnotation.BorderStyle == null)
					{
						pdfFreeTextAnnotation.BorderStyle = new PdfBorderStyle();
					}
					pdfFreeTextAnnotation.BorderStyle.Width = requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxThickness;
				}
				pdfFreeTextAnnotation.RegenerateAppearancesWithRichText();
			}
			pdfFreeTextAnnotation.CreateEmptyAppearance(AppearanceStreamModes.Normal);
			base.Select(pdfFreeTextAnnotation, false);
			return true;
		}

		// Token: 0x04001133 RID: 4403
		private Rectangle newRectControl;

		// Token: 0x04001134 RID: 4404
		private FS_POINTF createStartPoint;

		// Token: 0x04001135 RID: 4405
		private FS_POINTF createEndPoint;

		// Token: 0x04001136 RID: 4406
		private AnnotationFreeTextEditor editFreeTextControl;
	}
}
