using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Patagames.Pdf;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029C RID: 668
	public class AnnotationFocusControl : Control
	{
		// Token: 0x06002679 RID: 9849 RVA: 0x000B48F4 File Offset: 0x000B2AF4
		public AnnotationFocusControl(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			base.IsHitTestVisible = false;
			base.Foreground = new SolidColorBrush(AnnotationFocusControl.DefaultStrokeColor);
		}

		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x0600267A RID: 9850 RVA: 0x000B4928 File Offset: 0x000B2B28
		// (set) Token: 0x0600267B RID: 9851 RVA: 0x000B493A File Offset: 0x000B2B3A
		public PdfAnnotation Annotation
		{
			get
			{
				return (PdfAnnotation)base.GetValue(AnnotationFocusControl.AnnotationProperty);
			}
			set
			{
				base.SetValue(AnnotationFocusControl.AnnotationProperty, value);
			}
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x000B4948 File Offset: 0x000B2B48
		private static void OnAnnotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				AnnotationFocusControl annotationFocusControl = d as AnnotationFocusControl;
				if (annotationFocusControl != null)
				{
					annotationFocusControl.InvalidateMeasure();
				}
			}
		}

		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x0600267D RID: 9853 RVA: 0x000B4975 File Offset: 0x000B2B75
		// (set) Token: 0x0600267E RID: 9854 RVA: 0x000B4987 File Offset: 0x000B2B87
		public bool IsTextMarkupFocusVisible
		{
			get
			{
				return (bool)base.GetValue(AnnotationFocusControl.IsTextMarkupFocusVisibleProperty);
			}
			set
			{
				base.SetValue(AnnotationFocusControl.IsTextMarkupFocusVisibleProperty, value);
			}
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x000B499C File Offset: 0x000B2B9C
		protected override Size MeasureOverride(Size constraint)
		{
			this.bounds = this.GetAnnotationClientBounds(this.Annotation);
			if (this.bounds.IsEmpty)
			{
				this.bounds = new Rect(0.0, 0.0, 0.0, 0.0);
			}
			Canvas.SetLeft(this, this.bounds.Left);
			Canvas.SetTop(this, this.bounds.Top);
			return new Size(this.bounds.Width, this.bounds.Height);
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x000B4A34 File Offset: 0x000B2C34
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			if (this.bounds.IsEmpty)
			{
				this.bounds = new Rect(0.0, 0.0, 0.0, 0.0);
			}
			return base.ArrangeOverride(arrangeBounds);
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x000B4A84 File Offset: 0x000B2C84
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			PdfAnnotation annotation = this.Annotation;
			Rect rect = this.bounds;
			if (rect.IsEmpty || rect.Width == 0.0 || rect.Height == 0.0)
			{
				return;
			}
			Pen pen = this.GetPen();
			PdfTextMarkupAnnotation pdfTextMarkupAnnotation = annotation as PdfTextMarkupAnnotation;
			if (pdfTextMarkupAnnotation != null)
			{
				if (!this.IsTextMarkupFocusVisible || pdfTextMarkupAnnotation.QuadPoints == null || pdfTextMarkupAnnotation.QuadPoints.Count <= 0)
				{
					return;
				}
				SolidColorBrush solidColorBrush = pen.Brush as SolidColorBrush;
				if (solidColorBrush != null)
				{
					pen = new Pen(new SolidColorBrush(solidColorBrush.Color)
					{
						Opacity = 0.6
					}, 1.0);
					pen.Freeze();
				}
				using (IEnumerator<FS_QUADPOINTSF> enumerator = pdfTextMarkupAnnotation.QuadPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FS_QUADPOINTSF fs_QUADPOINTSF = enumerator.Current;
						FS_RECTF fs_RECTF = fs_QUADPOINTSF.ToPdfRect();
						Rect rect2;
						if (this.annotationCanvas.PdfViewer.TryGetClientRect(pdfTextMarkupAnnotation.Page.PageIndex, fs_RECTF, out rect2))
						{
							drawingContext.DrawRectangle(null, pen, new Rect(rect2.Left - rect.Left - 2.0, rect2.Top - rect.Top - 2.0, rect2.Width + 4.0, rect2.Height + 4.0));
						}
					}
					return;
				}
			}
			PdfLineAnnotation pdfLineAnnotation = annotation as PdfLineAnnotation;
			if (pdfLineAnnotation != null && pdfLineAnnotation.Line != null && pdfLineAnnotation.Line.Count > 1)
			{
				IReadOnlyList<FS_POINTF> line = pdfLineAnnotation.GetLine();
				if (line.Count >= 2)
				{
					FS_POINTF fs_POINTF = line[0];
					FS_POINTF fs_POINTF2 = line[1];
					PdfBorderStyle lineStyle = pdfLineAnnotation.LineStyle;
					float num = ((lineStyle != null) ? lineStyle.Width : 1f);
					double num2 = 1.0;
					Point point;
					Point point2;
					if (this.annotationCanvas.PdfViewer.TryGetClientPoint(pdfLineAnnotation.Page.PageIndex, new Point((double)num, 0.0), out point) && this.annotationCanvas.PdfViewer.TryGetClientPoint(pdfLineAnnotation.Page.PageIndex, new Point(0.0, 0.0), out point2))
					{
						num2 = point.X - point2.X;
					}
					Point point3;
					Point point4;
					if (this.annotationCanvas.PdfViewer.TryGetClientPoint(pdfLineAnnotation.Page.PageIndex, fs_POINTF.ToPoint(), out point3) && this.annotationCanvas.PdfViewer.TryGetClientPoint(pdfLineAnnotation.Page.PageIndex, fs_POINTF2.ToPoint(), out point4))
					{
						double num3 = Math.Sqrt((point4.X - point3.X) * (point4.X - point3.X) + (point4.Y - point3.Y) * (point4.Y - point3.Y));
						double num4 = Math.Atan2(point4.Y - point3.Y, point4.X - point3.X);
						Rect rect3;
						if (AnnotationFocusControl.TryCreateRect(-num2 - 2.0, -num2 / 2.0 - 4.0, num3 + num2 * 2.0 + 4.0, num2 + 8.0, out rect3))
						{
							point3 = new Point(point3.X - rect.Left, point3.Y - rect.Top);
							point4 = new Point(point4.X - rect.Left, point4.Y - rect.Top);
							double num5 = Math.Min(num3 / 2.0 + num2 + 2.0, num2 / 2.0 + 4.0);
							RectangleGeometry rectangleGeometry = new RectangleGeometry(rect3, num5, num5);
							Matrix identity = Matrix.Identity;
							identity.Rotate(num4 * 180.0 / 3.1415926535897931);
							identity.Translate(point3.X, point3.Y);
							rectangleGeometry.Transform = new MatrixTransform(identity);
							drawingContext.DrawGeometry(null, pen, rectangleGeometry);
							return;
						}
					}
				}
			}
			else
			{
				if (annotation is PdfMarkupAnnotation)
				{
					drawingContext.DrawRoundedRectangle(null, pen, new Rect(0.0, 0.0, rect.Width, rect.Height), 2.0, 2.0);
					return;
				}
				if (annotation.IsDigitalSignatureAnnotation())
				{
					drawingContext.DrawRoundedRectangle(null, pen, new Rect(0.0, 0.0, rect.Width, rect.Height), 2.0, 2.0);
				}
			}
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x000B4F98 File Offset: 0x000B3198
		private Rect GetAnnotationClientBounds(PdfAnnotation annot)
		{
			Rect rect = default(Rect);
			if (annot != null && this.annotationCanvas.PdfViewer != null && this.annotationCanvas.PdfViewer.Document != null && annot.Page != null && annot.Page.PageIndex != -1)
			{
				PdfTextMarkupAnnotation pdfTextMarkupAnnotation = annot as PdfTextMarkupAnnotation;
				if (pdfTextMarkupAnnotation != null)
				{
					if (pdfTextMarkupAnnotation.QuadPoints != null && pdfTextMarkupAnnotation.QuadPoints.Count > 0)
					{
						double num = double.MaxValue;
						double num2 = double.MinValue;
						double num3 = double.MinValue;
						double num4 = double.MaxValue;
						foreach (FS_QUADPOINTSF fs_QUADPOINTSF in pdfTextMarkupAnnotation.QuadPoints)
						{
							FS_RECTF fs_RECTF = fs_QUADPOINTSF.ToPdfRect();
							num = Math.Min((double)fs_RECTF.left, num);
							num2 = Math.Max((double)fs_RECTF.right, num2);
							num3 = Math.Max((double)fs_RECTF.top, num3);
							num4 = Math.Min((double)fs_RECTF.bottom, num4);
						}
						Rect rect2;
						if (num2 > num && num3 > num4 && this.annotationCanvas.PdfViewer.TryGetClientRect(pdfTextMarkupAnnotation.Page.PageIndex, new FS_RECTF(num, num3, num2, num4), out rect2))
						{
							rect = new Rect(rect2.Left - 2.0, rect2.Top - 2.0, rect2.Width + 4.0, rect2.Height + 4.0);
						}
					}
				}
				else
				{
					PdfLineAnnotation pdfLineAnnotation = annot as PdfLineAnnotation;
					if (pdfLineAnnotation != null)
					{
						Rect deviceBounds = pdfLineAnnotation.GetDeviceBounds();
						rect = new Rect(deviceBounds.Left - 10.0, deviceBounds.Top - 10.0, deviceBounds.Width + 20.0, deviceBounds.Height + 20.0);
					}
					else
					{
						PdfInkAnnotation pdfInkAnnotation = annot as PdfInkAnnotation;
						if (pdfInkAnnotation != null)
						{
							if (pdfInkAnnotation.InkList.Count > 0)
							{
								double num5 = double.MaxValue;
								double num6 = double.MinValue;
								double num7 = double.MinValue;
								double num8 = double.MaxValue;
								foreach (PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection in pdfInkAnnotation.InkList)
								{
									foreach (FS_POINTF fs_POINTF in pdfLinePointCollection)
									{
										num5 = Math.Min((double)fs_POINTF.X, num5);
										num6 = Math.Max((double)fs_POINTF.X, num6);
										num7 = Math.Max((double)fs_POINTF.Y, num7);
										num8 = Math.Min((double)fs_POINTF.Y, num8);
									}
								}
								Rect rect3;
								if (num6 > num5 && num7 > num8 && this.annotationCanvas.PdfViewer.TryGetClientRect(pdfInkAnnotation.Page.PageIndex, new FS_RECTF(num5, num7, num6, num8), out rect3))
								{
									rect = new Rect(rect3.Left - 3.0, rect3.Top - 3.0, rect3.Width + 6.0, rect3.Height + 6.0);
								}
							}
						}
						else
						{
							PdfMarkupAnnotation pdfMarkupAnnotation = annot as PdfMarkupAnnotation;
							if (pdfMarkupAnnotation != null)
							{
								rect = pdfMarkupAnnotation.GetDeviceBounds();
							}
							else if (annot.IsDigitalSignatureAnnotation())
							{
								rect = annot.GetDeviceBounds();
							}
						}
					}
				}
			}
			if (rect.IsEmpty)
			{
				rect = new Rect(0.0, 0.0, 0.0, 0.0);
			}
			return rect;
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x000B53A4 File Offset: 0x000B35A4
		private Pen GetPen()
		{
			bool foreground = base.Foreground != null;
			if (this.pen == null)
			{
				this.pen = new Pen();
				this.pen.Thickness = 1.0;
			}
			if (!foreground)
			{
				if (!(this.pen.Brush is SolidColorBrush))
				{
					this.pen.Brush = new SolidColorBrush(AnnotationFocusControl.DefaultStrokeColor);
				}
			}
			else if (this.pen.Brush != base.Foreground)
			{
				this.pen.Brush = base.Foreground;
			}
			return this.pen;
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x000B5433 File Offset: 0x000B3633
		private static bool TryCreateRect(double x, double y, double width, double height, out Rect rect)
		{
			rect = Rect.Empty;
			if (width < 0.0 || height < 0.0)
			{
				return false;
			}
			rect = new Rect(x, y, width, height);
			return true;
		}

		// Token: 0x04001093 RID: 4243
		private static Color DefaultStrokeColor = Color.FromArgb(204, 0, 0, 0);

		// Token: 0x04001094 RID: 4244
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04001095 RID: 4245
		private Rect bounds;

		// Token: 0x04001096 RID: 4246
		private Pen pen;

		// Token: 0x04001097 RID: 4247
		public static readonly DependencyProperty AnnotationProperty = DependencyProperty.Register("Annotation", typeof(PdfAnnotation), typeof(AnnotationFocusControl), new PropertyMetadata(null, new PropertyChangedCallback(AnnotationFocusControl.OnAnnotationPropertyChanged)));

		// Token: 0x04001098 RID: 4248
		public static readonly DependencyProperty IsTextMarkupFocusVisibleProperty = DependencyProperty.Register("IsTextMarkupFocusVisible", typeof(bool), typeof(AnnotationFocusControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
	}
}
