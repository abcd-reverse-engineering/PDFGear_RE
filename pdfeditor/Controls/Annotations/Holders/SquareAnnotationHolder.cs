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
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002BA RID: 698
	public class SquareAnnotationHolder : BaseAnnotationHolder<PdfFigureAnnotation>
	{
		// Token: 0x06002841 RID: 10305 RVA: 0x000BD1C4 File Offset: 0x000BB3C4
		public SquareAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06002842 RID: 10306 RVA: 0x000BD1CD File Offset: 0x000BB3CD
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002843 RID: 10307 RVA: 0x000BD1D0 File Offset: 0x000BB3D0
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (this.newSquareControl == null)
				{
					throw new ArgumentException("newSquareControl");
				}
				if (base.CurrentPage == null)
				{
					throw new ArgumentException("CurrentPage");
				}
			}
			else if (base.State == AnnotationHolderState.Selected)
			{
				AnnotationDragEditControl annotationDragEditControl = this.editControl;
				if (annotationDragEditControl == null)
				{
					return;
				}
				annotationDragEditControl.OnPageClientBoundsChanged();
			}
		}

		// Token: 0x06002844 RID: 10308 RVA: 0x000BD228 File Offset: 0x000BB428
		protected override void OnCancel()
		{
			if (this.editControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.editControl);
				this.editControl = null;
			}
			if (this.newSquareControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newSquareControl);
				this.newSquareControl = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x06002845 RID: 10309 RVA: 0x000BD298 File Offset: 0x000BB498
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfFigureAnnotation>> OnCompleteCreateNewAsync()
		{
			base.AnnotationCanvas.Children.Remove(this.newSquareControl);
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			global::System.Collections.Generic.IReadOnlyList<PdfFigureAnnotation> readOnlyList;
			if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) > 10f || Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) > 10f)
			{
				PdfFigureAnnotation squareAnnot = new PdfSquareAnnotation(page);
				squareAnnot.Subject = "Rectangle";
				Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeStroke);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				squareAnnot.Color = fs_COLOR;
				squareAnnot.Opacity = 1f;
				squareAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
				Color color2 = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeFill);
				FS_COLOR fs_COLOR2 = new FS_COLOR((int)color2.A, (int)color2.R, (int)color2.G, (int)color2.B);
				squareAnnot.InteriorColor = fs_COLOR2;
				float num = Math.Min(this.createStartPoint.X, this.createEndPoint.X);
				float num2 = Math.Max(this.createStartPoint.Y, this.createEndPoint.Y);
				float num3 = Math.Max(this.createStartPoint.X, this.createEndPoint.X);
				float num4 = Math.Min(this.createStartPoint.Y, this.createEndPoint.Y);
				squareAnnot.Rectangle = new FS_RECTF(num, num2, num3, num4);
				squareAnnot.BorderStyle = new PdfBorderStyle();
				squareAnnot.BorderStyle.Style = BorderStyles.Solid;
				squareAnnot.BorderStyle.Width = requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeThickness;
				squareAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				squareAnnot.CreationDate = squareAnnot.ModificationDate;
				squareAnnot.Flags |= AnnotationFlags.Print;
				page.Annots.Add(squareAnnot);
				await requiredService.OperationManager.TraceAnnotationInsertAsync(squareAnnot, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				this.createStartPoint = default(FS_POINTF);
				this.createEndPoint = default(FS_POINTF);
				if (squareAnnot != null)
				{
					readOnlyList = new PdfFigureAnnotation[] { squareAnnot };
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
			return readOnlyList;
		}

		// Token: 0x06002846 RID: 10310 RVA: 0x000BD2DC File Offset: 0x000BB4DC
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
				double num = Math.Min(point.X, this.startPoint.X);
				double num2 = Math.Min(point.Y, this.startPoint.Y);
				double num3 = Math.Max(point.X, this.startPoint.X) - num;
				double num4 = Math.Max(point.Y, this.startPoint.Y) - num2;
				if (this.newSquareControl != null)
				{
					this.newSquareControl.Width = num3;
					this.newSquareControl.Height = num4;
					Canvas.SetLeft(this.newSquareControl, num);
					Canvas.SetTop(this.newSquareControl, num2);
				}
			}
		}

		// Token: 0x06002847 RID: 10311 RVA: 0x000BD3BB File Offset: 0x000BB5BB
		protected override bool OnSelecting(PdfFigureAnnotation annotation, bool afterCreate)
		{
			this.editControl = new AnnotationDragEditControl(annotation, this);
			base.AnnotationCanvas.Children.Add(this.editControl);
			return true;
		}

		// Token: 0x06002848 RID: 10312 RVA: 0x000BD3E4 File Offset: 0x000BB5E4
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.startPoint = point;
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				Color color = (Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeStroke);
				this.newSquareControl = new Rectangle
				{
					Stroke = new SolidColorBrush(color),
					StrokeThickness = (double)mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeThickness
				};
				Canvas.SetLeft(this.newSquareControl, this.startPoint.X);
				Canvas.SetTop(this.newSquareControl, this.startPoint.Y);
				base.AnnotationCanvas.Children.Add(this.newSquareControl);
				return true;
			}
			return false;
		}

		// Token: 0x06002849 RID: 10313 RVA: 0x000BD4C9 File Offset: 0x000BB6C9
		public override bool OnPropertyChanged(string propertyName)
		{
			if (propertyName == "ShapeFill" || propertyName == "ShapeStroke" || propertyName == "ShapeThickness")
			{
				AnnotationDragEditControl annotationDragEditControl = this.editControl;
				return annotationDragEditControl != null && annotationDragEditControl.OnPropertyChanged(propertyName);
			}
			return false;
		}

		// Token: 0x0400114F RID: 4431
		private Rectangle newSquareControl;

		// Token: 0x04001150 RID: 4432
		private FS_POINTF createStartPoint;

		// Token: 0x04001151 RID: 4433
		private FS_POINTF createEndPoint;

		// Token: 0x04001152 RID: 4434
		public Point startPoint;

		// Token: 0x04001153 RID: 4435
		private AnnotationDragEditControl editControl;
	}
}
