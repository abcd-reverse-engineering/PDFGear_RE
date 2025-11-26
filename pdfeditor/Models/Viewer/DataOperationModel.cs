using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using pdfeditor.Controls;
using pdfeditor.Utils;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x0200012C RID: 300
	internal class DataOperationModel : ViewerOperationModel<DataOperationModel, bool>
	{
		// Token: 0x06001257 RID: 4695 RVA: 0x0004AE70 File Offset: 0x00049070
		public DataOperationModel(PdfViewer viewer)
			: base(viewer)
		{
			viewer.OverrideCursor = Cursors.SizeAll;
			this.createTime = DateTime.Now;
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x0004AE8F File Offset: 0x0004908F
		public DataOperationModel(PdfViewer viewer, Cursor cursor)
			: base(viewer)
		{
			viewer.OverrideCursor = cursor;
			this.createTime = DateTime.Now;
		}

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x0004AEAC File Offset: 0x000490AC
		protected AnnotationCanvas AnnotCanvas
		{
			get
			{
				AnnotationCanvas annotationCanvas;
				if (this.weakAnnotCanvas != null && this.weakAnnotCanvas.TryGetTarget(out annotationCanvas))
				{
					return annotationCanvas;
				}
				return null;
			}
		}

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x0004AED3 File Offset: 0x000490D3
		protected Border ElementContainer
		{
			get
			{
				return this.elementContainer;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x0600125B RID: 4699 RVA: 0x0004AEDB File Offset: 0x000490DB
		// (set) Token: 0x0600125C RID: 4700 RVA: 0x0004AEE3 File Offset: 0x000490E3
		public object Data { get; set; }

		// Token: 0x0600125D RID: 4701 RVA: 0x0004AEEC File Offset: 0x000490EC
		public T GetData<T>()
		{
			object data = this.Data;
			if (data is T)
			{
				return (T)((object)data);
			}
			return default(T);
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x0600125E RID: 4702 RVA: 0x0004AF1A File Offset: 0x0004911A
		// (set) Token: 0x0600125F RID: 4703 RVA: 0x0004AF22 File Offset: 0x00049122
		public UIElement PreviewElement
		{
			get
			{
				return this.previewElement;
			}
			set
			{
				if (this.previewElement != value)
				{
					this.previewElement = value;
					this.CreatePreview();
				}
			}
		}

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06001260 RID: 4704 RVA: 0x0004AF3A File Offset: 0x0004913A
		// (set) Token: 0x06001261 RID: 4705 RVA: 0x0004AF42 File Offset: 0x00049142
		public FS_SIZEF SizeInDocument { get; set; }

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06001262 RID: 4706 RVA: 0x0004AF4B File Offset: 0x0004914B
		// (set) Token: 0x06001263 RID: 4707 RVA: 0x0004AF53 File Offset: 0x00049153
		private protected Point MousePositionFromViewer { protected get; private set; }

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06001264 RID: 4708 RVA: 0x0004AF5C File Offset: 0x0004915C
		// (set) Token: 0x06001265 RID: 4709 RVA: 0x0004AF64 File Offset: 0x00049164
		public int CurrentPage { get; protected set; }

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06001266 RID: 4710 RVA: 0x0004AF6D File Offset: 0x0004916D
		// (set) Token: 0x06001267 RID: 4711 RVA: 0x0004AF75 File Offset: 0x00049175
		public FS_POINTF PositionFromDocument { get; protected set; }

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06001268 RID: 4712 RVA: 0x0004AF7E File Offset: 0x0004917E
		protected virtual bool RotatePreviewElementWithPage
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x0004AF84 File Offset: 0x00049184
		public Size GetPreviewSize(int pageIndex)
		{
			PdfViewer viewer = base.Viewer;
			FS_SIZEF sizeInDocument = this.SizeInDocument;
			Rect rect;
			if (viewer == null || !viewer.TryGetClientRect(pageIndex, new FS_RECTF(0f, sizeInDocument.Height, sizeInDocument.Width, 0f), out rect))
			{
				return new Size(0.0, 0.0);
			}
			PageRotate pageRotate;
			Pdfium.FPDF_GetPageRotationByIndex(viewer.Document.Handle, pageIndex, out pageRotate);
			if (!this.RotatePreviewElementWithPage && (pageRotate == PageRotate.Rotate90 || pageRotate == PageRotate.Rotate270))
			{
				return new Size(rect.Height, rect.Width);
			}
			return rect.Size;
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x0004B020 File Offset: 0x00049220
		protected virtual void CreatePreview()
		{
			if (base.IsDisposed || base.IsCompleted)
			{
				return;
			}
			if (this.elementContainer != null)
			{
				this.elementContainer.Child = null;
			}
			UIElement uielement = this.PreviewElement;
			if (uielement != null)
			{
				if (this.elementContainer == null)
				{
					this.elementContainer = new Border
					{
						IsHitTestVisible = false
					};
				}
				this.elementContainer.Child = uielement;
				PdfObjectExtensions.GetAnnotationCanvas(base.Viewer);
				this.UpdateAnnotCanvas();
				AnnotationCanvas annotCanvas = this.AnnotCanvas;
				if (annotCanvas != null)
				{
					annotCanvas.Children.Add(this.elementContainer);
				}
			}
			this.UpdatePagePosition();
			this.UpdatePreview();
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x0004B0BC File Offset: 0x000492BC
		private void UpdateAnnotCanvas()
		{
			if (base.IsDisposed)
			{
				return;
			}
			AnnotationCanvas annotationCanvas = this.AnnotCanvas;
			if (this.elementContainer != null && annotationCanvas != null)
			{
				annotationCanvas.Children.Remove(this.elementContainer);
			}
			WeakReference<AnnotationCanvas> weakReference = this.weakAnnotCanvas;
			if (weakReference != null)
			{
				weakReference.SetTarget(null);
			}
			this.weakAnnotCanvas = null;
			PdfViewer viewer = base.Viewer;
			if (viewer != null)
			{
				annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(viewer);
				this.weakAnnotCanvas = new WeakReference<AnnotationCanvas>(annotationCanvas);
			}
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0004B12B File Offset: 0x0004932B
		protected virtual void OnMouseMove(MouseEventArgs e)
		{
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0004B12D File Offset: 0x0004932D
		protected virtual void OnMouseDown(MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0004B12F File Offset: 0x0004932F
		protected virtual void OnMouseUp(MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0004B131 File Offset: 0x00049331
		protected virtual void OnPreviewKeyDown(KeyEventArgs e)
		{
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0004B134 File Offset: 0x00049334
		private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if ((DateTime.Now - this.createTime).TotalMilliseconds < 200.0)
			{
				this.createTime = default(DateTime);
				return;
			}
			if (base.Viewer.IsMouseCaptured)
			{
				base.Viewer.ReleaseMouseCapture();
			}
			if (DataOperationModel.IsScrollBar(e.OriginalSource))
			{
				return;
			}
			if (this.CurrentPage != -1 && !this.cancelQueued)
			{
				this.UpdateMousePosition(e);
				this.OnMouseUp(e);
				return;
			}
			Dispatcher dispatcher = base.ScrollOwner.Dispatcher ?? DispatcherHelper.UIDispatcher;
			if (dispatcher == null)
			{
				return;
			}
			dispatcher.InvokeAsync(delegate
			{
				if (!base.IsCompleted)
				{
					this.OnCompleted(false, false);
				}
			}, DispatcherPriority.Input);
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x0004B1E4 File Offset: 0x000493E4
		private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			ScrollViewer scrollOwner = base.ScrollOwner;
			bool? flag = ((scrollOwner != null) ? new bool?(scrollOwner.IsMouseOver) : null);
			if (flag == null || !flag.GetValueOrDefault())
			{
				if (!base.IsCompleted)
				{
					this.cancelQueued = true;
				}
				return;
			}
			if (DataOperationModel.IsScrollBar(e.OriginalSource))
			{
				return;
			}
			if (!base.Viewer.IsMouseCaptured)
			{
				base.Viewer.CaptureMouse();
			}
			this.UpdateMousePosition(e);
			this.OnMouseDown(e);
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0004B268 File Offset: 0x00049468
		private static bool IsScrollBar(object element)
		{
			if (element == null)
			{
				return false;
			}
			DependencyObject dependencyObject = element as DependencyObject;
			if (dependencyObject != null)
			{
				for (int i = 0; i < 5; i++)
				{
					FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
					dependencyObject = ((frameworkElement != null) ? frameworkElement.Parent : null) ?? VisualTreeHelper.GetParent(dependencyObject);
					if (dependencyObject == null)
					{
						return false;
					}
					if (dependencyObject is ScrollBar)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0004B2BD File Offset: 0x000494BD
		private void ScrollOwner_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			this.UpdateMousePosition(e);
			this.OnMouseMove(e);
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0004B2CD File Offset: 0x000494CD
		private void ScrollOwner_MouseLeave(object sender, MouseEventArgs e)
		{
			this.UpdateMousePosition(e);
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0004B2D6 File Offset: 0x000494D6
		private void ScrollOwner_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			this.OnPreviewKeyDown(e);
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0004B2DF File Offset: 0x000494DF
		private void Window_Deactivated(object sender, EventArgs e)
		{
			if (!base.IsCompleted)
			{
				this.OnCompleted(false, false);
			}
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0004B2F4 File Offset: 0x000494F4
		private void UpdateMousePosition(MouseEventArgs e)
		{
			if (base.IsCompleted)
			{
				return;
			}
			PdfViewer viewer = base.Viewer;
			if (viewer != null)
			{
				this.MousePositionFromViewer = e.GetPosition(viewer);
				this.UpdatePagePosition();
				this.UpdatePreview();
			}
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0004B330 File Offset: 0x00049530
		private void AddInputEventHandler(ScrollViewer scrollOwner)
		{
			PdfViewer viewer = base.Viewer;
			if (scrollOwner != null)
			{
				Window window = Window.GetWindow(scrollOwner);
				if (window != null)
				{
					window.Deactivated += this.Window_Deactivated;
					WeakEventManager<UIElement, MouseButtonEventArgs>.AddHandler(window, "PreviewMouseDown", new EventHandler<MouseButtonEventArgs>(this.Window_PreviewMouseDown));
					WeakEventManager<UIElement, MouseButtonEventArgs>.AddHandler(window, "PreviewMouseUp", new EventHandler<MouseButtonEventArgs>(this.Window_PreviewMouseUp));
					WeakEventManager<Window, EventArgs>.AddHandler(window, "Deactivated", new EventHandler<EventArgs>(this.Window_Deactivated));
					WeakEventManager<UIElement, MouseEventArgs>.AddHandler(scrollOwner, "PreviewMouseMove", new EventHandler<MouseEventArgs>(this.ScrollOwner_PreviewMouseMove));
					WeakEventManager<UIElement, MouseEventArgs>.AddHandler(scrollOwner, "MouseLeave", new EventHandler<MouseEventArgs>(this.ScrollOwner_MouseLeave));
					WeakEventManager<UIElement, KeyEventArgs>.AddHandler(scrollOwner, "PreviewKeyDown", new EventHandler<KeyEventArgs>(this.ScrollOwner_PreviewKeyDown));
				}
			}
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0004B3F4 File Offset: 0x000495F4
		private void RemoveInputEventHandler(ScrollViewer scrollOwner)
		{
			PdfViewer viewer = base.Viewer;
			if (scrollOwner != null)
			{
				WeakEventManager<UIElement, MouseEventArgs>.RemoveHandler(scrollOwner, "PreviewMouseMove", new EventHandler<MouseEventArgs>(this.ScrollOwner_PreviewMouseMove));
				WeakEventManager<UIElement, MouseEventArgs>.RemoveHandler(scrollOwner, "MouseLeave", new EventHandler<MouseEventArgs>(this.ScrollOwner_MouseLeave));
				WeakEventManager<UIElement, KeyEventArgs>.RemoveHandler(scrollOwner, "PreviewKeyDown", new EventHandler<KeyEventArgs>(this.ScrollOwner_PreviewKeyDown));
			}
			Window window = base.Window;
			if (window != null)
			{
				WeakEventManager<UIElement, MouseButtonEventArgs>.RemoveHandler(window, "PreviewMouseDown", new EventHandler<MouseButtonEventArgs>(this.Window_PreviewMouseDown));
				WeakEventManager<UIElement, MouseButtonEventArgs>.RemoveHandler(window, "PreviewMouseUp", new EventHandler<MouseButtonEventArgs>(this.Window_PreviewMouseUp));
				WeakEventManager<Window, EventArgs>.RemoveHandler(window, "Deactivated", new EventHandler<EventArgs>(this.Window_Deactivated));
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0004B4A0 File Offset: 0x000496A0
		protected virtual void UpdatePagePosition()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException("DataOperationModel");
			}
			if (base.IsCompleted)
			{
				return;
			}
			PdfViewer viewer = base.Viewer;
			ScrollViewer scrollOwner = base.ScrollOwner;
			UIElement uielement = this.PreviewElement;
			if (viewer == null || scrollOwner == null || uielement == null || !viewer.IsLoaded)
			{
				this.OnCompleted(true, true);
				return;
			}
			Point mousePositionFromViewer = this.MousePositionFromViewer;
			Point point;
			int num = viewer.DeviceToPage(mousePositionFromViewer.X, mousePositionFromViewer.Y, out point);
			this.CurrentPage = num;
			this.PositionFromDocument = point.ToPdfPoint();
			if (num == -1 || !scrollOwner.IsMouseOver)
			{
				this.elementContainer.Opacity = 0.0;
				return;
			}
			this.elementContainer.Opacity = 1.0;
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x0004B564 File Offset: 0x00049764
		protected virtual void UpdatePreview()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException("DataOperationModel");
			}
			if (base.IsCompleted)
			{
				return;
			}
			Point mousePositionFromViewer = this.MousePositionFromViewer;
			int currentPage = this.CurrentPage;
			Size previewSize = this.GetPreviewSize(currentPage);
			this.elementContainer.Width = previewSize.Width;
			this.elementContainer.Height = previewSize.Height;
			FrameworkElement frameworkElement = this.previewElement as FrameworkElement;
			if (frameworkElement != null)
			{
				frameworkElement.Width = previewSize.Width;
				frameworkElement.Height = previewSize.Height;
			}
			Canvas.SetLeft(this.elementContainer, mousePositionFromViewer.X);
			Canvas.SetTop(this.elementContainer, mousePositionFromViewer.Y);
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x0004B614 File Offset: 0x00049814
		protected override ViewerOperationCompletedEventArgs<bool> CreateCompletedEventArgs(ViewOperationResult<bool> result)
		{
			return new ViewerOperationCompletedEventArgs<bool>(result);
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0004B61C File Offset: 0x0004981C
		protected override void OnViewerLoaded()
		{
			base.OnViewerLoaded();
			this.UpdatePagePosition();
			this.UpdatePreview();
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x0004B630 File Offset: 0x00049830
		protected override void OnViewerScrollChanged()
		{
			base.OnViewerScrollChanged();
			this.UpdatePagePosition();
			this.UpdatePreview();
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0004B644 File Offset: 0x00049844
		protected override void OnViewerZoomChanged()
		{
			base.OnViewerZoomChanged();
			this.UpdatePagePosition();
			this.UpdatePreview();
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0004B658 File Offset: 0x00049858
		protected override void OnScrollContainerChanged(ScrollViewer oldValue, ScrollViewer newValue)
		{
			base.OnScrollContainerChanged(oldValue, newValue);
			this.RemoveInputEventHandler(oldValue);
			this.AddInputEventHandler(newValue);
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0004B670 File Offset: 0x00049870
		protected override void OnCompleted(bool success, bool result)
		{
			if (!base.IsCompleted)
			{
				this.UpdateAnnotCanvas();
				if (this.elementContainer != null)
				{
					this.elementContainer.Child = null;
				}
				this.elementContainer = null;
				PdfViewer viewer = base.Viewer;
				if (viewer != null)
				{
					viewer.OverrideCursor = null;
				}
			}
			base.OnCompleted(success, result);
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0004B6BF File Offset: 0x000498BF
		protected override void Dispose(bool disposing)
		{
			this.Data = null;
			base.Dispose(disposing);
		}

		// Token: 0x040005D5 RID: 1493
		private Border elementContainer;

		// Token: 0x040005D6 RID: 1494
		private WeakReference<AnnotationCanvas> weakAnnotCanvas;

		// Token: 0x040005D7 RID: 1495
		private UIElement previewElement;

		// Token: 0x040005D8 RID: 1496
		private bool cancelQueued;

		// Token: 0x040005D9 RID: 1497
		private DateTime createTime;
	}
}
