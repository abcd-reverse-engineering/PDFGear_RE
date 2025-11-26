using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PDFKit;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x02000130 RID: 304
	internal abstract class ViewerOperationModel<T, TResult> : IDisposable where T : ViewerOperationModel<T, TResult>
	{
		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x0004BC80 File Offset: 0x00049E80
		public Task<ViewOperationResult<TResult>> Task
		{
			get
			{
				return this.taskCompletionSource.Task;
			}
		}

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x0004BC8D File Offset: 0x00049E8D
		public bool IsDisposed
		{
			get
			{
				return this.disposedValue;
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x0004BC95 File Offset: 0x00049E95
		protected bool IsCompleted
		{
			get
			{
				return this.Task.IsCompleted;
			}
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x0004BCA2 File Offset: 0x00049EA2
		public ViewOperationResult<TResult> Result
		{
			get
			{
				if (!this.Task.IsCompleted)
				{
					return null;
				}
				return this.Task.Result;
			}
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0004BCC0 File Offset: 0x00049EC0
		public ViewerOperationModel(PdfViewer viewer)
		{
			ViewerOperationModel<T, TResult> <>4__this = this;
			this.taskCompletionSource = new TaskCompletionSource<ViewOperationResult<TResult>>();
			if (viewer != null)
			{
				this.weakViewer = new WeakReference<PdfViewer>(viewer);
				Window window = Window.GetWindow(viewer);
				if (window != null)
				{
					this.weakWindow = new WeakReference<Window>(window);
				}
			}
			this.AddEventHandler();
			viewer.Dispatcher.InvokeAsync(delegate
			{
				if (<>4__this.Window != null && viewer != null && !viewer.IsVisible)
				{
					<>4__this.OnCompleted(false, default(TResult));
				}
			}, DispatcherPriority.Input);
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06001296 RID: 4758 RVA: 0x0004BD4C File Offset: 0x00049F4C
		public PdfViewer Viewer
		{
			get
			{
				PdfViewer pdfViewer;
				if (this.weakViewer != null && this.weakViewer.TryGetTarget(out pdfViewer))
				{
					return pdfViewer;
				}
				return null;
			}
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06001297 RID: 4759 RVA: 0x0004BD74 File Offset: 0x00049F74
		protected Window Window
		{
			get
			{
				Window window;
				if (this.weakWindow != null && this.weakWindow.TryGetTarget(out window))
				{
					return window;
				}
				return null;
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06001298 RID: 4760 RVA: 0x0004BD9C File Offset: 0x00049F9C
		protected ScrollViewer ScrollOwner
		{
			get
			{
				ScrollViewer scrollViewer;
				if (this.weakScrollOwner != null && this.weakScrollOwner.TryGetTarget(out scrollViewer))
				{
					return scrollViewer;
				}
				return null;
			}
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x0004BDC4 File Offset: 0x00049FC4
		protected virtual void OnCompleted(bool success, TResult result)
		{
			if (this.IsCompleted)
			{
				throw new ArgumentException("IsCompleted");
			}
			ScrollViewer scrollOwner = this.ScrollOwner;
			this.RemoveEventHandler();
			this.OnScrollContainerChanged(scrollOwner, null);
			ViewOperationResult<TResult> viewOperationResult = (success ? new ViewOperationResult<TResult>(result) : null);
			this.taskCompletionSource.SetResult(viewOperationResult);
			ViewerOperationEventHandler<T, TResult> completed = this.Completed;
			if (completed == null)
			{
				return;
			}
			completed((T)((object)this), this.CreateCompletedEventArgs(viewOperationResult));
		}

		// Token: 0x0600129A RID: 4762
		protected abstract ViewerOperationCompletedEventArgs<TResult> CreateCompletedEventArgs(ViewOperationResult<TResult> result);

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600129B RID: 4763 RVA: 0x0004BE30 File Offset: 0x0004A030
		// (remove) Token: 0x0600129C RID: 4764 RVA: 0x0004BE68 File Offset: 0x0004A068
		public event ViewerOperationEventHandler<T, TResult> Completed;

		// Token: 0x0600129D RID: 4765 RVA: 0x0004BE9D File Offset: 0x0004A09D
		protected virtual void OnViewerLoaded()
		{
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x0004BE9F File Offset: 0x0004A09F
		protected virtual void OnViewerScrollChanged()
		{
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x0004BEA1 File Offset: 0x0004A0A1
		protected virtual void OnViewerZoomChanged()
		{
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x0004BEA3 File Offset: 0x0004A0A3
		protected virtual void OnScrollContainerChanged(ScrollViewer oldValue, ScrollViewer newValue)
		{
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x0004BEA5 File Offset: 0x0004A0A5
		private void Viewer_Loaded(object sender, EventArgs e)
		{
			this.OnViewerLoaded();
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x0004BEAD File Offset: 0x0004A0AD
		private void ScrollOwner_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.OnViewerScrollChanged();
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x0004BEB8 File Offset: 0x0004A0B8
		private void Viewer_ScrollOwnerChanged(object sender, EventArgs e)
		{
			ScrollViewer scrollOwner = this.ScrollOwner;
			this.UpdateScrollOwner();
			this.OnScrollContainerChanged(scrollOwner, this.ScrollOwner);
			this.OnViewerScrollChanged();
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x0004BEE5 File Offset: 0x0004A0E5
		private void Viewer_ZoomChanged(object sender, EventArgs e)
		{
			this.OnViewerZoomChanged();
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0004BEF0 File Offset: 0x0004A0F0
		private void AddEventHandler()
		{
			this.RemoveEventHandler();
			PdfViewer viewer = this.Viewer;
			if (viewer != null)
			{
				WeakEventManager<PdfViewer, EventArgs>.AddHandler(viewer, "Loaded", new EventHandler<EventArgs>(this.Viewer_Loaded));
				WeakEventManager<PdfViewer, EventArgs>.AddHandler(viewer, "ZoomChanged", new EventHandler<EventArgs>(this.Viewer_ZoomChanged));
				WeakEventManager<PdfViewer, EventArgs>.AddHandler(viewer, "ScrollOwnerChanged", new EventHandler<EventArgs>(this.Viewer_ScrollOwnerChanged));
				ScrollViewer scrollOwner = this.ScrollOwner;
				this.UpdateScrollOwner();
				this.OnScrollContainerChanged(scrollOwner, this.ScrollOwner);
			}
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x0004BF6C File Offset: 0x0004A16C
		private void RemoveEventHandler()
		{
			PdfViewer viewer = this.Viewer;
			if (viewer != null)
			{
				WeakEventManager<PdfViewer, EventArgs>.RemoveHandler(viewer, "Loaded", new EventHandler<EventArgs>(this.Viewer_Loaded));
				WeakEventManager<PdfViewer, EventArgs>.RemoveHandler(viewer, "ZoomChanged", new EventHandler<EventArgs>(this.Viewer_ZoomChanged));
				WeakEventManager<PdfViewer, EventArgs>.RemoveHandler(viewer, "ScrollOwnerChanged", new EventHandler<EventArgs>(this.Viewer_ScrollOwnerChanged));
			}
			ScrollViewer scrollOwner = this.ScrollOwner;
			if (scrollOwner != null)
			{
				WeakEventManager<ScrollViewer, ScrollChangedEventArgs>.RemoveHandler(scrollOwner, "ScrollChanged", new EventHandler<ScrollChangedEventArgs>(this.ScrollOwner_ScrollChanged));
			}
			WeakReference<ScrollViewer> weakReference = this.weakScrollOwner;
			if (weakReference != null)
			{
				weakReference.SetTarget(null);
			}
			this.weakScrollOwner = null;
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x0004C004 File Offset: 0x0004A204
		private void UpdateScrollOwner()
		{
			ViewerOperationModel<T, TResult>.<>c__DisplayClass35_0 CS$<>8__locals1 = new ViewerOperationModel<T, TResult>.<>c__DisplayClass35_0();
			CS$<>8__locals1.scrollOwner = this.ScrollOwner;
			if (CS$<>8__locals1.scrollOwner != null)
			{
				WeakEventManager<ScrollViewer, ScrollChangedEventArgs>.RemoveHandler(CS$<>8__locals1.scrollOwner, "ScrollChanged", new EventHandler<ScrollChangedEventArgs>(this.ScrollOwner_ScrollChanged));
			}
			ViewerOperationModel<T, TResult>.<>c__DisplayClass35_0 CS$<>8__locals2 = CS$<>8__locals1;
			PdfViewer viewer = this.Viewer;
			CS$<>8__locals2.scrollOwner = ((viewer != null) ? viewer.ScrollOwner : null);
			this.weakScrollOwner = new WeakReference<ScrollViewer>(CS$<>8__locals1.scrollOwner);
			if (CS$<>8__locals1.scrollOwner != null)
			{
				WeakEventManager<ScrollViewer, ScrollChangedEventArgs>.AddHandler(CS$<>8__locals1.scrollOwner, "ScrollChanged", new EventHandler<ScrollChangedEventArgs>(this.ScrollOwner_ScrollChanged));
				CS$<>8__locals1.scrollOwner.Dispatcher.InvokeAsync(delegate
				{
					CS$<>8__locals1.scrollOwner.Focus();
				}, DispatcherPriority.Background);
			}
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x0004C0B4 File Offset: 0x0004A2B4
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					if (!this.IsCompleted)
					{
						this.OnCompleted(false, default(TResult));
					}
					WeakReference<PdfViewer> weakReference = this.weakViewer;
					if (weakReference != null)
					{
						weakReference.SetTarget(null);
					}
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x0004C0FD File Offset: 0x0004A2FD
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x040005E3 RID: 1507
		private readonly WeakReference<PdfViewer> weakViewer;

		// Token: 0x040005E4 RID: 1508
		private readonly WeakReference<Window> weakWindow;

		// Token: 0x040005E5 RID: 1509
		private WeakReference<ScrollViewer> weakScrollOwner;

		// Token: 0x040005E6 RID: 1510
		private bool disposedValue;

		// Token: 0x040005E7 RID: 1511
		private TaskCompletionSource<ViewOperationResult<TResult>> taskCompletionSource;
	}
}
