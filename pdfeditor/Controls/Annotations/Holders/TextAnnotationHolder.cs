using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.PdfViewerDecorators;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002BD RID: 701
	public class TextAnnotationHolder : BaseAnnotationHolder<PdfTextAnnotation>
	{
		// Token: 0x0600285F RID: 10335 RVA: 0x000BDE94 File Offset: 0x000BC094
		public TextAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x06002860 RID: 10336 RVA: 0x000BDE9D File Offset: 0x000BC09D
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002861 RID: 10337 RVA: 0x000BDEA0 File Offset: 0x000BC0A0
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (base.CurrentPage == null)
				{
					throw new ArgumentException("CurrentPage");
				}
			}
			else if (base.State == AnnotationHolderState.Selected)
			{
				AnnotationTextControl annotationTextControl = this.editControl;
				if (annotationTextControl == null)
				{
					return;
				}
				annotationTextControl.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x06002862 RID: 10338 RVA: 0x000BDED7 File Offset: 0x000BC0D7
		protected override void OnCancel()
		{
			this.createPoint = default(FS_POINTF);
			if (this.editControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.editControl);
				this.editControl = null;
			}
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x000BDF0C File Offset: 0x000BC10C
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfTextAnnotation>> OnCompleteCreateNewAsync()
		{
			PdfTextAnnotation textAnnot = null;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			requiredService.AnnotationMode = AnnotationMode.None;
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			textAnnot = new PdfTextAnnotation(page);
			textAnnot.Flags |= AnnotationFlags.Print | AnnotationFlags.NoZoom | AnnotationFlags.NoRotate;
			textAnnot.Color = FS_COLOR.Red;
			textAnnot.Contents = "";
			textAnnot.Opacity = 1f;
			textAnnot.Subject = "";
			textAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
			textAnnot.StandardIconName = IconNames.Note;
			textAnnot.Rectangle = TextAnnotationHolder.GetBounds(this.createPoint, textAnnot.StandardIconName);
			textAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
			textAnnot.CreationDate = textAnnot.ModificationDate;
			PdfPopupAnnotation pdfPopupAnnotation = new PdfPopupAnnotation(page);
			pdfPopupAnnotation.Parent = textAnnot;
			pdfPopupAnnotation.IsOpen = true;
			PdfAnnotation pdfAnnotation = pdfPopupAnnotation;
			FS_RECTF rect = textAnnot.GetRECT();
			AnnotationCanvas annotationCanvas = base.AnnotationCanvas;
			pdfAnnotation.Rectangle = TextAnnotationHolder.GetPopupBounds(rect, (annotationCanvas != null) ? annotationCanvas.PdfViewer : null, page, 180.0, 140.0);
			textAnnot.Popup = pdfPopupAnnotation;
			textAnnot.RegenerateAppearancesAdvance();
			page.Annots.Add(textAnnot);
			page.Annots.Add(pdfPopupAnnotation);
			base.AnnotationCanvas.PopupHolder.ClearAnnotationPopup();
			AnnotationPopupHolder popupHolder = base.AnnotationCanvas.PopupHolder;
			PdfDocument document = base.AnnotationCanvas.PdfViewer.Document;
			PdfPage pdfPage;
			if (document == null)
			{
				pdfPage = null;
			}
			else
			{
				PdfPageCollection pages = document.Pages;
				pdfPage = ((pages != null) ? pages.CurrentPage : null);
			}
			popupHolder.InitAnnotationPopup(pdfPage);
			await requiredService.OperationManager.TraceAnnotationInsertAsync(textAnnot, "");
			await page.TryRedrawPageAsync(default(CancellationToken));
			this.createPoint = default(FS_POINTF);
			return new PdfTextAnnotation[] { textAnnot };
		}

		// Token: 0x06002864 RID: 10340 RVA: 0x000BDF50 File Offset: 0x000BC150
		private static FS_RECTF GetPopupBounds(FS_RECTF textAnnotRect, PdfViewer viewer, PdfPage page, double width, double height)
		{
			float num = textAnnotRect.right + 40f;
			if (viewer != null && page.Rotation == PageRotate.Normal)
			{
				Rect rect = viewer.CalcActualRect(page.PageIndex);
				if (!rect.IsEmpty && rect != default(Rect) && viewer.ViewportWidth / 3.0 * 2.0 > rect.Width)
				{
					num = page.GetEffectiveSize(PageRotate.Normal, false).Width + 8f;
				}
			}
			return new FS_RECTF((double)num, (double)textAnnotRect.top, (double)num + width, (double)textAnnotRect.top - height);
		}

		// Token: 0x06002865 RID: 10341 RVA: 0x000BDFF0 File Offset: 0x000BC1F0
		private static FS_RECTF GetBounds(FS_POINTF point, IconNames icon)
		{
			float num = 20f;
			float num2 = 20f;
			switch (icon)
			{
			case IconNames.Note:
				num2 = 17.696f;
				num = 20.836f;
				break;
			case IconNames.Comment:
				num2 = 19.414f;
				num = 19.414f;
				break;
			case IconNames.Key:
				num2 = 11.208f;
				num = 20.036f;
				break;
			case IconNames.Help:
				num2 = 21.71f;
				num = 21.712f;
				break;
			case IconNames.NewParagraph:
				num2 = 15.612f;
				num = 21.175f;
				break;
			case IconNames.Paragraph:
				num2 = 21.324f;
				num = 21.766f;
				break;
			case IconNames.Insert:
				num2 = 19.802f;
				num = 22.031f;
				break;
			}
			return new FS_RECTF(point.X, point.Y + num, point.X + num2, point.Y);
		}

		// Token: 0x06002866 RID: 10342 RVA: 0x000BE0B2 File Offset: 0x000BC2B2
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (page != base.CurrentPage)
			{
				return;
			}
			this.createPoint = pagePoint;
		}

		// Token: 0x06002867 RID: 10343 RVA: 0x000BE0C8 File Offset: 0x000BC2C8
		protected override bool OnSelecting(PdfTextAnnotation annotation, bool afterCreate)
		{
			if (this.editControl != null)
			{
				throw new ArgumentException("editControl");
			}
			this.editControl = new AnnotationTextControl(annotation, this);
			base.AnnotationCanvas.Children.Add(this.editControl);
			base.AnnotationCanvas.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
			{
				PdfViewer pdfViewer = this.AnnotationCanvas.PdfViewer;
				PdfPageCollection pdfPageCollection;
				if (pdfViewer == null)
				{
					pdfPageCollection = null;
				}
				else
				{
					PdfDocument document = pdfViewer.Document;
					pdfPageCollection = ((document != null) ? document.Pages : null);
				}
				PdfPageCollection pdfPageCollection2 = pdfPageCollection;
				if (pdfPageCollection2 != null && pdfPageCollection2.CurrentPage != annotation.Page)
				{
					pdfViewer.CurrentIndex = annotation.Page.PageIndex;
				}
				if (afterCreate)
				{
					this.AnnotationCanvas.PopupHolder.FocusPopupTextBox(annotation, afterCreate);
				}
			}));
			return true;
		}

		// Token: 0x06002868 RID: 10344 RVA: 0x000BE14B File Offset: 0x000BC34B
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createPoint = pagePoint;
			return true;
		}

		// Token: 0x06002869 RID: 10345 RVA: 0x000BE155 File Offset: 0x000BC355
		public override bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x04001158 RID: 4440
		private FS_POINTF createPoint;

		// Token: 0x04001159 RID: 4441
		private AnnotationTextControl editControl;
	}
}
