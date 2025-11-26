using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using HandyControl.Tools;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Services;
using pdfeditor.Utils;
using PDFKit.Utils;

namespace pdfeditor.Controls
{
	// Token: 0x020001D7 RID: 471
	[TemplatePart(Name = "PART_ContentImage")]
	public partial class PdfPagePreviewControl : Control
	{
		// Token: 0x06001A92 RID: 6802 RVA: 0x0006A878 File Offset: 0x00068A78
		static PdfPagePreviewControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewControl), new FrameworkPropertyMetadata(typeof(PdfPagePreviewControl)));
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06001A93 RID: 6803 RVA: 0x0006AA73 File Offset: 0x00068C73
		// (set) Token: 0x06001A94 RID: 6804 RVA: 0x0006AA7C File Offset: 0x00068C7C
		public Image ContentImage
		{
			get
			{
				return this.contentImage;
			}
			set
			{
				if (this.contentImage == value)
				{
					return;
				}
				Image image = this.contentImage;
				ImageSource imageSource = ((image != null) ? image.Source : null);
				if (this.contentImage != null)
				{
					this.contentImage.Source = null;
				}
				this.contentImage = value;
				if (this.contentImage != null)
				{
					this.contentImage.Source = imageSource;
				}
				this.UpdateViewport(true);
			}
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06001A95 RID: 6805 RVA: 0x0006AADC File Offset: 0x00068CDC
		// (set) Token: 0x06001A96 RID: 6806 RVA: 0x0006AAE4 File Offset: 0x00068CE4
		public ScrollViewer ScrollViewer
		{
			get
			{
				return this.scrollViewer;
			}
			set
			{
				if (this.scrollViewer == value)
				{
					return;
				}
				if (this.scrollViewer != null)
				{
					this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
				}
				this.scrollViewer = value;
				this.viewportContext = default(PdfPagePreviewControl.ViewportContext);
				if (this.scrollViewer != null)
				{
					this.scrollViewer.ScrollChanged += this.ScrollViewer_ScrollChanged;
				}
				this.UpdateViewport(true);
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x0006AB53 File Offset: 0x00068D53
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.UpdateViewport(true);
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x0006AB5C File Offset: 0x00068D5C
		public PdfPagePreviewControl()
		{
			base.IsVisibleChanged += this.PdfPagePreviewControl_IsVisibleChanged;
			this.IsImageLoaded = false;
			VisualStateManager.GoToState(this, "ImageLoading", true);
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x0006AB8A File Offset: 0x00068D8A
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.ContentImage = base.GetTemplateChild("PART_ContentImage") as Image;
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x0006ABA8 File Offset: 0x00068DA8
		private void PdfPagePreviewControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				DependencyObject itemContainer = this.GetItemContainer();
				this.ScrollViewer = ScrollUtils.GetScrollViewerFromItemContainer(itemContainer);
				return;
			}
			this.ScrollViewer = null;
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource == null)
			{
				return;
			}
			cancellationTokenSource.Cancel();
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x0006ABE8 File Offset: 0x00068DE8
		private void UpdateViewport(bool force = false)
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			if (this.Paused)
			{
				return;
			}
			bool flag = false;
			if (base.IsVisible)
			{
				if (this.ForceImageSize)
				{
					this.TrySetElementSize(force);
				}
				if (this.ScrollViewer != null)
				{
					PdfPagePreviewControl.ViewportContext viewportContext = PdfPagePreviewControl.ViewportContext.Create(this.ScrollViewer);
					if (force)
					{
						this.viewportContext = viewportContext;
					}
					else if (!viewportContext.Changed(this.viewportContext))
					{
						return;
					}
					try
					{
						Grid parent = VisualHelper.GetParent<Grid>(this);
						Rect rect = parent.TransformToVisual(this.ScrollViewer).TransformBounds(new Rect(0.0, 0.0, parent.ActualWidth, parent.ActualHeight));
						if (rect.Bottom > -100.0 && rect.Top < this.ScrollViewer.ActualHeight + 100.0)
						{
							flag = true;
						}
					}
					catch
					{
					}
				}
			}
			this.TryUpdateImageSource(flag, force);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x0006ACE4 File Offset: 0x00068EE4
		private PageRotate GetRotate(PdfPage page)
		{
			if (this.RenderActualRotate)
			{
				return page.Rotation;
			}
			return PageRotate.Normal;
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x0006ACF8 File Offset: 0x00068EF8
		private void TrySetElementSize(bool force = false)
		{
			if (this.ContentImage == null || this.ScrollViewer == null)
			{
				return;
			}
			if (!double.IsNaN(this.ContentImage.Width) && !force)
			{
				return;
			}
			PdfThumbnailService requiredService = Ioc.Default.GetRequiredService<PdfThumbnailService>();
			if (this.Document == null || this.PageIndex == -1 || this.PageIndex >= this.Document.Pages.Count)
			{
				return;
			}
			PdfPage pdfPage = this.Document.Pages[this.PageIndex];
			global::System.ValueTuple<int, int> thumbnailSize = this.GetThumbnailSize();
			int item = thumbnailSize.Item1;
			int item2 = thumbnailSize.Item2;
			Size thumbnailImageSize = requiredService.GetThumbnailImageSize(pdfPage, this.GetRotate(pdfPage), item, item2);
			this.ContentImage.Width = thumbnailImageSize.Width;
			this.ContentImage.Height = thumbnailImageSize.Height;
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x0006ADC4 File Offset: 0x00068FC4
		private async void TryUpdateImageSource(bool show, bool force = false)
		{
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this.cts = new CancellationTokenSource();
			if (this.ContentImage != null)
			{
				if (show)
				{
					if (force || this.ContentImage.Source == null)
					{
						await this.TryUpdateImageSourceAsync();
					}
					if (this.RenderActualRotate)
					{
						StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<int>, string>(this, "MESSAGE_PAGE_ROTATE_CHANGED");
						StrongReferenceMessenger.Default.Register(this, "MESSAGE_PAGE_ROTATE_CHANGED", new MessageHandler<object, ValueChangedMessage<int>>(this.OnPageRotateChanged));
					}
				}
				else
				{
					this.ContentImage.Source = null;
					this.IsImageLoaded = false;
					VisualStateManager.GoToState(this, "ImageLoading", true);
					StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<int>, string>(this, "MESSAGE_PAGE_ROTATE_CHANGED");
				}
			}
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x0006AE0C File Offset: 0x0006900C
		private void OnPageRotateChanged(object recipient, ValueChangedMessage<int> message)
		{
			int? num = ((message != null) ? new int?(message.Value) : null);
			int pageIndex = this.PageIndex;
			if (((num.GetValueOrDefault() == pageIndex) & (num != null)) || (message != null && message.Value == -1))
			{
				this.UpdateViewport(true);
			}
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x0006AE64 File Offset: 0x00069064
		private async Task TryUpdateImageSourceAsync()
		{
			if (this.ContentImage != null)
			{
				await Task.Delay(10).ConfigureAwait(false);
				if (this.cts == null || this.cts.IsCancellationRequested)
				{
					this.cts = null;
				}
				else
				{
					await base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(async delegate
					{
						if (this.cts == null || this.cts.IsCancellationRequested)
						{
							this.cts = null;
						}
						else if (this.ContentImage != null)
						{
							this.imageUpdating = true;
							try
							{
								PdfThumbnailService requiredService = Ioc.Default.GetRequiredService<PdfThumbnailService>();
								if (this.Document == null || this.PageIndex == -1 || this.PageIndex >= this.Document.Pages.Count)
								{
									this.ContentImage.Source = null;
								}
								PdfPage pdfPage = this.Document.Pages[this.PageIndex];
								global::System.ValueTuple<int, int> thumbnailSize = this.GetThumbnailSize();
								int item = thumbnailSize.Item1;
								int item2 = thumbnailSize.Item2;
								WriteableBitmap writeableBitmap = await requiredService.TryGetPdfBitmapAsync(pdfPage, this.TryGetBackgroundColor(), this.GetRotate(pdfPage), item, item2, this.cts.Token);
								if (this.ContentImage != null)
								{
									this.ContentImage.Source = writeableBitmap;
									this.IsImageLoaded = true;
									VisualStateManager.GoToState(this, "ImageReady", true);
								}
								this.cts = null;
							}
							catch (OperationCanceledException)
							{
							}
							this.imageUpdating = false;
						}
					}));
				}
			}
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x0006AEA8 File Offset: 0x000690A8
		private Color TryGetBackgroundColor()
		{
			Brush background = base.Background;
			SolidColorBrush solidColorBrush = base.Background as SolidColorBrush;
			if (solidColorBrush != null)
			{
				byte b = Math.Min(Math.Max((byte)((double)solidColorBrush.Color.A * solidColorBrush.Opacity), 0), byte.MaxValue);
				byte r = solidColorBrush.Color.R;
				byte g = solidColorBrush.Color.G;
				byte b2 = solidColorBrush.Color.B;
				return Color.FromArgb(b, r, g, b2);
			}
			return Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x0006AF48 File Offset: 0x00069148
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "width", "height" })]
		private global::System.ValueTuple<int, int> GetThumbnailSize()
		{
			if (this.Document == null || this.Document.Pages.Count <= this.PageIndex)
			{
				return default(global::System.ValueTuple<int, int>);
			}
			int num = this.ThumbnailWidth;
			int num2 = this.ThumbnailHeight;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num == 0 && num2 == 0)
			{
				num = 150;
			}
			PdfPage pdfPage = this.Document.Pages[this.PageIndex];
			if (num == 0 && this.GetRotate(pdfPage) != PageRotate.Normal)
			{
				FS_SIZEF effectiveSize = pdfPage.GetEffectiveSize(pdfPage.Rotation, false);
				FS_SIZEF effectiveSize2 = pdfPage.GetEffectiveSize(PageRotate.Normal, false);
				num = (int)((double)(effectiveSize2.Width * (float)num2) * 1.0 / (double)effectiveSize2.Height);
				num2 = (int)((double)(effectiveSize.Height * (float)num) * 1.0 / (double)effectiveSize.Width);
			}
			return new global::System.ValueTuple<int, int>(num, num2);
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06001AA3 RID: 6819 RVA: 0x0006B028 File Offset: 0x00069228
		// (set) Token: 0x06001AA4 RID: 6820 RVA: 0x0006B03A File Offset: 0x0006923A
		public bool ForceImageSize
		{
			get
			{
				return (bool)base.GetValue(PdfPagePreviewControl.ForceImageSizeProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.ForceImageSizeProperty, value);
			}
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x06001AA5 RID: 6821 RVA: 0x0006B04D File Offset: 0x0006924D
		// (set) Token: 0x06001AA6 RID: 6822 RVA: 0x0006B05F File Offset: 0x0006925F
		public bool IsImageLoaded
		{
			get
			{
				return (bool)base.GetValue(PdfPagePreviewControl.IsImageLoadedProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.IsImageLoadedProperty, value);
			}
		}

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x06001AA7 RID: 6823 RVA: 0x0006B072 File Offset: 0x00069272
		// (set) Token: 0x06001AA8 RID: 6824 RVA: 0x0006B084 File Offset: 0x00069284
		public PdfDocument Document
		{
			get
			{
				return (PdfDocument)base.GetValue(PdfPagePreviewControl.DocumentProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.DocumentProperty, value);
			}
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x0006B092 File Offset: 0x00069292
		// (set) Token: 0x06001AAA RID: 6826 RVA: 0x0006B0A4 File Offset: 0x000692A4
		public bool RenderActualRotate
		{
			get
			{
				return (bool)base.GetValue(PdfPagePreviewControl.RenderActualRotateProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.RenderActualRotateProperty, value);
			}
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x0006B0B8 File Offset: 0x000692B8
		private static void OnRenderActualRotatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				PdfPagePreviewControl pdfPagePreviewControl = d as PdfPagePreviewControl;
				if (pdfPagePreviewControl != null)
				{
					int pageIndex = pdfPagePreviewControl.PageIndex;
					PdfDocument document = pdfPagePreviewControl.Document;
					PdfPageCollection pdfPageCollection = ((document != null) ? document.Pages : null);
					if (pdfPageCollection != null && pageIndex >= 0 && pageIndex < pdfPageCollection.Count && pdfPageCollection[pageIndex].Rotation != PageRotate.Normal)
					{
						pdfPagePreviewControl.UpdateViewport(false);
					}
				}
			}
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06001AAC RID: 6828 RVA: 0x0006B128 File Offset: 0x00069328
		// (set) Token: 0x06001AAD RID: 6829 RVA: 0x0006B13A File Offset: 0x0006933A
		public int PageIndex
		{
			get
			{
				return (int)base.GetValue(PdfPagePreviewControl.PageIndexProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.PageIndexProperty, value);
			}
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06001AAE RID: 6830 RVA: 0x0006B14D File Offset: 0x0006934D
		// (set) Token: 0x06001AAF RID: 6831 RVA: 0x0006B15F File Offset: 0x0006935F
		public int ThumbnailHeight
		{
			get
			{
				return (int)base.GetValue(PdfPagePreviewControl.ThumbnailHeightProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.ThumbnailHeightProperty, value);
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x06001AB0 RID: 6832 RVA: 0x0006B172 File Offset: 0x00069372
		// (set) Token: 0x06001AB1 RID: 6833 RVA: 0x0006B184 File Offset: 0x00069384
		public int ThumbnailWidth
		{
			get
			{
				return (int)base.GetValue(PdfPagePreviewControl.ThumbnailWidthProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.ThumbnailWidthProperty, value);
			}
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x0006B198 File Offset: 0x00069398
		private static async void OnThumbnailSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PdfPagePreviewControl sender = d as PdfPagePreviewControl;
			if (sender != null)
			{
				if ((int)e.OldValue != 0)
				{
					CancellationTokenSource cts = new CancellationTokenSource();
					CancellationTokenSource cancellationTokenSource = sender.cts;
					if (cancellationTokenSource != null)
					{
						cancellationTokenSource.Cancel();
					}
					sender.cts = cts;
					await sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						if (cts.IsCancellationRequested || !sender.IsLoaded)
						{
							return;
						}
						sender.UpdateViewport(true);
					}));
				}
				else
				{
					sender.UpdateViewport(true);
				}
			}
		}

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x0006B1D7 File Offset: 0x000693D7
		// (set) Token: 0x06001AB4 RID: 6836 RVA: 0x0006B1E9 File Offset: 0x000693E9
		public bool Paused
		{
			get
			{
				return (bool)base.GetValue(PdfPagePreviewControl.PausedProperty);
			}
			set
			{
				base.SetValue(PdfPagePreviewControl.PausedProperty, value);
			}
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0006B1FC File Offset: 0x000693FC
		private static void OnPausedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (object.Equals(e.NewValue, e.OldValue))
			{
				PdfPagePreviewControl pdfPagePreviewControl = d as PdfPagePreviewControl;
				if (pdfPagePreviewControl != null)
				{
					if ((bool)e.NewValue)
					{
						if (!pdfPagePreviewControl.imageUpdating)
						{
							pdfPagePreviewControl.UpdateViewport(false);
							return;
						}
					}
					else
					{
						CancellationTokenSource cancellationTokenSource = pdfPagePreviewControl.cts;
						if (cancellationTokenSource != null)
						{
							cancellationTokenSource.Cancel();
						}
						pdfPagePreviewControl.cts = null;
					}
				}
			}
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x0006B25E File Offset: 0x0006945E
		private DependencyObject GetItemContainer()
		{
			return PdfPagePreviewControl.FindParent<PdfPagePreviewListViewItem>(this, null, 20);
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x0006B26C File Offset: 0x0006946C
		private static T FindParent<T>(DependencyObject element, string name = null, int maxDeep = 20) where T : DependencyObject
		{
			PdfPagePreviewControl.<>c__DisplayClass61_0<T> CS$<>8__locals1;
			CS$<>8__locals1.name = name;
			if (maxDeep < 0)
			{
				return default(T);
			}
			if (maxDeep != 0)
			{
				for (int i = 0; i < maxDeep; i++)
				{
					if (PdfPagePreviewControl.<FindParent>g__IsResult|61_1<T>(element, ref CS$<>8__locals1))
					{
						return (T)((object)element);
					}
					element = PdfPagePreviewControl.<FindParent>g__GetParent|61_0<T>(element);
				}
				return default(T);
			}
			if (PdfPagePreviewControl.<FindParent>g__IsResult|61_1<T>(element, ref CS$<>8__locals1))
			{
				return (T)((object)element);
			}
			return default(T);
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x0006B2DC File Offset: 0x000694DC
		private static ScrollViewer GetScrollViewerFromItemContainer(DependencyObject container)
		{
			ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
			if (itemsControl != null && VisualTreeHelper.GetChildrenCount(itemsControl) > 0)
			{
				FrameworkElement frameworkElement = VisualTreeHelper.GetChild(itemsControl, 0) as FrameworkElement;
				if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
				{
					return VisualTreeHelper.GetChild(frameworkElement, 0) as ScrollViewer;
				}
			}
			return null;
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x0006B35C File Offset: 0x0006955C
		[CompilerGenerated]
		internal static DependencyObject <FindParent>g__GetParent|61_0<T>(DependencyObject _element) where T : DependencyObject
		{
			FrameworkElement frameworkElement = _element as FrameworkElement;
			if (frameworkElement != null)
			{
				DependencyObject parent = frameworkElement.Parent;
				if (parent != null)
				{
					return parent;
				}
			}
			FrameworkContentElement frameworkContentElement = _element as FrameworkContentElement;
			if (frameworkContentElement != null)
			{
				DependencyObject parent2 = frameworkContentElement.Parent;
				if (parent2 != null)
				{
					return parent2;
				}
			}
			return VisualTreeHelper.GetParent(_element);
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x0006B39C File Offset: 0x0006959C
		[CompilerGenerated]
		internal static bool <FindParent>g__IsResult|61_1<T>(DependencyObject _element, ref PdfPagePreviewControl.<>c__DisplayClass61_0<T> A_1) where T : DependencyObject
		{
			T t = _element as T;
			if (t != null)
			{
				if (string.IsNullOrEmpty(A_1.name))
				{
					return true;
				}
				FrameworkElement frameworkElement = t as FrameworkElement;
				if (frameworkElement != null && frameworkElement.Name == A_1.name)
				{
					return true;
				}
				FrameworkContentElement frameworkContentElement = t as FrameworkContentElement;
				if (frameworkContentElement != null && frameworkContentElement.Name == A_1.name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000944 RID: 2372
		private const string PART_ContentImageName = "PART_ContentImage";

		// Token: 0x04000945 RID: 2373
		private Image contentImage;

		// Token: 0x04000946 RID: 2374
		private ScrollViewer scrollViewer;

		// Token: 0x04000947 RID: 2375
		private CancellationTokenSource cts;

		// Token: 0x04000948 RID: 2376
		private PdfPagePreviewControl.ViewportContext viewportContext;

		// Token: 0x04000949 RID: 2377
		private bool imageUpdating;

		// Token: 0x0400094A RID: 2378
		public static readonly DependencyProperty ForceImageSizeProperty = DependencyProperty.Register("ForceImageSize", typeof(bool), typeof(PdfPagePreviewControl), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			if (!object.Equals(a.NewValue, a.OldValue))
			{
				PdfPagePreviewControl pdfPagePreviewControl = s as PdfPagePreviewControl;
				if (pdfPagePreviewControl != null && pdfPagePreviewControl.IsImageLoaded)
				{
					pdfPagePreviewControl.TrySetElementSize(true);
				}
			}
		}));

		// Token: 0x0400094B RID: 2379
		public static readonly DependencyProperty IsImageLoadedProperty = DependencyProperty.Register("IsImageLoaded", typeof(bool), typeof(PdfPagePreviewControl), new PropertyMetadata(false));

		// Token: 0x0400094C RID: 2380
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(PdfDocument), typeof(PdfPagePreviewControl), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PdfPagePreviewControl pdfPagePreviewControl2 = s as PdfPagePreviewControl;
			if (pdfPagePreviewControl2 != null)
			{
				pdfPagePreviewControl2.UpdateViewport(true);
			}
		}));

		// Token: 0x0400094D RID: 2381
		public static readonly DependencyProperty RenderActualRotateProperty = DependencyProperty.Register("RenderActualRotate", typeof(bool), typeof(PdfPagePreviewControl), new PropertyMetadata(false, new PropertyChangedCallback(PdfPagePreviewControl.OnRenderActualRotatePropertyChanged)));

		// Token: 0x0400094E RID: 2382
		public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register("PageIndex", typeof(int), typeof(PdfPagePreviewControl), new PropertyMetadata(-1, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			PdfPagePreviewControl pdfPagePreviewControl3 = s as PdfPagePreviewControl;
			if (pdfPagePreviewControl3 != null)
			{
				pdfPagePreviewControl3.UpdateViewport(true);
			}
		}));

		// Token: 0x0400094F RID: 2383
		public static readonly DependencyProperty ThumbnailHeightProperty = DependencyProperty.Register("ThumbnailHeight", typeof(int), typeof(PdfPagePreviewControl), new PropertyMetadata(0, new PropertyChangedCallback(PdfPagePreviewControl.OnThumbnailSizePropertyChanged)));

		// Token: 0x04000950 RID: 2384
		public static readonly DependencyProperty ThumbnailWidthProperty = DependencyProperty.Register("ThumbnailWidth", typeof(int), typeof(PdfPagePreviewControl), new PropertyMetadata(0, new PropertyChangedCallback(PdfPagePreviewControl.OnThumbnailSizePropertyChanged)));

		// Token: 0x04000951 RID: 2385
		public static readonly DependencyProperty PausedProperty = DependencyProperty.Register("Paused", typeof(bool), typeof(PdfPagePreviewControl), new PropertyMetadata(false, new PropertyChangedCallback(PdfPagePreviewControl.OnPausedPropertyChanged)));

		// Token: 0x020005F9 RID: 1529
		private struct ViewportContext
		{
			// Token: 0x060032E1 RID: 13025 RVA: 0x000F98AA File Offset: 0x000F7AAA
			public ViewportContext(double offsetY, double viewportHeight)
			{
				this.OffsetY = offsetY;
				this.ViewportHeight = viewportHeight;
			}

			// Token: 0x17000D3C RID: 3388
			// (get) Token: 0x060032E2 RID: 13026 RVA: 0x000F98BA File Offset: 0x000F7ABA
			public double OffsetY { get; }

			// Token: 0x17000D3D RID: 3389
			// (get) Token: 0x060032E3 RID: 13027 RVA: 0x000F98C2 File Offset: 0x000F7AC2
			public double ViewportHeight { get; }

			// Token: 0x060032E4 RID: 13028 RVA: 0x000F98CA File Offset: 0x000F7ACA
			public bool Changed(PdfPagePreviewControl.ViewportContext oldContext)
			{
				return Math.Abs(this.OffsetY - oldContext.OffsetY) > 10.0 || Math.Abs(this.ViewportHeight - oldContext.ViewportHeight) > 10.0;
			}

			// Token: 0x060032E5 RID: 13029 RVA: 0x000F990C File Offset: 0x000F7B0C
			public static PdfPagePreviewControl.ViewportContext Create(ScrollViewer scrollViewer)
			{
				if (scrollViewer == null)
				{
					return default(PdfPagePreviewControl.ViewportContext);
				}
				return new PdfPagePreviewControl.ViewportContext(scrollViewer.VerticalOffset, scrollViewer.ViewportHeight);
			}
		}
	}
}
