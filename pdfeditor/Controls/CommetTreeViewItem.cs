using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace pdfeditor.Controls
{
	// Token: 0x020001B4 RID: 436
	internal class CommetTreeViewItem : TreeViewItem
	{
		// Token: 0x060018DF RID: 6367 RVA: 0x0006036C File Offset: 0x0005E56C
		static CommetTreeViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommetTreeViewItem), new FrameworkPropertyMetadata(typeof(CommetTreeViewItem)));
			EventManager.RegisterClassHandler(typeof(CommetTreeViewItem), FrameworkElement.RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(CommetTreeViewItem.OnRequestBringIntoView));
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x000603BC File Offset: 0x0005E5BC
		private static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			if (e.TargetObject == sender)
			{
				e.Handled = true;
				ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer((DependencyObject)sender);
				FrameworkElement frameworkElement = ((VisualTreeHelper.GetChildrenCount((DependencyObject)sender) > 0) ? (VisualTreeHelper.GetChild((DependencyObject)sender, 0) as FrameworkElement) : null);
				if (frameworkElement != null)
				{
					Rect rect = new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight);
					if (itemsControl != null)
					{
						rect = new Rect(rect.Left - itemsControl.Margin.Left, rect.Top, rect.Width + itemsControl.Margin.Left, rect.Height);
					}
					frameworkElement.BringIntoView(rect);
				}
			}
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0006047E File Offset: 0x0005E67E
		public CommetTreeViewItem()
		{
			base.SizeChanged += this.CommetTreeViewItem_SizeChanged;
			base.DataContextChanged += this.CommetTreeViewItem_DataContextChanged;
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x060018E2 RID: 6370 RVA: 0x000604AA File Offset: 0x0005E6AA
		// (set) Token: 0x060018E3 RID: 6371 RVA: 0x000604B4 File Offset: 0x0005E6B4
		private ToggleButton Expander
		{
			get
			{
				return this.expander;
			}
			set
			{
				if (this.expander == value)
				{
					return;
				}
				if (this.expander != null)
				{
					this.expander.SizeChanged -= this.OnChildSizeChanged;
				}
				this.expander = value;
				if (this.expander != null)
				{
					this.expander.SizeChanged += this.OnChildSizeChanged;
				}
			}
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x00060510 File Offset: 0x0005E710
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Expander = base.GetTemplateChild("Expander") as ToggleButton;
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x0006052E File Offset: 0x0005E72E
		private void CommetTreeViewItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x00060530 File Offset: 0x0005E730
		private void OnChildSizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x00060532 File Offset: 0x0005E732
		private void CommetTreeViewItem_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x00060534 File Offset: 0x0005E734
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is CommetTreeViewItem;
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x0006053F File Offset: 0x0005E73F
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new CommetTreeViewItem();
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x00060548 File Offset: 0x0005E748
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			CommetTreeViewItem commetTreeViewItem = element as CommetTreeViewItem;
			if (commetTreeViewItem != null && commetTreeViewItem.IsSelected)
			{
				commetTreeViewItem.BringIntoView();
			}
			base.PrepareContainerForItemOverride(element, item);
		}

		// Token: 0x04000858 RID: 2136
		private ToggleButton expander;
	}
}
