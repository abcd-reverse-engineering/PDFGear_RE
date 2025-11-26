using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommonLib.Common;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A5 RID: 677
	public class PopupAnnotationCollection : Canvas
	{
		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x060026E6 RID: 9958 RVA: 0x000B7F11 File Offset: 0x000B6111
		private PdfViewer PdfViewer
		{
			get
			{
				return this.annotationCanvas.PdfViewer;
			}
		}

		// Token: 0x060026E7 RID: 9959 RVA: 0x000B7F20 File Offset: 0x000B6120
		public PopupAnnotationCollection(AnnotationCanvas annotationCanvas, PdfPage page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			this.page = page;
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.dict = new ConcurrentDictionary<PdfPopupAnnotation, PopupAnnotationCollection.PopupAnnotationContext>(new PopupAnnotationCollection.PdfWrapperCompare());
			base.Loaded += this.PopupAnnotationCollection_Loaded;
			base.Unloaded += this.PopupAnnotationCollection_Unloaded;
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x000B7F93 File Offset: 0x000B6193
		private void PopupAnnotationCollection_Loaded(object sender, RoutedEventArgs e)
		{
			this.InitPopupHost();
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000B7F9B File Offset: 0x000B619B
		private void PopupAnnotationCollection_Unloaded(object sender, RoutedEventArgs e)
		{
			this.dict.Clear();
			base.InternalChildren.Clear();
			this.annotationRepliesDict = null;
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000B7FBC File Offset: 0x000B61BC
		private void InitPopupHost()
		{
			this.dict.Clear();
			base.InternalChildren.Clear();
			if (this.page.Annots != null)
			{
				this.annotationRepliesDict = CommetUtils.GetMarkupAnnotationReplies(this.page);
				AnnotationCanvas annotationCanvas = this.annotationCanvas;
				PdfTextAnnotation pdfTextAnnotation = ((annotationCanvas != null) ? annotationCanvas.SelectedAnnotation : null) as PdfTextAnnotation;
				foreach (PdfAnnotation pdfAnnotation in this.page.Annots)
				{
					PdfAnnotation pdfAnnotation2 = null;
					PdfPopupAnnotation pdfPopupAnnotation = pdfAnnotation as PdfPopupAnnotation;
					if (pdfPopupAnnotation != null)
					{
						try
						{
							pdfAnnotation2 = pdfPopupAnnotation.Parent;
							goto IL_0081;
						}
						catch
						{
							goto IL_0081;
						}
						goto IL_007F;
					}
					goto IL_007F;
					IL_0081:
					if (!(pdfPopupAnnotation != null) || !(pdfAnnotation2 != null))
					{
						continue;
					}
					PdfMarkupAnnotation pdfMarkupAnnotation = pdfPopupAnnotation.Parent as PdfMarkupAnnotation;
					if (pdfMarkupAnnotation != null && pdfMarkupAnnotation.Relationship != RelationTypes.NonSpecified)
					{
						continue;
					}
					PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext = this.AddAnnotationCore(pdfPopupAnnotation);
					this.UpdatePosition(popupAnnotationContext);
					if (pdfTextAnnotation == pdfPopupAnnotation.Parent)
					{
						this.SetSelected(pdfPopupAnnotation, true);
						continue;
					}
					continue;
					IL_007F:
					pdfPopupAnnotation = null;
					goto IL_0081;
				}
				this.UpdateChildrenZIndex();
			}
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x000B80D4 File Offset: 0x000B62D4
		private PopupAnnotationCollection.PopupAnnotationContext AddAnnotationCore(PdfPopupAnnotation popup)
		{
			if (popup == null)
			{
				throw new ArgumentNullException("popup");
			}
			if (this.dict.ContainsKey(popup))
			{
				return null;
			}
			global::System.Collections.Generic.IReadOnlyList<PdfMarkupAnnotation> readOnlyList;
			if (this.annotationRepliesDict == null || popup.Parent == null || !this.annotationRepliesDict.TryGetValue(popup.Parent, out readOnlyList))
			{
				readOnlyList = null;
			}
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext = new PopupAnnotationCollection.PopupAnnotationContext(this.annotationCanvas, popup, readOnlyList);
			this.dict[popup] = popupAnnotationContext;
			base.InternalChildren.Add(popupAnnotationContext.AnnotationPopupControl);
			base.InternalChildren.Add(popupAnnotationContext.RelationshipLine);
			return popupAnnotationContext;
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x000B816C File Offset: 0x000B636C
		private void RemoveAnnotationCore(PdfPopupAnnotation popup)
		{
			if (popup == null)
			{
				throw new ArgumentNullException("popup");
			}
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (this.dict.TryRemove(popup, out popupAnnotationContext))
			{
				base.InternalChildren.Remove(popupAnnotationContext.AnnotationPopupControl);
				base.InternalChildren.Remove(popupAnnotationContext.RelationshipLine);
			}
		}

		// Token: 0x060026ED RID: 9965 RVA: 0x000B81BC File Offset: 0x000B63BC
		private void UpdateChildrenZIndex()
		{
			if (base.InternalChildren.Count == 0)
			{
				return;
			}
			PdfAnnotationCollection annots = this.page.Annots;
			int num = ((annots != null) ? annots.Count : (this.dict.Count * 2));
			foreach (KeyValuePair<PdfPopupAnnotation, PopupAnnotationCollection.PopupAnnotationContext> keyValuePair in this.dict)
			{
				PdfPopupAnnotation pdfPopupAnnotation;
				PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
				keyValuePair.Deconstruct(out pdfPopupAnnotation, out popupAnnotationContext);
				PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext2 = popupAnnotationContext;
				int num2 = popupAnnotationContext2.PopupAnnotationWrapper.AnnotationIndex;
				if (popupAnnotationContext2.IsParentSelected)
				{
					num2 += num;
				}
				Panel.SetZIndex(popupAnnotationContext2.AnnotationPopupControl, num2 * 2);
				Panel.SetZIndex(popupAnnotationContext2.RelationshipLine, num2 * 2 - 1);
			}
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000B8278 File Offset: 0x000B6478
		private void UpdatePosition(PopupAnnotationCollection.PopupAnnotationContext context)
		{
			if (context == null)
			{
				return;
			}
			DpiScale dpi = VisualTreeHelper.GetDpi(this);
			double num = (double)context.PopupAnnotationWrapper.Rectangle.Width * dpi.PixelsPerDip;
			double num2 = (double)context.PopupAnnotationWrapper.Rectangle.Height * dpi.PixelsPerDip;
			Point point = new Point((double)context.PopupAnnotationWrapper.Rectangle.left, (double)context.PopupAnnotationWrapper.Rectangle.top);
			Point point2;
			if (this.PdfViewer.TryGetClientPoint(context.PopupAnnotationWrapper.Page.PageIndex, point, out point2))
			{
				Canvas.SetLeft(context.AnnotationPopupControl, point2.X);
				Canvas.SetTop(context.AnnotationPopupControl, point2.Y);
			}
			context.AnnotationPopupControl.Width = num;
			context.AnnotationPopupControl.Height = num2;
			context.PopupAnnotationWrapper.NotifyAnnotationChanged();
			this.UpdateLine(context);
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000B8364 File Offset: 0x000B6564
		private void UpdateLine(PopupAnnotationCollection.PopupAnnotationContext context)
		{
			if (this.annotationCanvas.PdfViewer == null)
			{
				return;
			}
			PdfAnnotation parent = context.PopupAnnotationWrapper.Annotation.Parent;
			if (parent == null)
			{
				return;
			}
			Rect deviceBounds = parent.GetDeviceBounds();
			Point point = new Point(deviceBounds.Left + deviceBounds.Width / 2.0, deviceBounds.Top + deviceBounds.Height / 2.0);
			Point point2 = new Point(Canvas.GetLeft(context.AnnotationPopupControl), Canvas.GetTop(context.AnnotationPopupControl));
			context.RelationshipLine.X1 = point.X;
			context.RelationshipLine.Y1 = point.Y;
			context.RelationshipLine.X2 = point2.X;
			context.RelationshipLine.Y2 = point2.Y;
			SolidColorBrush solidColorBrush = context.RelationshipLine.Stroke as SolidColorBrush;
			if (solidColorBrush != null)
			{
				solidColorBrush.Color = context.PopupAnnotationWrapper.BackgroundColor;
			}
		}

		// Token: 0x060026F0 RID: 9968 RVA: 0x000B8468 File Offset: 0x000B6668
		public void AddAnnotation(PdfPopupAnnotation popup)
		{
			this.AddAnnotationCore(popup);
			this.UpdateChildrenZIndex();
			AnnotationCanvas annotationCanvas = this.annotationCanvas;
			PdfTextAnnotation pdfTextAnnotation = ((annotationCanvas != null) ? annotationCanvas.SelectedAnnotation : null) as PdfTextAnnotation;
			if (pdfTextAnnotation != null && pdfTextAnnotation == popup.Parent)
			{
				this.SetSelected(popup, true);
			}
		}

		// Token: 0x060026F1 RID: 9969 RVA: 0x000B84B4 File Offset: 0x000B66B4
		public void RemoveAnnotation(PdfPopupAnnotation popup)
		{
			this.RemoveAnnotationCore(popup);
			this.UpdateChildrenZIndex();
		}

		// Token: 0x060026F2 RID: 9970 RVA: 0x000B84C4 File Offset: 0x000B66C4
		private PdfPopupAnnotation GetPopupAnnotation(PdfAnnotation annotation)
		{
			PdfPopupAnnotation pdfPopupAnnotation = annotation as PdfPopupAnnotation;
			if (pdfPopupAnnotation != null)
			{
				PdfPage pdfPage = pdfPopupAnnotation.Page;
				int? num = ((pdfPage != null) ? new int?(pdfPage.PageIndex) : null);
				int pageIndex = this.page.PageIndex;
				if (!((num.GetValueOrDefault() == pageIndex) & (num != null)))
				{
					throw new ArgumentException("Page");
				}
				PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
				if (!this.dict.TryGetValue(pdfPopupAnnotation, out popupAnnotationContext))
				{
					return null;
				}
				return pdfPopupAnnotation;
			}
			else
			{
				PdfMarkupAnnotation pdfMarkupAnnotation = annotation as PdfMarkupAnnotation;
				if (pdfMarkupAnnotation != null && pdfMarkupAnnotation.Popup != null)
				{
					return this.GetPopupAnnotation(pdfMarkupAnnotation.Popup);
				}
				return null;
			}
		}

		// Token: 0x060026F3 RID: 9971 RVA: 0x000B8568 File Offset: 0x000B6768
		public bool IsPopupVisible(PdfAnnotation annotation)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(annotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			return popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext) && popupAnnotationContext.PopupAnnotationWrapper.IsOpen;
		}

		// Token: 0x060026F4 RID: 9972 RVA: 0x000B85A4 File Offset: 0x000B67A4
		public bool ShowPopup(PdfAnnotation annotation, bool bringIntoView)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(annotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				this.UpdatePosition(popupAnnotationContext);
				object dataContext = this.annotationCanvas.DataContext;
				popupAnnotationContext.PopupAnnotationWrapper.IsOpen = true;
				popupAnnotationContext.UpdateLineVisible();
				if (bringIntoView)
				{
					this.TryBringPopupControlIntoView(popupAnnotation);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060026F5 RID: 9973 RVA: 0x000B8604 File Offset: 0x000B6804
		public void HidePopup(PdfAnnotation annotation)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(annotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				object dataContext = this.annotationCanvas.DataContext;
				popupAnnotationContext.PopupAnnotationWrapper.IsOpen = false;
				popupAnnotationContext.UpdateLineVisible();
			}
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x000B8650 File Offset: 0x000B6850
		public void UpdatePosition()
		{
			foreach (KeyValuePair<PdfPopupAnnotation, PopupAnnotationCollection.PopupAnnotationContext> keyValuePair in this.dict)
			{
				PdfPopupAnnotation pdfPopupAnnotation;
				PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
				keyValuePair.Deconstruct(out pdfPopupAnnotation, out popupAnnotationContext);
				PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext2 = popupAnnotationContext;
				if (popupAnnotationContext2.PopupAnnotationWrapper.IsOpen)
				{
					this.UpdatePosition(popupAnnotationContext2);
				}
			}
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x000B86B4 File Offset: 0x000B68B4
		public void SetHovered(PdfAnnotation pdfAnnotation, bool value)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(pdfAnnotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				if (value)
				{
					this.UpdatePosition(popupAnnotationContext);
				}
				if (pdfAnnotation == popupAnnotation)
				{
					popupAnnotationContext.IsParentHovered = value;
					return;
				}
				if (pdfAnnotation == popupAnnotation.Parent)
				{
					popupAnnotationContext.IsPopupHovered = value;
				}
			}
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000B8714 File Offset: 0x000B6914
		public void SetSelected(PdfAnnotation pdfAnnotation, bool value)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(pdfAnnotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				if (value)
				{
					this.UpdatePosition(popupAnnotationContext);
				}
				popupAnnotationContext.IsParentSelected = value;
				this.UpdateChildrenZIndex();
			}
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x000B875C File Offset: 0x000B695C
		public TextBox GetPopupTextBox(PdfAnnotation pdfAnnotation)
		{
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(pdfAnnotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				AnnotationPopupControl annotationPopupControl = popupAnnotationContext.AnnotationPopupControl;
				FrameworkElement frameworkElement = ((annotationPopupControl != null) ? annotationPopupControl.Content : null) as FrameworkElement;
				if (frameworkElement != null)
				{
					TextBox textBox = frameworkElement.FindName("TextContentBox") as TextBox;
					if (textBox != null)
					{
						return textBox;
					}
				}
			}
			return null;
		}

		// Token: 0x060026FA RID: 9978 RVA: 0x000B87BC File Offset: 0x000B69BC
		public void TryBringPopupControlIntoView(PdfAnnotation pdfAnnotation)
		{
			if (this.annotationCanvas.ActualWidth == 0.0 || this.annotationCanvas.ActualHeight == 0.0)
			{
				return;
			}
			ScrollViewer scrollOwner = this.annotationCanvas.PdfViewer.ScrollOwner;
			if (scrollOwner == null)
			{
				return;
			}
			PdfPopupAnnotation popupAnnotation = this.GetPopupAnnotation(pdfAnnotation);
			PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext;
			if (popupAnnotation != null && this.dict.TryGetValue(popupAnnotation, out popupAnnotationContext))
			{
				AnnotationPopupControl annotationPopupControl = popupAnnotationContext.AnnotationPopupControl;
				if (annotationPopupControl == null || (annotationPopupControl.ActualWidth == 0.0 && annotationPopupControl.ActualHeight == 0.0))
				{
					return;
				}
				Rect rect = annotationPopupControl.TransformToVisual(this.annotationCanvas).TransformBounds(new Rect(0.0, 0.0, annotationPopupControl.ActualWidth + 8.0, annotationPopupControl.ActualHeight));
				if (rect.Left < 0.0 || rect.Right > this.annotationCanvas.ActualWidth)
				{
					double num;
					if (rect.Left < 0.0)
					{
						num = rect.Left;
					}
					else
					{
						num = rect.Right - this.annotationCanvas.ActualWidth;
					}
					double num2 = scrollOwner.HorizontalOffset + num;
					scrollOwner.ScrollToHorizontalOffset(num2);
				}
				if (rect.Top < 0.0 || rect.Bottom > this.annotationCanvas.ActualHeight)
				{
					double num3;
					if (rect.Top < 0.0)
					{
						num3 = rect.Top;
					}
					else
					{
						num3 = rect.Bottom - this.annotationCanvas.ActualHeight;
					}
					double num4 = scrollOwner.VerticalOffset + num3;
					scrollOwner.ScrollToVerticalOffset(num4);
				}
			}
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x000B898C File Offset: 0x000B6B8C
		public void KillFocus()
		{
			foreach (PopupAnnotationCollection.PopupAnnotationContext popupAnnotationContext in this.dict.Values)
			{
				if (popupAnnotationContext.AnnotationPopupControl.IsKeyboardFocusWithin)
				{
					popupAnnotationContext.AnnotationPopupControl.Apply();
				}
			}
		}

		// Token: 0x040010D6 RID: 4310
		private ConcurrentDictionary<PdfPopupAnnotation, PopupAnnotationCollection.PopupAnnotationContext> dict;

		// Token: 0x040010D7 RID: 4311
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x040010D8 RID: 4312
		private readonly PdfPage page;

		// Token: 0x040010D9 RID: 4313
		private IReadOnlyDictionary<PdfAnnotation, global::System.Collections.Generic.IReadOnlyList<PdfMarkupAnnotation>> annotationRepliesDict;

		// Token: 0x02000771 RID: 1905
		private class PdfWrapperCompare : IEqualityComparer<PdfWrapper>
		{
			// Token: 0x06003723 RID: 14115 RVA: 0x00115A32 File Offset: 0x00113C32
			public bool Equals(PdfWrapper x, PdfWrapper y)
			{
				return x == y || (x != null && x.Equals(y));
			}

			// Token: 0x06003724 RID: 14116 RVA: 0x00115A4C File Offset: 0x00113C4C
			public int GetHashCode(PdfWrapper obj)
			{
				if (obj == null)
				{
					return int.MinValue;
				}
				if (!obj.Dictionary.IsDisposed)
				{
					return (int)(long)obj.Dictionary.Handle;
				}
				if (PopupAnnotationCollection.PdfWrapperCompare.privateHandleGetter == null)
				{
					PopupAnnotationCollection.PdfWrapperCompare.privateHandleGetter = TypeHelper.CreateFieldOrPropertyGetter<PdfWrapper>("_handle", BindingFlags.Instance | BindingFlags.NonPublic);
				}
				return (int)PopupAnnotationCollection.PdfWrapperCompare.privateHandleGetter(obj);
			}

			// Token: 0x040025AF RID: 9647
			private static Func<PdfWrapper, object> privateHandleGetter;
		}

		// Token: 0x02000772 RID: 1906
		private class PopupAnnotationContext
		{
			// Token: 0x06003726 RID: 14118 RVA: 0x00115AB8 File Offset: 0x00113CB8
			public PopupAnnotationContext(AnnotationCanvas annotationCanvas, PdfPopupAnnotation annotation, global::System.Collections.Generic.IReadOnlyList<PdfMarkupAnnotation> replies)
			{
				PopupAnnotationWrapper popupAnnotationWrapper = new PopupAnnotationWrapper(annotation);
				if (replies != null && replies.Count > 0)
				{
					popupAnnotationWrapper.Replies = new ObservableCollection<PopupAnnotationReplyWrapper>(replies.Select((PdfMarkupAnnotation c) => new PopupAnnotationReplyWrapper(c)));
				}
				AnnotationPopupControl annotationPopupControl = new AnnotationPopupControl(annotationCanvas, popupAnnotationWrapper);
				Line line = new Line
				{
					Stroke = new SolidColorBrush(popupAnnotationWrapper.BackgroundColor)
					{
						Opacity = 0.5
					},
					StrokeThickness = 1.0,
					IsHitTestVisible = false,
					Opacity = 0.0
				};
				this.PopupAnnotationWrapper = popupAnnotationWrapper;
				this.AnnotationPopupControl = annotationPopupControl;
				this.RelationshipLine = line;
			}

			// Token: 0x17000DA3 RID: 3491
			// (get) Token: 0x06003727 RID: 14119 RVA: 0x00115B76 File Offset: 0x00113D76
			public PopupAnnotationWrapper PopupAnnotationWrapper { get; }

			// Token: 0x17000DA4 RID: 3492
			// (get) Token: 0x06003728 RID: 14120 RVA: 0x00115B7E File Offset: 0x00113D7E
			public AnnotationPopupControl AnnotationPopupControl { get; }

			// Token: 0x17000DA5 RID: 3493
			// (get) Token: 0x06003729 RID: 14121 RVA: 0x00115B86 File Offset: 0x00113D86
			public Line RelationshipLine { get; }

			// Token: 0x17000DA6 RID: 3494
			// (get) Token: 0x0600372A RID: 14122 RVA: 0x00115B8E File Offset: 0x00113D8E
			// (set) Token: 0x0600372B RID: 14123 RVA: 0x00115B96 File Offset: 0x00113D96
			public bool IsParentSelected
			{
				get
				{
					return this.isParentSelected;
				}
				set
				{
					this.isParentSelected = value;
					this.isParentHovered = false;
					this.UpdateLineVisible();
				}
			}

			// Token: 0x17000DA7 RID: 3495
			// (get) Token: 0x0600372C RID: 14124 RVA: 0x00115BAC File Offset: 0x00113DAC
			// (set) Token: 0x0600372D RID: 14125 RVA: 0x00115BB4 File Offset: 0x00113DB4
			public bool IsParentHovered
			{
				get
				{
					return this.isParentHovered;
				}
				set
				{
					this.isParentHovered = value;
					this.UpdateLineVisible();
				}
			}

			// Token: 0x17000DA8 RID: 3496
			// (get) Token: 0x0600372E RID: 14126 RVA: 0x00115BC3 File Offset: 0x00113DC3
			// (set) Token: 0x0600372F RID: 14127 RVA: 0x00115BCB File Offset: 0x00113DCB
			public bool IsPopupHovered
			{
				get
				{
					return this.isPopupHovered;
				}
				set
				{
					this.isPopupHovered = value;
					this.UpdateLineVisible();
				}
			}

			// Token: 0x06003730 RID: 14128 RVA: 0x00115BDC File Offset: 0x00113DDC
			public void UpdateLineVisible()
			{
				if (this.PopupAnnotationWrapper.IsOpen && (this.IsParentSelected || this.IsParentHovered || this.IsPopupHovered))
				{
					this.RelationshipLine.Opacity = 1.0;
					return;
				}
				this.RelationshipLine.Opacity = 0.0;
			}

			// Token: 0x040025B0 RID: 9648
			private bool isParentSelected;

			// Token: 0x040025B1 RID: 9649
			private bool isParentHovered;

			// Token: 0x040025B2 RID: 9650
			private bool isPopupHovered;
		}
	}
}
