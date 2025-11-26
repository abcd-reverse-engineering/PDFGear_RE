using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029E RID: 670
	public partial class AnnotationLineControl : UserControl, IAnnotationControl<PdfLineAnnotation>, IAnnotationControl
	{
		// Token: 0x060026AA RID: 9898 RVA: 0x000B67C8 File Offset: 0x000B49C8
		public AnnotationLineControl(PdfLineAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.Annotation = annot;
			this.Holder = holder;
			global::System.Collections.Generic.IReadOnlyList<FS_POINTF> line = annot.GetLine();
			if (line.Count >= 2)
			{
				this.point1 = line[0];
				this.point2 = line[1];
			}
			this.color = Color.FromArgb((byte)annot.Color.A, (byte)annot.Color.R, (byte)annot.Color.G, (byte)annot.Color.B);
			PdfBorderStyle lineStyle = annot.LineStyle;
			this.rawThickness = ((lineStyle != null) ? lineStyle.Width : 1f);
			this.ContentLine.DataContext = this;
			base.Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 24, 146, byte.MaxValue));
			this.pointCursor = CursorHelper.CreateCursor(global::System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style\\\\Resources\\\\pointmove.png"), 0U, 0U);
			this.Point1Rect.Cursor = this.pointCursor;
			this.Point2Rect.Cursor = this.pointCursor;
		}

		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x060026AB RID: 9899 RVA: 0x000B68EC File Offset: 0x000B4AEC
		public PdfLineAnnotation Annotation { get; }

		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x060026AC RID: 9900 RVA: 0x000B68F4 File Offset: 0x000B4AF4
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x060026AD RID: 9901 RVA: 0x000B6901 File Offset: 0x000B4B01
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x060026AE RID: 9902 RVA: 0x000B6909 File Offset: 0x000B4B09
		public IAnnotationHolder Holder { get; }

		// Token: 0x060026AF RID: 9903 RVA: 0x000B6914 File Offset: 0x000B4B14
		public void OnPageClientBoundsChanged()
		{
			Point? point = this.PageToDevice((double)this.point1.X, (double)this.point1.Y);
			Point? point2 = this.PageToDevice((double)this.point2.X, (double)this.point2.Y);
			if (point == null || point2 == null)
			{
				return;
			}
			Point value = point.Value;
			Point value2 = point2.Value;
			Point point3 = new Point(Math.Min(value.X, value2.X), Math.Min(value.Y, value2.Y));
			Point point4 = new Point(value.X - point3.X, value.Y - point3.Y);
			Point point5 = new Point(value2.X - point3.X, value2.Y - point3.Y);
			float num = this.rawThickness;
			float zoom = this.ParentCanvas.PdfViewer.Zoom;
			this.ContentLine.X1 = point4.X;
			this.ContentLine.Y1 = point4.Y;
			this.ContentLine.X2 = point5.X;
			this.ContentLine.Y2 = point5.Y;
			this.DraggerLine.X1 = point4.X;
			this.DraggerLine.Y1 = point4.Y;
			this.DraggerLine.X2 = point5.X;
			this.DraggerLine.Y2 = point5.Y;
			this.ContentLine.StrokeThickness = 1.0;
			Canvas.SetLeft(this.Point1Rect, point4.X - 4.0);
			Canvas.SetTop(this.Point1Rect, point4.Y - 4.0);
			Canvas.SetLeft(this.Point2Rect, point5.X - 4.0);
			Canvas.SetTop(this.Point2Rect, point5.Y - 4.0);
			this.LayoutRoot.Width = Math.Abs(point4.X - point5.X);
			this.LayoutRoot.Height = Math.Abs(point4.Y - point5.Y);
			Canvas.SetLeft(this, point3.X);
			Canvas.SetTop(this, point3.Y);
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x000B6B80 File Offset: 0x000B4D80
		private void Point1Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.Point1Rect.CaptureMouse())
			{
				return;
			}
			this.isPressed = true;
			Window.GetWindow(this).MouseMove += this.Point1Rect_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp += this.Point1Rect_MouseLeftButtonUp;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point1 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x000B6C20 File Offset: 0x000B4E20
		private void Point1Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			Window.GetWindow(this).MouseMove -= this.Point1Rect_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp -= this.Point1Rect_MouseLeftButtonUp;
			this.Point1Rect.ReleaseMouseCapture();
			this.isPressed = false;
			this.ContentLine.Opacity = 0.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point1 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
			using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
			{
				this.Annotation.Line[0] = this.point1;
			}
			this.Annotation.TryRedrawAnnotation(false);
			PdfPage page = this.Annotation.Page;
			if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, page.Height, page.Width, 0f)))
			{
				this.Holder.Cancel();
				return;
			}
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x000B6D84 File Offset: 0x000B4F84
		private void Point1Rect_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.isPressed)
			{
				return;
			}
			this.ContentLine.Opacity = 1.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point1 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x000B6DFC File Offset: 0x000B4FFC
		private void Point2Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.Point2Rect.CaptureMouse())
			{
				return;
			}
			this.isPressed = true;
			Window.GetWindow(this).MouseMove += this.Point2Rect_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp += this.Point2Rect_MouseLeftButtonUp;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point2 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026B4 RID: 9908 RVA: 0x000B6E9C File Offset: 0x000B509C
		private void Point2Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			Window.GetWindow(this).MouseMove -= this.Point2Rect_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp -= this.Point2Rect_MouseLeftButtonUp;
			this.Point2Rect.ReleaseMouseCapture();
			this.isPressed = false;
			this.ContentLine.Opacity = 0.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point2 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
			using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
			{
				this.Annotation.Line[1] = this.point2;
			}
			this.Annotation.TryRedrawAnnotation(false);
			PdfPage page = this.Annotation.Page;
			if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, page.Height, page.Width, 0f)))
			{
				this.Holder.Cancel();
				return;
			}
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x000B7000 File Offset: 0x000B5200
		private void Point2Rect_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.isPressed)
			{
				return;
			}
			this.ContentLine.Opacity = 1.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.point2 = new FS_POINTF(point.Value.X, point.Value.Y);
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x000B7078 File Offset: 0x000B5278
		private void DraggerLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.DraggerLine.CaptureMouse())
			{
				return;
			}
			this.isPressed = true;
			Window.GetWindow(this).MouseMove += this.DraggerLine_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp += this.DraggerLine_MouseLeftButtonUp;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			this.startPoint = new FS_POINTF(point.Value.X, point.Value.Y);
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x000B7110 File Offset: 0x000B5310
		private void DraggerLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			Window.GetWindow(this).MouseMove -= this.DraggerLine_MouseMove;
			Window.GetWindow(this).MouseLeftButtonUp -= this.DraggerLine_MouseLeftButtonUp;
			this.DraggerLine.ReleaseMouseCapture();
			this.isPressed = false;
			this.ContentLine.Opacity = 0.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			FS_POINTF fs_POINTF = new FS_POINTF(point.Value.X, point.Value.Y);
			float num = fs_POINTF.X - this.startPoint.X;
			float num2 = fs_POINTF.Y - this.startPoint.Y;
			this.startPoint = default(FS_POINTF);
			this.point1.X = this.point1.X + num;
			this.point1.Y = this.point1.Y + num2;
			this.point2.X = this.point2.X + num;
			this.point2.Y = this.point2.Y + num2;
			this.OnPageClientBoundsChanged();
			using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
			{
				this.Annotation.Line[0] = this.point1;
				this.Annotation.Line[1] = this.point2;
			}
			this.Annotation.TryRedrawAnnotation(false);
			PdfPage page = this.Annotation.Page;
			if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, page.Height, page.Width, 0f)))
			{
				this.Holder.Cancel();
			}
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x000B7310 File Offset: 0x000B5510
		private void DraggerLine_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.isPressed)
			{
				return;
			}
			this.ContentLine.Opacity = 1.0;
			Point position = e.GetPosition(this.ParentCanvas);
			Point? point = this.DeviceToPage(position);
			if (point == null)
			{
				return;
			}
			FS_POINTF fs_POINTF = new FS_POINTF(point.Value.X, point.Value.Y);
			float num = fs_POINTF.X - this.startPoint.X;
			float num2 = fs_POINTF.Y - this.startPoint.Y;
			this.startPoint = fs_POINTF;
			this.point1.X = this.point1.X + num;
			this.point1.Y = this.point1.Y + num2;
			this.point2.X = this.point2.X + num;
			this.point2.Y = this.point2.Y + num2;
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x000B7404 File Offset: 0x000B5604
		public bool OnPropertyChanged(string propertyName)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return false;
			}
			if (propertyName == "LineStroke" || propertyName == "LineWidth")
			{
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
				{
					if (propertyName == "LineWidth")
					{
						if (this.Annotation.LineStyle == null)
						{
							this.Annotation.LineStyle = new PdfBorderStyle();
						}
						this.Annotation.LineStyle.Width = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineWidth;
					}
					if (propertyName == "LineStroke")
					{
						FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.LineStroke)).ToPdfColor();
						this.Annotation.Color = fs_COLOR;
					}
					this.Annotation.RegenerateAppearances();
				}
				this.Annotation.TryRedrawAnnotation(false);
				return true;
			}
			return false;
		}

		// Token: 0x040010B5 RID: 4277
		private FS_POINTF point1;

		// Token: 0x040010B6 RID: 4278
		private FS_POINTF point2;

		// Token: 0x040010B7 RID: 4279
		private Color color;

		// Token: 0x040010B8 RID: 4280
		private float rawThickness;

		// Token: 0x040010B9 RID: 4281
		private Cursor pointCursor;

		// Token: 0x040010BC RID: 4284
		private bool isPressed;

		// Token: 0x040010BD RID: 4285
		private FS_POINTF startPoint;
	}
}
