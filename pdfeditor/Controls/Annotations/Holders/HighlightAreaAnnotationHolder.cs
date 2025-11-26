using System;
using System.Collections.Generic;
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
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B6 RID: 694
	public class HighlightAreaAnnotationHolder : BaseAnnotationHolder<PdfHighlightAnnotation>
	{
		// Token: 0x0600281A RID: 10266 RVA: 0x000BC310 File Offset: 0x000BA510
		public HighlightAreaAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x0600281B RID: 10267 RVA: 0x000BC319 File Offset: 0x000BA519
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600281C RID: 10268 RVA: 0x000BC31C File Offset: 0x000BA51C
		public override void OnPageClientBoundsChanged()
		{
			AnnotationFocusControl annotationFocusControl = this.selectControl;
			if (annotationFocusControl == null)
			{
				return;
			}
			annotationFocusControl.InvalidateVisual();
		}

		// Token: 0x0600281D RID: 10269 RVA: 0x000BC330 File Offset: 0x000BA530
		public override bool OnPropertyChanged(string propertyName)
		{
			PdfHighlightAnnotation pdfHighlightAnnotation = base.SelectedAnnotation as PdfHighlightAnnotation;
			if (pdfHighlightAnnotation != null && propertyName == "HighlightAreaStroke")
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				using (requiredService.OperationManager.TraceAnnotationChange(pdfHighlightAnnotation.Page, ""))
				{
					FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.HighlightAreaStroke)).ToPdfColor();
					pdfHighlightAnnotation.Color = fs_COLOR;
				}
				pdfHighlightAnnotation.TryRedrawAnnotation(false);
			}
			return false;
		}

		// Token: 0x0600281E RID: 10270 RVA: 0x000BC3C8 File Offset: 0x000BA5C8
		protected override void OnCancel()
		{
			if (this.selectControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.selectControl);
				this.selectControl = null;
			}
			if (this.newAreaControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newAreaControl);
				this.newAreaControl = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x000BC438 File Offset: 0x000BA638
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfHighlightAnnotation>> OnCompleteCreateNewAsync()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			global::System.Collections.Generic.IReadOnlyList<PdfHighlightAnnotation> readOnlyList;
			try
			{
				if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) > 10f || Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) > 10f)
				{
					PdfHighlightAnnotation highlight = new PdfHighlightAnnotation(page);
					highlight.Subject = "AreaHighlight";
					highlight.Text = AnnotationAuthorUtil.GetAuthorName();
					Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.HighlightAreaStroke);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					highlight.Color = fs_COLOR;
					float num = Math.Min(this.createStartPoint.X, this.createEndPoint.X);
					float num2 = Math.Max(this.createStartPoint.Y, this.createEndPoint.Y);
					float num3 = Math.Max(this.createStartPoint.X, this.createEndPoint.X);
					float num4 = Math.Min(this.createStartPoint.Y, this.createEndPoint.Y);
					FS_RECTF fs_RECTF = new FS_RECTF(num, num2, num3, num4);
					highlight.QuadPoints = new PdfQuadPointsCollection { fs_RECTF.ToQuadPoints() };
					highlight.Flags |= AnnotationFlags.Print;
					highlight.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					highlight.CreationDate = highlight.ModificationDate;
					if (page.Annots != null)
					{
						page.Annots.Add(highlight);
						highlight.RegenerateAppearancesWithoutRound();
					}
					await requiredService.OperationManager.TraceAnnotationInsertAsync(highlight, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
					this.createStartPoint = default(FS_POINTF);
					this.createEndPoint = default(FS_POINTF);
					if (highlight != null)
					{
						readOnlyList = new PdfHighlightAnnotation[] { highlight };
					}
					else
					{
						readOnlyList = null;
					}
				}
				else
				{
					readOnlyList = null;
				}
			}
			finally
			{
				if (this.selectControl != null)
				{
					base.AnnotationCanvas.Children.Remove(this.selectControl);
					this.selectControl = null;
				}
				if (this.newAreaControl != null)
				{
					base.AnnotationCanvas.Children.Remove(this.newAreaControl);
					this.newAreaControl = null;
				}
			}
			return readOnlyList;
		}

		// Token: 0x06002820 RID: 10272 RVA: 0x000BC47C File Offset: 0x000BA67C
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (page != base.CurrentPage)
			{
				return;
			}
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				base.AnnotationCanvas.PdfViewer.DeselectText();
				double num = Math.Min(point.X, this.startPoint.X);
				double num2 = Math.Min(point.Y, this.startPoint.Y);
				double num3 = Math.Max(point.X, this.startPoint.X) - num;
				double num4 = Math.Max(point.Y, this.startPoint.Y) - num2;
				if (this.newAreaControl != null)
				{
					this.newAreaControl.Width = num3;
					this.newAreaControl.Height = num4;
					Canvas.SetLeft(this.newAreaControl, num);
					Canvas.SetTop(this.newAreaControl, num2);
				}
			}
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x000BC56B File Offset: 0x000BA76B
		protected override bool OnSelecting(PdfHighlightAnnotation annotation, bool afterCreate)
		{
			this.selectControl = new AnnotationFocusControl(base.AnnotationCanvas)
			{
				Annotation = annotation,
				IsTextMarkupFocusVisible = !afterCreate
			};
			base.AnnotationCanvas.Children.Add(this.selectControl);
			return true;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x000BC5A8 File Offset: 0x000BA7A8
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			if (this.newAreaControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newAreaControl);
			}
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.startPoint = point;
				object dataContext = base.AnnotationCanvas.DataContext;
				Color color = (Color)ColorConverter.ConvertFromString("#000000");
				this.newAreaControl = new Rectangle
				{
					Stroke = new SolidColorBrush(color),
					StrokeThickness = 1.0
				};
				Canvas.SetLeft(this.newAreaControl, this.startPoint.X);
				Canvas.SetTop(this.newAreaControl, this.startPoint.Y);
				base.AnnotationCanvas.Children.Add(this.newAreaControl);
				return true;
			}
			return false;
		}

		// Token: 0x04001137 RID: 4407
		private Rectangle newAreaControl;

		// Token: 0x04001138 RID: 4408
		private FS_POINTF createStartPoint;

		// Token: 0x04001139 RID: 4409
		private FS_POINTF createEndPoint;

		// Token: 0x0400113A RID: 4410
		public Point startPoint;

		// Token: 0x0400113B RID: 4411
		private AnnotationFocusControl selectControl;
	}
}
