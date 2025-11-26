using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using pdfeditor.Models.Bookmarks;

namespace pdfeditor.Controls.Bookmarks
{
	// Token: 0x02000298 RID: 664
	internal sealed partial class BookmarkTreeView : TreeView
	{
		// Token: 0x06002627 RID: 9767 RVA: 0x000B2254 File Offset: 0x000B0454
		static BookmarkTreeView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BookmarkTreeView), new FrameworkPropertyMetadata(typeof(BookmarkTreeView)));
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000B22B2 File Offset: 0x000B04B2
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06002629 RID: 9769 RVA: 0x000B22BA File Offset: 0x000B04BA
		// (set) Token: 0x0600262A RID: 9770 RVA: 0x000B22CC File Offset: 0x000B04CC
		public bool CanDragItems
		{
			get
			{
				return (bool)base.GetValue(BookmarkTreeView.CanDragItemsProperty);
			}
			set
			{
				base.SetValue(BookmarkTreeView.CanDragItemsProperty, value);
			}
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000B22DF File Offset: 0x000B04DF
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
			if (e.ChangedButton == MouseButton.Left && e.OriginalSource is ScrollViewer)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate
				{
					BookmarkModel bookmarkModel = base.SelectedItem as BookmarkModel;
					if (bookmarkModel != null)
					{
						bookmarkModel.IsSelected = false;
					}
				}));
			}
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x000B2316 File Offset: 0x000B0516
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is BookmarkTreeViewItem;
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x000B2321 File Offset: 0x000B0521
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new BookmarkTreeViewItem();
		}

		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x0600262E RID: 9774 RVA: 0x000B2328 File Offset: 0x000B0528
		private BookmarkControl ParentBookmarkControl
		{
			get
			{
				FrameworkElement frameworkElement = this;
				while (frameworkElement != null && !(frameworkElement is BookmarkControl))
				{
					frameworkElement = (frameworkElement.Parent ?? VisualTreeHelper.GetParent(frameworkElement)) as FrameworkElement;
				}
				return frameworkElement as BookmarkControl;
			}
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x000B2360 File Offset: 0x000B0560
		protected override void OnGotFocus(RoutedEventArgs e)
		{
		}

		// Token: 0x04001072 RID: 4210
		public static readonly DependencyProperty CanDragItemsProperty = DependencyProperty.Register("CanDragItems", typeof(bool), typeof(BookmarkTreeView), new PropertyMetadata(false));
	}
}
