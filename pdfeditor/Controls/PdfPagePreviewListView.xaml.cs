using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace pdfeditor.Controls
{
	// Token: 0x020001D8 RID: 472
	public partial class PdfPagePreviewListView : ListBox
	{
		// Token: 0x06001ABC RID: 6844 RVA: 0x0006B415 File Offset: 0x00069615
		static PdfPagePreviewListView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewListView), new FrameworkPropertyMetadata(typeof(PdfPagePreviewListView)));
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x0006B43A File Offset: 0x0006963A
		public PdfPagePreviewListView()
		{
			base.Loaded += this.PdfPagePreviewListView_Loaded;
			base.Unloaded += this.PdfPagePreviewListView_Unloaded;
			base.SizeChanged += this.PdfPagePreviewListView_SizeChanged;
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x06001ABE RID: 6846 RVA: 0x0006B478 File Offset: 0x00069678
		protected virtual double ViewportThreshold
		{
			get
			{
				return 1870.0;
			}
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x0006B483 File Offset: 0x00069683
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PdfPagePreviewListViewItem;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x0006B48E File Offset: 0x0006968E
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PdfPagePreviewListViewItem();
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x0006B495 File Offset: 0x00069695
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.itemHost = null;
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x0006B4A4 File Offset: 0x000696A4
		private void PdfPagePreviewListView_Loaded(object sender, RoutedEventArgs e)
		{
			if (base.SelectedItem != null)
			{
				this.ScrollIntoView(base.SelectedItem);
			}
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x0006B4BA File Offset: 0x000696BA
		private void PdfPagePreviewListView_Unloaded(object sender, RoutedEventArgs e)
		{
			this.itemHost = null;
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x0006B4C4 File Offset: 0x000696C4
		private void PdfPagePreviewListView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.itemHost == null)
			{
				ItemsPresenter itemsPresenter = base.GetTemplateChild("ItemPresenter") as ItemsPresenter;
				if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
				{
					Panel panel = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
					if (panel != null)
					{
						this.itemHost = panel;
					}
				}
			}
			Panel panel2 = this.itemHost;
			if (panel2 == null)
			{
				return;
			}
			panel2.InvalidateMeasure();
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x0006B520 File Offset: 0x00069720
		public new void ScrollIntoView(object item)
		{
			if (item == null)
			{
				return;
			}
			ItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
			if (!base.IsVisible || itemContainerGenerator == null)
			{
				base.ScrollIntoView(item);
				return;
			}
			FrameworkElement container = itemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
			if (container == null || container.ActualWidth == 0.0 || container.ActualHeight == 0.0)
			{
				VisualStateManager.GoToState(this, "Scrolling", true);
				base.ScrollIntoView(item);
				try
				{
					base.Dispatcher.Invoke(delegate
					{
						VisualStateManager.GoToState(this, "NotScrolling", true);
					}, DispatcherPriority.Background);
					return;
				}
				catch
				{
					return;
				}
			}
			double viewportThreshold = this.ViewportThreshold;
			Rect rect = new Rect(-viewportThreshold, -viewportThreshold, base.ActualWidth + viewportThreshold * 2.0, base.ActualHeight + viewportThreshold * 2.0);
			Rect rect2 = container.TransformToVisual(this).TransformBounds(new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight));
			if (rect.IntersectsWith(rect2))
			{
				FrameworkElement container2 = container;
				if (container2 == null)
				{
					return;
				}
				container2.BringIntoView();
				return;
			}
			else
			{
				VisualStateManager.GoToState(this, "Scrolling", true);
				container.BringIntoView();
				try
				{
					base.Dispatcher.Invoke(delegate
					{
						container.BringIntoView();
						VisualStateManager.GoToState(this, "NotScrolling", true);
					}, DispatcherPriority.Background);
				}
				catch
				{
				}
			}
		}

		// Token: 0x04000952 RID: 2386
		private Panel itemHost;
	}
}
