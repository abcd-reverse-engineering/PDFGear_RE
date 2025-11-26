using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using Patagames.Pdf.Net.EventArguments;
using PDFKit;

namespace pdfeditor.Utils.Behaviors
{
	// Token: 0x02000121 RID: 289
	public class PdfViewerPageBindingBehavior : Behavior<PdfViewer>
	{
		// Token: 0x06000D0A RID: 3338 RVA: 0x00042154 File Offset: 0x00040354
		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.BeforeDocumentChanged += this.AssociatedObject_BeforeDocumentChanged;
			base.AssociatedObject.CurrentPageChanged += this.AssociatedObject_CurrentPageChanged;
			base.AssociatedObject.ScrollOwnerChanged += this.AssociatedObject_ScrollOwnerChanged;
			this.UpdateScrollOwner(false);
			this.PageIndex = base.AssociatedObject.CurrentIndex;
			this.timer.Tick += this.Timer_Tick;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x000421DC File Offset: 0x000403DC
		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.timer.Tick -= this.Timer_Tick;
			base.AssociatedObject.BeforeDocumentChanged -= this.AssociatedObject_BeforeDocumentChanged;
			base.AssociatedObject.CurrentPageChanged -= this.AssociatedObject_CurrentPageChanged;
			base.AssociatedObject.ScrollOwnerChanged -= this.AssociatedObject_ScrollOwnerChanged;
			this.UpdateScrollOwner(false);
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x00042254 File Offset: 0x00040454
		private void AssociatedObject_CurrentPageChanged(object sender, EventArgs e)
		{
			PdfViewer associatedObject = base.AssociatedObject;
			if (((associatedObject != null) ? associatedObject.Document : null) != null)
			{
				if (this.isFirstChange)
				{
					this.isFirstChange = false;
					this.timer.Stop();
					this.PageIndex = associatedObject.CurrentIndex;
					return;
				}
				this.timer.Start();
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000D0D RID: 3341 RVA: 0x000422A8 File Offset: 0x000404A8
		// (set) Token: 0x06000D0E RID: 3342 RVA: 0x000422BA File Offset: 0x000404BA
		public int PageIndex
		{
			get
			{
				return (int)base.GetValue(PdfViewerPageBindingBehavior.PageIndexProperty);
			}
			set
			{
				base.SetValue(PdfViewerPageBindingBehavior.PageIndexProperty, value);
			}
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x000422D0 File Offset: 0x000404D0
		private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				PdfViewerPageBindingBehavior pdfViewerPageBindingBehavior = d as PdfViewerPageBindingBehavior;
				if (pdfViewerPageBindingBehavior != null)
				{
					pdfViewerPageBindingBehavior.UpdateViewerPageIndex();
				}
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x00042302 File Offset: 0x00040502
		// (set) Token: 0x06000D11 RID: 3345 RVA: 0x00042314 File Offset: 0x00040514
		public bool IsDebounceEnabled
		{
			get
			{
				return (bool)base.GetValue(PdfViewerPageBindingBehavior.IsDebounceEnabledProperty);
			}
			set
			{
				base.SetValue(PdfViewerPageBindingBehavior.IsDebounceEnabledProperty, value);
			}
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x00042328 File Offset: 0x00040528
		private static void OnIsDebounceEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				PdfViewerPageBindingBehavior pdfViewerPageBindingBehavior = d as PdfViewerPageBindingBehavior;
				if (pdfViewerPageBindingBehavior != null && !(bool)e.NewValue)
				{
					pdfViewerPageBindingBehavior.timer.Stop();
					pdfViewerPageBindingBehavior.Timer_Tick(pdfViewerPageBindingBehavior.timer, EventArgs.Empty);
				}
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00042380 File Offset: 0x00040580
		private void Timer_Tick(object sender, EventArgs e)
		{
			this.timer.Stop();
			PdfViewer associatedObject = base.AssociatedObject;
			if (associatedObject != null)
			{
				this.PageIndex = associatedObject.CurrentIndex;
			}
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x000423B0 File Offset: 0x000405B0
		private void UpdateViewerPageIndex()
		{
			if (base.AssociatedObject != null && base.AssociatedObject.CurrentIndex != this.PageIndex)
			{
				if (!this.IsFromScroll)
				{
					base.AssociatedObject.ScrollToPage(this.PageIndex);
				}
				base.AssociatedObject.CurrentIndex = this.PageIndex;
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x00042404 File Offset: 0x00040604
		private void UpdateScrollOwner(bool detaching = false)
		{
			if (this.scrollViewer != null)
			{
				this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
				this.scrollViewer = null;
			}
			if (!detaching)
			{
				PdfViewer associatedObject = base.AssociatedObject;
				if (((associatedObject != null) ? associatedObject.ScrollOwner : null) != null)
				{
					this.scrollViewer = base.AssociatedObject.ScrollOwner;
					this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
				}
			}
			this.lastScrollTime = default(TimeSpan);
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x00042482 File Offset: 0x00040682
		private void AssociatedObject_BeforeDocumentChanged(object sender, DocumentClosingEventArgs e)
		{
			this.isFirstChange = true;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0004248B File Offset: 0x0004068B
		private void AssociatedObject_ScrollOwnerChanged(object sender, EventArgs e)
		{
			this.UpdateScrollOwner(false);
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x00042494 File Offset: 0x00040694
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.lastScrollTime = TimeSpan.FromTicks(Stopwatch.GetTimestamp());
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000D19 RID: 3353 RVA: 0x000424A8 File Offset: 0x000406A8
		private bool IsFromScroll
		{
			get
			{
				return (TimeSpan.FromTicks(Stopwatch.GetTimestamp()) - this.lastScrollTime).Milliseconds < 10;
			}
		}

		// Token: 0x040005A1 RID: 1441
		private TimeSpan lastScrollTime;

		// Token: 0x040005A2 RID: 1442
		private ScrollViewer scrollViewer;

		// Token: 0x040005A3 RID: 1443
		private bool isFirstChange = true;

		// Token: 0x040005A4 RID: 1444
		private DispatcherTimer timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(50.0)
		};

		// Token: 0x040005A5 RID: 1445
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(PdfViewerPageBindingBehavior), new PropertyMetadata(-1, new PropertyChangedCallback(PdfViewerPageBindingBehavior.OnPageIndexPropertyChanged)));

		// Token: 0x040005A6 RID: 1446
		public static readonly DependencyProperty IsDebounceEnabledProperty = DependencyProperty.Register("IsDebounceEnabled", typeof(bool), typeof(PdfViewerPageBindingBehavior), new PropertyMetadata(true, new PropertyChangedCallback(PdfViewerPageBindingBehavior.OnIsDebounceEnabledPropertyChanged)));
	}
}
