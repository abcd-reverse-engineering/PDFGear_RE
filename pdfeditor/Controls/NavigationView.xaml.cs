using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001D2 RID: 466
	[TemplatePart(Name = "PART_LayoutRoot")]
	[TemplatePart(Name = "PART_ResizeDragger")]
	public partial class NavigationView : ListBox
	{
		// Token: 0x06001A57 RID: 6743 RVA: 0x00069DE4 File Offset: 0x00067FE4
		static NavigationView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationView), new FrameworkPropertyMetadata(typeof(NavigationView)));
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0006A0D1 File Offset: 0x000682D1
		public NavigationView()
		{
			VisualStateManager.GoToState(this, "Close", true);
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06001A59 RID: 6745 RVA: 0x0006A0F5 File Offset: 0x000682F5
		// (set) Token: 0x06001A5A RID: 6746 RVA: 0x0006A102 File Offset: 0x00068302
		public object Header
		{
			get
			{
				return base.GetValue(NavigationView.HeaderProperty);
			}
			set
			{
				base.SetValue(NavigationView.HeaderProperty, value);
			}
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06001A5B RID: 6747 RVA: 0x0006A110 File Offset: 0x00068310
		// (set) Token: 0x06001A5C RID: 6748 RVA: 0x0006A122 File Offset: 0x00068322
		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(NavigationView.HeaderTemplateProperty);
			}
			set
			{
				base.SetValue(NavigationView.HeaderTemplateProperty, value);
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06001A5D RID: 6749 RVA: 0x0006A130 File Offset: 0x00068330
		// (set) Token: 0x06001A5E RID: 6750 RVA: 0x0006A13D File Offset: 0x0006833D
		public object Footer
		{
			get
			{
				return base.GetValue(NavigationView.FooterProperty);
			}
			set
			{
				base.SetValue(NavigationView.FooterProperty, value);
			}
		}

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x0006A14B File Offset: 0x0006834B
		// (set) Token: 0x06001A60 RID: 6752 RVA: 0x0006A15D File Offset: 0x0006835D
		public DataTemplate FooterTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(NavigationView.FooterTemplateProperty);
			}
			set
			{
				base.SetValue(NavigationView.FooterTemplateProperty, value);
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06001A61 RID: 6753 RVA: 0x0006A16B File Offset: 0x0006836B
		// (set) Token: 0x06001A62 RID: 6754 RVA: 0x0006A17D File Offset: 0x0006837D
		public double NavigationListWidth
		{
			get
			{
				return (double)base.GetValue(NavigationView.NavigationListWidthProperty);
			}
			set
			{
				base.SetValue(NavigationView.NavigationListWidthProperty, value);
			}
		}

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06001A63 RID: 6755 RVA: 0x0006A190 File Offset: 0x00068390
		// (set) Token: 0x06001A64 RID: 6756 RVA: 0x0006A19D File Offset: 0x0006839D
		public object Content
		{
			get
			{
				return base.GetValue(NavigationView.ContentProperty);
			}
			set
			{
				base.SetValue(NavigationView.ContentProperty, value);
			}
		}

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06001A65 RID: 6757 RVA: 0x0006A1AB File Offset: 0x000683AB
		// (set) Token: 0x06001A66 RID: 6758 RVA: 0x0006A1BD File Offset: 0x000683BD
		public DataTemplate ContentTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(NavigationView.ContentTemplateProperty);
			}
			set
			{
				base.SetValue(NavigationView.ContentTemplateProperty, value);
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06001A67 RID: 6759 RVA: 0x0006A1CB File Offset: 0x000683CB
		// (set) Token: 0x06001A68 RID: 6760 RVA: 0x0006A1DD File Offset: 0x000683DD
		public Brush PaneBackground
		{
			get
			{
				return (Brush)base.GetValue(NavigationView.PaneBackgroundProperty);
			}
			set
			{
				base.SetValue(NavigationView.PaneBackgroundProperty, value);
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06001A69 RID: 6761 RVA: 0x0006A1EB File Offset: 0x000683EB
		// (set) Token: 0x06001A6A RID: 6762 RVA: 0x0006A1FD File Offset: 0x000683FD
		public double MinContentWidth
		{
			get
			{
				return (double)base.GetValue(NavigationView.MinContentWidthProperty);
			}
			set
			{
				base.SetValue(NavigationView.MinContentWidthProperty, value);
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06001A6B RID: 6763 RVA: 0x0006A210 File Offset: 0x00068410
		// (set) Token: 0x06001A6C RID: 6764 RVA: 0x0006A222 File Offset: 0x00068422
		public double MaxContentWidth
		{
			get
			{
				return (double)base.GetValue(NavigationView.MaxContentWidthProperty);
			}
			set
			{
				base.SetValue(NavigationView.MaxContentWidthProperty, value);
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06001A6D RID: 6765 RVA: 0x0006A235 File Offset: 0x00068435
		// (set) Token: 0x06001A6E RID: 6766 RVA: 0x0006A247 File Offset: 0x00068447
		public double ContentWidth
		{
			get
			{
				return (double)base.GetValue(NavigationView.ContentWidthProperty);
			}
			set
			{
				base.SetValue(NavigationView.ContentWidthProperty, value);
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06001A6F RID: 6767 RVA: 0x0006A25A File Offset: 0x0006845A
		// (set) Token: 0x06001A70 RID: 6768 RVA: 0x0006A26C File Offset: 0x0006846C
		public bool IsClosed
		{
			get
			{
				return (bool)base.GetValue(NavigationView.IsClosedProperty);
			}
			set
			{
				base.SetValue(NavigationView.IsClosedProperty, value);
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x06001A71 RID: 6769 RVA: 0x0006A27F File Offset: 0x0006847F
		// (set) Token: 0x06001A72 RID: 6770 RVA: 0x0006A291 File Offset: 0x00068491
		public NavigationViewDirection Direction
		{
			get
			{
				return (NavigationViewDirection)base.GetValue(NavigationView.DirectionProperty);
			}
			set
			{
				base.SetValue(NavigationView.DirectionProperty, value);
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x06001A73 RID: 6771 RVA: 0x0006A2A4 File Offset: 0x000684A4
		// (set) Token: 0x06001A74 RID: 6772 RVA: 0x0006A2B6 File Offset: 0x000684B6
		public bool IsAnimationEnabled
		{
			get
			{
				return (bool)base.GetValue(NavigationView.IsAnimationEnabledProperty);
			}
			set
			{
				base.SetValue(NavigationView.IsAnimationEnabledProperty, value);
			}
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0006A2CC File Offset: 0x000684CC
		private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				NavigationView navigationView = d as NavigationView;
				if (navigationView != null)
				{
					navigationView.RemoveLogicalChild(e.OldValue);
					DependencyObject dependencyObject = e.NewValue as DependencyObject;
					if (dependencyObject != null)
					{
						navigationView.AddLogicalChild(dependencyObject);
					}
				}
			}
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0006A318 File Offset: 0x00068518
		private static void OnFooterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				NavigationView navigationView = d as NavigationView;
				if (navigationView != null)
				{
					navigationView.RemoveLogicalChild(e.OldValue);
					DependencyObject dependencyObject = e.NewValue as DependencyObject;
					if (dependencyObject != null)
					{
						navigationView.AddLogicalChild(dependencyObject);
					}
				}
			}
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0006A364 File Offset: 0x00068564
		private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				NavigationView navigationView = d as NavigationView;
				if (navigationView != null)
				{
					navigationView.RemoveLogicalChild(e.OldValue);
					DependencyObject dependencyObject = e.NewValue as DependencyObject;
					if (dependencyObject != null)
					{
						navigationView.AddLogicalChild(dependencyObject);
					}
				}
			}
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0006A3B0 File Offset: 0x000685B0
		private static void OnIsClosedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NavigationView navigationView = d as NavigationView;
			if (navigationView != null && !object.Equals(e.NewValue, e.OldValue))
			{
				navigationView.UpdateInlineState();
			}
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x0006A3E4 File Offset: 0x000685E4
		private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NavigationView navigationView = d as NavigationView;
			if (navigationView != null && !object.Equals(e.NewValue, e.OldValue))
			{
				navigationView.UpdateInlineState();
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06001A7A RID: 6778 RVA: 0x0006A416 File Offset: 0x00068616
		// (set) Token: 0x06001A7B RID: 6779 RVA: 0x0006A420 File Offset: 0x00068620
		private Thumb ResizeDragger
		{
			get
			{
				return this.resizeDragger;
			}
			set
			{
				if (this.resizeDragger != value)
				{
					if (this.resizeDragger != null)
					{
						this.resizeDragger.DragStarted -= this.ResizeDragger_DragStarted;
						this.resizeDragger.DragDelta -= this.ResizeDragger_DragDelta;
						this.resizeDragger.DragCompleted -= this.ResizeDragger_DragCompleted;
					}
					this.resizeDragger = value;
					if (this.resizeDragger != null)
					{
						this.resizeDragger.DragStarted += this.ResizeDragger_DragStarted;
						this.resizeDragger.DragDelta += this.ResizeDragger_DragDelta;
						this.resizeDragger.DragCompleted += this.ResizeDragger_DragCompleted;
					}
				}
			}
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x0006A4DA File Offset: 0x000686DA
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.ResizeDragger = base.GetTemplateChild("PART_ResizeDragger") as Thumb;
			this.UpdateInlineState();
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x0006A4FE File Offset: 0x000686FE
		private void ResizeDragger_DragStarted(object sender, DragStartedEventArgs e)
		{
			this.oldWidth = this.ContentWidth;
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x0006A50C File Offset: 0x0006870C
		private void ResizeDragger_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (this.oldWidth == -1.0)
			{
				return;
			}
			double num = e.HorizontalChange;
			if (this.Direction == NavigationViewDirection.Right)
			{
				num = -num;
			}
			this.oldWidth += num;
			if (this.oldWidth > this.MaxContentWidth)
			{
				this.oldWidth = this.MaxContentWidth;
			}
			if (this.oldWidth < this.MinContentWidth)
			{
				this.oldWidth = this.MinContentWidth;
			}
			this.ContentWidth = this.oldWidth;
			Ioc.Default.GetRequiredService<MainViewModel>().SetPageStyle();
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x0006A59B File Offset: 0x0006879B
		private void ResizeDragger_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			this.oldWidth = -1.0;
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x0006A5AC File Offset: 0x000687AC
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new NavigationViewItem();
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x0006A5B3 File Offset: 0x000687B3
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is NavigationViewItem;
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x0006A5C0 File Offset: 0x000687C0
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			NavigationViewItem navigationViewItem = element as NavigationViewItem;
			if (navigationViewItem != null)
			{
				navigationViewItem.ItemClicked += this.Container_ItemClicked;
			}
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x0006A5F4 File Offset: 0x000687F4
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			NavigationViewItem navigationViewItem = element as NavigationViewItem;
			if (navigationViewItem != null)
			{
				navigationViewItem.ItemClicked -= this.Container_ItemClicked;
			}
			base.ClearContainerForItemOverride(element, item);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x0006A628 File Offset: 0x00068828
		private async void Container_ItemClicked(object sender, NavigationViewItemClickEventArgs e)
		{
			NavigationViewItem element = sender as NavigationViewItem;
			if (element != null && element.IsSelected)
			{
				await Task.Yield();
				element.IsSelected = false;
			}
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x0006A660 File Offset: 0x00068860
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			object selectedItem = base.SelectedItem;
			this.IsClosed = selectedItem == null;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (requiredService.DocumentWrapper.Document != null)
			{
				requiredService.SetPageStyle();
			}
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x0006A6A4 File Offset: 0x000688A4
		private void UpdateInlineState()
		{
			if (this.IsClosed)
			{
				VisualStateManager.GoToState(this, "Close", this.IsAnimationEnabled);
				return;
			}
			if (this.Direction == NavigationViewDirection.Left)
			{
				VisualStateManager.GoToState(this, "LeftInline", this.IsAnimationEnabled);
				return;
			}
			VisualStateManager.GoToState(this, "RightInline", this.IsAnimationEnabled);
		}

		// Token: 0x0400092D RID: 2349
		private const string LayoutRootName = "PART_LayoutRoot";

		// Token: 0x0400092E RID: 2350
		private const string ResizeDraggerName = "PART_ResizeDragger";

		// Token: 0x0400092F RID: 2351
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(NavigationView), new PropertyMetadata(null, new PropertyChangedCallback(NavigationView.OnHeaderPropertyChanged)));

		// Token: 0x04000930 RID: 2352
		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NavigationView), new PropertyMetadata(null));

		// Token: 0x04000931 RID: 2353
		public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(object), typeof(NavigationView), new PropertyMetadata(null, new PropertyChangedCallback(NavigationView.OnFooterPropertyChanged)));

		// Token: 0x04000932 RID: 2354
		public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(NavigationView), new PropertyMetadata(null));

		// Token: 0x04000933 RID: 2355
		public static readonly DependencyProperty NavigationListWidthProperty = DependencyProperty.Register("NavigationListWidth", typeof(double), typeof(NavigationView), new PropertyMetadata(32.0));

		// Token: 0x04000934 RID: 2356
		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(NavigationView), new PropertyMetadata(null, new PropertyChangedCallback(NavigationView.OnContentPropertyChanged)));

		// Token: 0x04000935 RID: 2357
		public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(NavigationView), new PropertyMetadata(null));

		// Token: 0x04000936 RID: 2358
		public static readonly DependencyProperty PaneBackgroundProperty = DependencyProperty.Register("PaneBackground", typeof(Brush), typeof(NavigationView), new PropertyMetadata(null));

		// Token: 0x04000937 RID: 2359
		public static readonly DependencyProperty MinContentWidthProperty = DependencyProperty.Register("MinContentWidth", typeof(double), typeof(NavigationView), new PropertyMetadata(0.0));

		// Token: 0x04000938 RID: 2360
		public static readonly DependencyProperty MaxContentWidthProperty = DependencyProperty.Register("MaxContentWidth", typeof(double), typeof(NavigationView), new PropertyMetadata(double.MaxValue));

		// Token: 0x04000939 RID: 2361
		public static readonly DependencyProperty ContentWidthProperty = DependencyProperty.Register("ContentWidth", typeof(double), typeof(NavigationView), new PropertyMetadata(240.0));

		// Token: 0x0400093A RID: 2362
		public static readonly DependencyProperty IsClosedProperty = DependencyProperty.Register("IsClosed", typeof(bool), typeof(NavigationView), new PropertyMetadata(true, new PropertyChangedCallback(NavigationView.OnIsClosedPropertyChanged)));

		// Token: 0x0400093B RID: 2363
		public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(NavigationViewDirection), typeof(NavigationView), new PropertyMetadata(NavigationViewDirection.Left, new PropertyChangedCallback(NavigationView.OnDirectionPropertyChanged)));

		// Token: 0x0400093C RID: 2364
		public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(NavigationView), new PropertyMetadata(true));

		// Token: 0x0400093D RID: 2365
		private Thumb resizeDragger;

		// Token: 0x0400093E RID: 2366
		private double oldWidth = -1.0;
	}
}
