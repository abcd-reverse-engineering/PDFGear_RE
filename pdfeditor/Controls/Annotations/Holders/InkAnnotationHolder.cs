using System;
using System.Collections.Generic;
using System.Linq;
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
	// Token: 0x020002B7 RID: 695
	public class InkAnnotationHolder : BaseAnnotationHolder<PdfInkAnnotation>
	{
		// Token: 0x06002823 RID: 10275 RVA: 0x000BC693 File Offset: 0x000BA893
		public InkAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
			this.lineFsPoints = new List<FS_POINTF>();
			this.newlines = new List<Line>();
			this.linePoints = new List<Point>();
		}

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x06002824 RID: 10276 RVA: 0x000BC6BD File Offset: 0x000BA8BD
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x000BC6C0 File Offset: 0x000BA8C0
		public override void OnPageClientBoundsChanged()
		{
			AnnotationFocusControl annotationFocusControl = this.selectControl;
			if (annotationFocusControl != null)
			{
				annotationFocusControl.InvalidateVisual();
			}
			if (base.State != AnnotationHolderState.CreatingNew && base.State == AnnotationHolderState.Selected)
			{
				AnnotationDragControl annotationDragControl = this.dragControl;
				if (annotationDragControl == null)
				{
					return;
				}
				annotationDragControl.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x000BC6F8 File Offset: 0x000BA8F8
		protected override void OnCancel()
		{
			if (this.dragControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.dragControl);
				this.dragControl = null;
			}
			foreach (Line line in this.newlines)
			{
				base.AnnotationCanvas.Children.Remove(line);
			}
			this.lineFsPoints.Clear();
			this.linePoints.Clear();
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x000BC7A8 File Offset: 0x000BA9A8
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfInkAnnotation>> OnCompleteCreateNewAsync()
		{
			if (this.newlines != null && this.newlines.Count > 0)
			{
				this.newlines.ForEach(delegate(Line l)
				{
					if (base.AnnotationCanvas.Children.Contains(l))
					{
						base.AnnotationCanvas.Children.Remove(l);
					}
				});
			}
			List<FS_POINTF> list = this.lineFsPoints.Distinct<FS_POINTF>().ToList<FS_POINTF>();
			PdfInkAnnotation inkAnnot = null;
			if (list.Count > 1)
			{
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				float num3 = float.MinValue;
				float num4 = float.MinValue;
				for (int i = 0; i < list.Count; i++)
				{
					num = Math.Min(list[i].X, num);
					num2 = Math.Min(list[i].Y, num2);
					num3 = Math.Max(list[i].X, num3);
					num4 = Math.Max(list[i].Y, num4);
				}
				if (Math.Sqrt((double)((num3 - num) * (num3 - num) + (num4 - num2) * (num4 - num2))) > 3.0)
				{
					this.newlines.Clear();
					MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
					PdfPage page = base.CurrentPage;
					if (page.Document == null)
					{
						throw new ArgumentException("Document");
					}
					if (page.Annots == null)
					{
						page.CreateAnnotations();
					}
					inkAnnot = new PdfInkAnnotation(page);
					Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkStroke);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					inkAnnot.Color = fs_COLOR;
					inkAnnot.Opacity = 1f;
					inkAnnot.LineStyle = new PdfBorderStyle();
					inkAnnot.LineStyle.Style = BorderStyles.Solid;
					inkAnnot.LineStyle.Width = requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkWidth;
					inkAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
					PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection = new PdfLinePointCollection<PdfInkAnnotation>();
					for (int j = 0; j < list.Count; j++)
					{
						pdfLinePointCollection.Add(list[j]);
					}
					inkAnnot.InkList = new PdfInkPointCollection();
					inkAnnot.InkList.Add(pdfLinePointCollection);
					inkAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					inkAnnot.CreationDate = inkAnnot.ModificationDate;
					inkAnnot.Flags |= AnnotationFlags.Print;
					page.Annots.Add(inkAnnot);
					await requiredService.OperationManager.TraceAnnotationInsertAsync(inkAnnot, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
					page = null;
				}
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
			this.lineFsPoints.Clear();
			global::System.Collections.Generic.IReadOnlyList<PdfInkAnnotation> readOnlyList;
			if (inkAnnot != null)
			{
				readOnlyList = new PdfInkAnnotation[] { inkAnnot };
			}
			else
			{
				readOnlyList = null;
			}
			return readOnlyList;
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x000BC7EC File Offset: 0x000BA9EC
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
				this.lineFsPoints.Add(pagePoint);
				this.linePoints.Add(point);
				List<Point> list = this.linePoints.Distinct<Point>().ToList<Point>();
				int num = list.Count<Point>();
				if (point != list.ElementAt(0) && list.Count >= 2)
				{
					Line line = this.CreateLine(new Point(list[num - 2].X, list[num - 2].Y), point);
					this.newlines.Add(line);
					base.AnnotationCanvas.Children.Add(line);
				}
			}
		}

		// Token: 0x06002829 RID: 10281 RVA: 0x000BC8C4 File Offset: 0x000BAAC4
		private Line CreateLine(Point startPoint, Point endPoint)
		{
			MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
			SolidColorBrush solidColorBrush = Brushes.Transparent;
			if (!string.IsNullOrEmpty(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkStroke))
			{
				try
				{
					solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkStroke));
				}
				catch
				{
				}
			}
			return new Line
			{
				Stroke = solidColorBrush,
				StrokeThickness = (double)mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkWidth,
				X1 = startPoint.X,
				Y1 = startPoint.Y,
				X2 = endPoint.X,
				Y2 = endPoint.Y,
				IsHitTestVisible = false
			};
		}

		// Token: 0x0600282A RID: 10282 RVA: 0x000BC994 File Offset: 0x000BAB94
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.lineFsPoints.Clear();
				this.linePoints.Clear();
				this.linePoints.Add(point);
				this.lineFsPoints.Add(pagePoint);
				return true;
			}
			return false;
		}

		// Token: 0x0600282B RID: 10283 RVA: 0x000BCA00 File Offset: 0x000BAC00
		protected override bool OnSelecting(PdfInkAnnotation annotation, bool afterCreate)
		{
			this.dragControl = new AnnotationDragControl(annotation, this);
			base.AnnotationCanvas.Children.Add(this.dragControl);
			return true;
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x000BCA27 File Offset: 0x000BAC27
		public override bool OnPropertyChanged(string propertyName)
		{
			if (propertyName == "InkStroke" || propertyName == "InkWidth")
			{
				AnnotationDragControl annotationDragControl = this.dragControl;
				return annotationDragControl != null && annotationDragControl.OnPropertyChanged(propertyName);
			}
			return false;
		}

		// Token: 0x0400113C RID: 4412
		private FS_POINTF createStartPoint;

		// Token: 0x0400113D RID: 4413
		private FS_POINTF createEndPoint;

		// Token: 0x0400113E RID: 4414
		private AnnotationFocusControl selectControl;

		// Token: 0x0400113F RID: 4415
		private List<FS_POINTF> lineFsPoints;

		// Token: 0x04001140 RID: 4416
		private List<Line> newlines;

		// Token: 0x04001141 RID: 4417
		private List<Point> linePoints;

		// Token: 0x04001142 RID: 4418
		private AnnotationDragControl dragControl;
	}
}
