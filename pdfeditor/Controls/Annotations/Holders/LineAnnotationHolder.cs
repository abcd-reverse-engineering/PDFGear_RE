using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B8 RID: 696
	public class LineAnnotationHolder : BaseAnnotationHolder<PdfLineAnnotation>
	{
		// Token: 0x0600282E RID: 10286 RVA: 0x000BCA7D File Offset: 0x000BAC7D
		public LineAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x0600282F RID: 10287 RVA: 0x000BCA86 File Offset: 0x000BAC86
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x000BCA8C File Offset: 0x000BAC8C
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (this.newLineControl == null)
				{
					throw new ArgumentException("newLineControl");
				}
				if (base.CurrentPage == null)
				{
					throw new ArgumentException("CurrentPage");
				}
				Point point;
				Point point2;
				if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createStartPoint.ToPoint(), out point) && base.AnnotationCanvas.PdfViewer.TryGetClientPoint(base.CurrentPage.PageIndex, this.createEndPoint.ToPoint(), out point2))
				{
					this.newLineControl.X1 = point.X;
					this.newLineControl.Y1 = point.Y;
					this.newLineControl.X2 = point2.X;
					this.newLineControl.Y2 = point2.Y;
					return;
				}
			}
			else if (base.State == AnnotationHolderState.Selected)
			{
				AnnotationLineControl annotationLineControl = this.editLineControl;
				if (annotationLineControl == null)
				{
					return;
				}
				annotationLineControl.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x06002831 RID: 10289 RVA: 0x000BCB84 File Offset: 0x000BAD84
		protected override void OnCancel()
		{
			if (this.newLineControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newLineControl);
				this.newLineControl = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
			if (this.editLineControl != null)
			{
				if (this.editLineControl.IsMouseCaptured)
				{
					this.editLineControl.ReleaseMouseCapture();
				}
				base.AnnotationCanvas.Children.Remove(this.editLineControl);
				this.editLineControl = null;
			}
		}

		// Token: 0x06002832 RID: 10290 RVA: 0x000BCC0C File Offset: 0x000BAE0C
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfLineAnnotation>> OnCompleteCreateNewAsync()
		{
			base.AnnotationCanvas.Children.Remove(this.newLineControl);
			PdfLineAnnotation newLineAnnot = null;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) > 10f || Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) > 10f)
			{
				newLineAnnot = new PdfLineAnnotation(page);
				Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineStroke);
				newLineAnnot.Color = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				newLineAnnot.CaptionPosition = CaptionPositions.Inline;
				newLineAnnot.Line = new PdfLinePointCollection<PdfLineAnnotation>();
				newLineAnnot.Line.Add(new FS_POINTF(this.createStartPoint.X, this.createStartPoint.Y));
				newLineAnnot.Line.Add(new FS_POINTF(this.createEndPoint.X, this.createEndPoint.Y));
				newLineAnnot.LineStyle = new PdfBorderStyle();
				newLineAnnot.LineStyle.Width = requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineWidth;
				newLineAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
				if (requiredService.AnnotationMode == AnnotationMode.Arrow)
				{
					newLineAnnot.LineEnding = new PdfLineEndingCollection(LineEndingStyles.None, LineEndingStyles.OpenArrow);
				}
				newLineAnnot.Cap = false;
				newLineAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				newLineAnnot.CreationDate = newLineAnnot.ModificationDate;
				newLineAnnot.Flags |= AnnotationFlags.Print;
				page.Annots.Add(newLineAnnot);
				await requiredService.OperationManager.TraceAnnotationInsertAsync(newLineAnnot, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
			}
			this.newLineControl = null;
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
			global::System.Collections.Generic.IReadOnlyList<PdfLineAnnotation> readOnlyList;
			if (newLineAnnot != null)
			{
				readOnlyList = new PdfLineAnnotation[] { newLineAnnot };
			}
			else
			{
				readOnlyList = null;
			}
			return readOnlyList;
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x000BCC50 File Offset: 0x000BAE50
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
				this.newLineControl.X2 = point.X;
				this.newLineControl.Y2 = point.Y;
			}
		}

		// Token: 0x06002834 RID: 10292 RVA: 0x000BCCB4 File Offset: 0x000BAEB4
		private Line CreateLine(Point startPoint)
		{
			MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
			SolidColorBrush solidColorBrush = Brushes.Transparent;
			if (!string.IsNullOrEmpty(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineStroke))
			{
				try
				{
					solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineStroke));
				}
				catch
				{
				}
			}
			return new Line
			{
				Stroke = solidColorBrush,
				StrokeThickness = (double)mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineWidth,
				X1 = startPoint.X,
				Y1 = startPoint.Y,
				X2 = startPoint.X,
				Y2 = startPoint.Y,
				IsHitTestVisible = false
			};
		}

		// Token: 0x06002835 RID: 10293 RVA: 0x000BCD84 File Offset: 0x000BAF84
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.newLineControl = this.CreateLine(point);
				base.AnnotationCanvas.Children.Add(this.newLineControl);
				return true;
			}
			return false;
		}

		// Token: 0x06002836 RID: 10294 RVA: 0x000BCDE8 File Offset: 0x000BAFE8
		protected override bool OnSelecting(PdfLineAnnotation annotation, bool afterCreate)
		{
			if (this.editLineControl != null)
			{
				throw new ArgumentException("editLineControl");
			}
			global::System.Collections.Generic.IReadOnlyList<FS_POINTF> line = annotation.GetLine();
			if (line == null || line.Count < 2)
			{
				return false;
			}
			this.editLineControl = new AnnotationLineControl(annotation, this);
			base.AnnotationCanvas.Children.Add(this.editLineControl);
			return true;
		}

		// Token: 0x06002837 RID: 10295 RVA: 0x000BCE42 File Offset: 0x000BB042
		public override bool OnPropertyChanged(string propertyName)
		{
			if (propertyName == "LineStroke" || propertyName == "LineWidth")
			{
				AnnotationLineControl annotationLineControl = this.editLineControl;
				return annotationLineControl != null && annotationLineControl.OnPropertyChanged(propertyName);
			}
			return false;
		}

		// Token: 0x04001143 RID: 4419
		private Line newLineControl;

		// Token: 0x04001144 RID: 4420
		private FS_POINTF createStartPoint;

		// Token: 0x04001145 RID: 4421
		private FS_POINTF createEndPoint;

		// Token: 0x04001146 RID: 4422
		private AnnotationLineControl editLineControl;
	}
}
