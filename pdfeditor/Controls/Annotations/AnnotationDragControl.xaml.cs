using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029A RID: 666
	public partial class AnnotationDragControl : UserControl, IAnnotationControl<PdfInkAnnotation>, IAnnotationControl
	{
		// Token: 0x06002659 RID: 9817 RVA: 0x000B3591 File Offset: 0x000B1791
		public AnnotationDragControl(PdfInkAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.Annotation = annot;
			this.Holder = holder;
			this.pc = new List<Point>();
			this.array = new List<List<Point>>();
			this.offsetPoints = new List<PointCollection>();
		}

		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x0600265A RID: 9818 RVA: 0x000B35CE File Offset: 0x000B17CE
		public PdfInkAnnotation Annotation { get; }

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x0600265B RID: 9819 RVA: 0x000B35D6 File Offset: 0x000B17D6
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x0600265C RID: 9820 RVA: 0x000B35DE File Offset: 0x000B17DE
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x0600265D RID: 9821 RVA: 0x000B35EB File Offset: 0x000B17EB
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x0600265E RID: 9822 RVA: 0x000B35F4 File Offset: 0x000B17F4
		public void OnPageClientBoundsChanged()
		{
			if (this.Annotation != null && this.Annotation.InkList != null && this.Annotation.InkList.Count > 0)
			{
				double num = double.MaxValue;
				double num2 = double.MinValue;
				double num3 = double.MinValue;
				double num4 = double.MaxValue;
				this.pc.Clear();
				this.offsetPoints.Clear();
				this.array.Clear();
				this.Annotation.Opacity = 1f;
				PdfInkPointCollection inkList = this.Annotation.InkList;
				if (inkList != null && inkList.Count > 0)
				{
					for (int i = 0; i < inkList.Count; i++)
					{
						List<Point> list = new List<Point>();
						foreach (FS_POINTF fs_POINTF in inkList[i])
						{
							num = Math.Min((double)fs_POINTF.X, num);
							num2 = Math.Max((double)fs_POINTF.X, num2);
							num3 = Math.Max((double)fs_POINTF.Y, num3);
							num4 = Math.Min((double)fs_POINTF.Y, num4);
							Point point;
							if (this.ParentCanvas.PdfViewer.TryGetClientPoint(this.Annotation.Page.PageIndex, new Point((double)fs_POINTF.X, (double)fs_POINTF.Y), out point))
							{
								list.Add(point);
							}
						}
						this.array.Add(list);
					}
				}
				Rect rect;
				if (num2 >= num && num3 >= num4 && this.ParentCanvas.PdfViewer.TryGetClientRect(this.Annotation.Page.PageIndex, new FS_RECTF(num, num3, num2, num4), out rect))
				{
					this.AnnotationDrag.Width = rect.Width + 6.0;
					this.AnnotationDrag.Height = rect.Height + 12.0;
					this.LayoutRoot.Width = this.AnnotationDrag.Width;
					this.LayoutRoot.Height = this.AnnotationDrag.Height;
					Canvas.SetLeft(this, rect.Left - 3.0);
					Canvas.SetTop(this, rect.Top - 6.0);
					this.DragResizeView.Width = rect.Width + 6.0;
					this.DragResizeView.Height = rect.Height + 6.0;
					double left = Canvas.GetLeft(this);
					double top = Canvas.GetTop(this);
					for (int j = 0; j < this.array.Count; j++)
					{
						PointCollection pointCollection = new PointCollection();
						this.offsetPoints.Add(pointCollection);
						foreach (Point point2 in this.array[j])
						{
							Point point3 = new Point(point2.X - left, Math.Abs(point2.Y - top));
							this.offsetPoints[j].Add(point3);
						}
						Polyline polyline = new Polyline
						{
							Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1892ff")),
							StrokeThickness = 1.0,
							Cursor = Cursors.SizeAll,
							SnapsToDevicePixels = false
						};
						if (this.offsetPoints[j].Count > 0)
						{
							polyline.Points = this.offsetPoints[j];
						}
						this.AnotationDataCanvas.Children.Add(polyline);
					}
				}
			}
		}

		// Token: 0x0600265F RID: 9823 RVA: 0x000B39EC File Offset: 0x000B1BEC
		private void ResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			this.startPoint = new FS_POINTF(this.Annotation.GetRECT().left, this.Annotation.GetRECT().top);
		}

		// Token: 0x06002660 RID: 9824 RVA: 0x000B3A1C File Offset: 0x000B1C1C
		private void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			double num = Canvas.GetLeft(this);
			double num2 = Canvas.GetTop(this);
			num += e.OffsetX;
			num2 += e.OffsetY;
			Canvas.SetLeft(this, num);
			Canvas.SetTop(this, num2);
			PdfViewer pdfViewer = this.ParentCanvas.PdfViewer;
			PdfInkAnnotation annotation = this.Annotation;
			PdfPage pdfPage = ((annotation != null) ? annotation.Page : null);
			if (pdfViewer == null || pdfPage == null || this.Annotation.InkList == null || this.Annotation.InkList.Count == 0)
			{
				return;
			}
			using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
			{
				PdfInkPointCollection pdfInkPointCollection = new PdfInkPointCollection();
				foreach (PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection in this.Annotation.InkList)
				{
					if (pdfLinePointCollection != null)
					{
						this.startPoint = default(FS_POINTF);
						PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection2 = new PdfLinePointCollection<PdfInkAnnotation>();
						for (int i = 0; i < pdfLinePointCollection.Count; i++)
						{
							Point point;
							if (pdfViewer.TryGetClientPoint(pdfPage.PageIndex, pdfLinePointCollection[i].ToPoint(), out point))
							{
								point.X += e.OffsetX;
								point.Y += e.OffsetY;
								Point point2;
								if (pdfViewer.TryGetPagePoint(pdfPage.PageIndex, point, out point2))
								{
									pdfLinePointCollection2.Add(point2.ToPdfPoint());
								}
							}
						}
						pdfInkPointCollection.Add(pdfLinePointCollection2);
					}
				}
				this.Annotation.InkList.Clear();
				this.Annotation.InkList = pdfInkPointCollection;
			}
			if (this.Annotation.Opacity == 0f)
			{
				this.Annotation.Opacity = 0.01f;
			}
			this.Annotation.TryRedrawAnnotation(false);
			this.OnPageClientBoundsChanged();
			if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, pdfPage.Height, pdfPage.Width, 0f)))
			{
				this.Holder.Cancel();
			}
		}

		// Token: 0x06002661 RID: 9825 RVA: 0x000B3C7C File Offset: 0x000B1E7C
		public bool OnPropertyChanged(string propertyName)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return false;
			}
			if (propertyName == "InkStroke" || propertyName == "InkWidth")
			{
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
				{
					if (propertyName == "InkWidth")
					{
						if (this.Annotation.LineStyle == null)
						{
							this.Annotation.LineStyle = new PdfBorderStyle();
						}
						this.Annotation.LineStyle.Width = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkWidth;
					}
					if (propertyName == "InkStroke")
					{
						FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.InkStroke)).ToPdfColor();
						this.Annotation.Color = fs_COLOR;
					}
					this.Annotation.RegenerateAppearances();
				}
				this.Annotation.TryRedrawAnnotation(false);
				return true;
			}
			return false;
		}

		// Token: 0x0400107E RID: 4222
		private FS_POINTF startPoint;

		// Token: 0x0400107F RID: 4223
		private List<Point> pc;

		// Token: 0x04001080 RID: 4224
		private List<List<Point>> array;

		// Token: 0x04001081 RID: 4225
		private List<PointCollection> offsetPoints;
	}
}
