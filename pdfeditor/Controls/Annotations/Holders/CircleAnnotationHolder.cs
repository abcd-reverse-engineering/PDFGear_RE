using System;
using System.Collections.Generic;
using System.Linq;
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
	// Token: 0x020002B3 RID: 691
	public class CircleAnnotationHolder : BaseAnnotationHolder<PdfFigureAnnotation>
	{
		// Token: 0x06002802 RID: 10242 RVA: 0x000BB6A8 File Offset: 0x000B98A8
		public CircleAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
			this.circleFsPoints = new List<FS_POINTF>();
		}

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x06002803 RID: 10243 RVA: 0x000BB6BC File Offset: 0x000B98BC
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x000BB6C0 File Offset: 0x000B98C0
		public override void OnPageClientBoundsChanged()
		{
			if (base.State == AnnotationHolderState.CreatingNew)
			{
				if (this.newCircleControl == null)
				{
					throw new ArgumentException("newCircleControl");
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

		// Token: 0x06002805 RID: 10245 RVA: 0x000BB718 File Offset: 0x000B9918
		protected override void OnCancel()
		{
			if (this.editControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.editControl);
				this.editControl = null;
			}
			if (this.newCircleControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.newCircleControl);
				this.newCircleControl = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x000BB788 File Offset: 0x000B9988
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<PdfFigureAnnotation>> OnCompleteCreateNewAsync()
		{
			base.AnnotationCanvas.Children.Remove(this.newCircleControl);
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfPage page = base.CurrentPage;
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			global::System.Collections.Generic.IReadOnlyList<PdfFigureAnnotation> readOnlyList;
			if (Math.Abs(this.createStartPoint.X - this.createEndPoint.X) > 10f || Math.Abs(this.createStartPoint.Y - this.createEndPoint.Y) > 10f)
			{
				PdfFigureAnnotation circleAnnot = new PdfCircleAnnotation(page);
				circleAnnot.Subject = "Ellipse";
				List<FS_POINTF> list = this.circleFsPoints.Distinct<FS_POINTF>().ToList<FS_POINTF>();
				int num = list.Count<FS_POINTF>();
				if (list.Count > 0)
				{
					FS_POINTF fs_POINTF = new FS_POINTF(list[0].X, list[0].Y);
					FS_POINTF fs_POINTF2 = new FS_POINTF(list[num - 1].X, list[num - 1].Y);
					new FS_POINTF(list[0].X, list[num - 1].Y);
					new FS_POINTF(list[num - 1].X, list[0].Y);
					circleAnnot.Text = AnnotationAuthorUtil.GetAuthorName();
					Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseStroke);
					FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
					Color color2 = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseFill);
					FS_COLOR fs_COLOR2 = new FS_COLOR((int)color2.A, (int)color2.R, (int)color2.G, (int)color2.B);
					circleAnnot.InteriorColor = fs_COLOR2;
					circleAnnot.Color = fs_COLOR;
					circleAnnot.Opacity = 1f;
					if (fs_POINTF.X < fs_POINTF2.X)
					{
						if (fs_POINTF.Y > fs_POINTF2.Y)
						{
							circleAnnot.Rectangle = new FS_RECTF(fs_POINTF.X, fs_POINTF.Y, fs_POINTF2.X, fs_POINTF2.Y);
						}
						else
						{
							circleAnnot.Rectangle = new FS_RECTF(fs_POINTF.X, fs_POINTF2.Y, fs_POINTF2.X, fs_POINTF.Y);
						}
					}
					else if (fs_POINTF.Y < fs_POINTF2.Y)
					{
						circleAnnot.Rectangle = new FS_RECTF(fs_POINTF2.X, fs_POINTF2.Y, fs_POINTF.X, fs_POINTF.Y);
					}
					else
					{
						circleAnnot.Rectangle = new FS_RECTF(fs_POINTF2.X, fs_POINTF.Y, fs_POINTF.X, fs_POINTF2.Y);
					}
					circleAnnot.BorderStyle = new PdfBorderStyle();
					circleAnnot.BorderStyle.Style = BorderStyles.Solid;
					circleAnnot.BorderStyle.Width = requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseThickness;
					circleAnnot.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					circleAnnot.CreationDate = circleAnnot.ModificationDate;
					circleAnnot.Flags |= AnnotationFlags.Print;
					page.Annots.Add(circleAnnot);
					await requiredService.OperationManager.TraceAnnotationInsertAsync(circleAnnot, "");
					await page.TryRedrawPageAsync(default(CancellationToken));
				}
				this.createStartPoint = default(FS_POINTF);
				this.createEndPoint = default(FS_POINTF);
				this.circleFsPoints.Clear();
				if (circleAnnot != null)
				{
					readOnlyList = new PdfFigureAnnotation[] { circleAnnot };
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

		// Token: 0x06002807 RID: 10247 RVA: 0x000BB7CC File Offset: 0x000B99CC
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
				this.circleFsPoints.Add(pagePoint);
				double num = Math.Min(point.X, this.startPoint.X);
				double num2 = Math.Min(point.Y, this.startPoint.Y);
				double num3 = Math.Max(point.X, this.startPoint.X) - num;
				double num4 = Math.Max(point.Y, this.startPoint.Y) - num2;
				if (this.newCircleControl != null)
				{
					this.newCircleControl.Width = num3;
					this.newCircleControl.Height = num4;
					Canvas.SetLeft(this.newCircleControl, num);
					Canvas.SetTop(this.newCircleControl, num2);
				}
			}
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x000BB8B7 File Offset: 0x000B9AB7
		protected override bool OnSelecting(PdfFigureAnnotation annotation, bool afterCreate)
		{
			this.editControl = new AnnotationDragEditControl(annotation, this);
			base.AnnotationCanvas.Children.Add(this.editControl);
			return true;
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x000BB8E0 File Offset: 0x000B9AE0
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			Point point;
			if (base.AnnotationCanvas.PdfViewer.TryGetClientPoint(page.PageIndex, pagePoint.ToPoint(), out point))
			{
				this.startPoint = point;
				this.circleFsPoints.Clear();
				MainViewModel mainViewModel = base.AnnotationCanvas.DataContext as MainViewModel;
				Color color = (Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseStroke);
				this.newCircleControl = new Ellipse
				{
					Stroke = new SolidColorBrush(color),
					StrokeThickness = (double)mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseThickness
				};
				Canvas.SetLeft(this.newCircleControl, this.startPoint.X);
				Canvas.SetTop(this.newCircleControl, this.startPoint.Y);
				base.AnnotationCanvas.Children.Add(this.newCircleControl);
				return true;
			}
			return false;
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x000BB9D0 File Offset: 0x000B9BD0
		public override bool OnPropertyChanged(string propertyName)
		{
			if (propertyName == "EllipseFill" || propertyName == "EllipseStroke" || propertyName == "EllipseThickness")
			{
				AnnotationDragEditControl annotationDragEditControl = this.editControl;
				return annotationDragEditControl != null && annotationDragEditControl.OnPropertyChanged(propertyName);
			}
			return false;
		}

		// Token: 0x0400112D RID: 4397
		private Ellipse newCircleControl;

		// Token: 0x0400112E RID: 4398
		private FS_POINTF createStartPoint;

		// Token: 0x0400112F RID: 4399
		private FS_POINTF createEndPoint;

		// Token: 0x04001130 RID: 4400
		public Point startPoint;

		// Token: 0x04001131 RID: 4401
		private List<FS_POINTF> circleFsPoints;

		// Token: 0x04001132 RID: 4402
		private AnnotationDragEditControl editControl;
	}
}
