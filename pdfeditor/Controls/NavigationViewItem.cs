using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls
{
	// Token: 0x020001D3 RID: 467
	public class NavigationViewItem : ListBoxItem
	{
		// Token: 0x06001A87 RID: 6791 RVA: 0x0006A6FC File Offset: 0x000688FC
		static NavigationViewItem()
		{
			NavigationViewItem.ItemClickedEvent = EventManager.RegisterRoutedEvent("ItemClicked", RoutingStrategy.Bubble, typeof(EventHandler<NavigationViewItemClickEventArgs>), typeof(NavigationViewItem));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItem), new FrameworkPropertyMetadata(typeof(NavigationViewItem)));
		}

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06001A88 RID: 6792 RVA: 0x0006A750 File Offset: 0x00068950
		// (remove) Token: 0x06001A89 RID: 6793 RVA: 0x0006A75E File Offset: 0x0006895E
		public event EventHandler<NavigationViewItemClickEventArgs> ItemClicked
		{
			add
			{
				base.AddHandler(NavigationViewItem.ItemClickedEvent, value);
			}
			remove
			{
				base.RemoveHandler(NavigationViewItem.ItemClickedEvent, value);
			}
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x0006A76C File Offset: 0x0006896C
		protected virtual void OnItemClickedEvent(DependencyObject element, object item)
		{
			NavigationViewItemClickEventArgs navigationViewItemClickEventArgs = new NavigationViewItemClickEventArgs(item, NavigationViewItem.ItemClickedEvent, this);
			base.RaiseEvent(navigationViewItemClickEventArgs);
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x0006A78D File Offset: 0x0006898D
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (base.Content != null)
			{
				this.OnItemClickedEvent(this, base.Content);
			}
			base.OnMouseLeftButtonDown(e);
		}
	}
}
