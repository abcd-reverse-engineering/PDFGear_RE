using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Utils;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Bookmarks
{
	// Token: 0x02000295 RID: 661
	public sealed partial class BookmarkControl : Control
	{
		// Token: 0x060025F8 RID: 9720 RVA: 0x000B11C8 File Offset: 0x000AF3C8
		static BookmarkControl()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BookmarkControl), new FrameworkPropertyMetadata(typeof(BookmarkControl)));
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x060025FA RID: 9722 RVA: 0x000B12A3 File Offset: 0x000AF4A3
		// (set) Token: 0x060025FB RID: 9723 RVA: 0x000B12AC File Offset: 0x000AF4AC
		private BookmarkTreeView BookmarkTreeView
		{
			get
			{
				return this.bookmarkTreeView;
			}
			set
			{
				if (this.bookmarkTreeView == value)
				{
					return;
				}
				if (this.bookmarkTreeView != null)
				{
					this.bookmarkTreeView.SelectedItemChanged -= this.BookmarkTreeView_SelectedItemChanged;
				}
				this.bookmarkTreeView = value;
				if (this.bookmarkTreeView != null)
				{
					this.bookmarkTreeView.SelectedItemChanged += this.BookmarkTreeView_SelectedItemChanged;
					ItemsControl itemsControl = this.bookmarkTreeView;
					BookmarkModel bookmarkModel = this.bookmarkSource;
					itemsControl.ItemsSource = ((bookmarkModel != null) ? bookmarkModel.Children : null);
				}
				this.UpdateHighlightBookmark();
			}
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x000B132C File Offset: 0x000AF52C
		private void BookmarkTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			this.SelectedItem = e.NewValue as BookmarkModel;
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000B133F File Offset: 0x000AF53F
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.BookmarkTreeView = base.GetTemplateChild("PART_BookmarkTreeView") as BookmarkTreeView;
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000B1360 File Offset: 0x000AF560
		private void UpdateBookmarks()
		{
			try
			{
				BookmarkModel bookmarkModel = this.lastHighlightBookmark;
				IntPtr? intPtr;
				if (bookmarkModel == null)
				{
					intPtr = null;
				}
				else
				{
					PdfBookmark rawBookmark = bookmarkModel.RawBookmark;
					intPtr = ((rawBookmark != null) ? new IntPtr?(rawBookmark.Handle) : null);
				}
				IntPtr? intPtr2 = intPtr;
				if (this.lastHighlightBookmark != null)
				{
					this.lastHighlightBookmark.IsHighlighted = false;
				}
				this.lastHighlightBookmark = null;
				if (this.Bookmarks == null)
				{
					this.bookmarkLeaves = null;
					this.bookmarkSource = null;
				}
				else
				{
					BookmarkModel bookmarkModel2 = this.bookmarkSource;
					if (bookmarkModel2 != null)
					{
						WeakEventManager<BookmarkModel, EventArgs>.RemoveHandler(bookmarkModel2, "ChildrenChanged", new EventHandler<EventArgs>(this.Bookmarks_ChildrenChanged));
					}
					this.bookmarkSource = this.Bookmarks;
					if (this.bookmarkSource != null)
					{
						WeakEventManager<BookmarkModel, EventArgs>.AddHandler(this.bookmarkSource, "ChildrenChanged", new EventHandler<EventArgs>(this.Bookmarks_ChildrenChanged));
					}
					this.UpdateBookmarkLeaves();
					if (this.bookmarkSource != bookmarkModel2)
					{
						this.UpdateExpandState(this.bookmarkSource.Children, (bookmarkModel2 != null) ? bookmarkModel2.Children : null);
					}
					if (intPtr2 != null)
					{
						BookmarkModel bookmarkModel3 = BookmarkControl.FindBookmarkModel(this.bookmarkSource.Children, intPtr2.Value);
						if (bookmarkModel3 != null)
						{
							this.lastHighlightBookmark = bookmarkModel3;
							bookmarkModel3.IsHighlighted = true;
						}
					}
				}
				if (this.BookmarkTreeView != null)
				{
					ItemsControl itemsControl = this.BookmarkTreeView;
					BookmarkModel bookmarkModel4 = this.bookmarkSource;
					itemsControl.ItemsSource = ((bookmarkModel4 != null) ? bookmarkModel4.Children : null);
				}
			}
			catch (OperationCanceledException)
			{
				this.bookmarkLeaves = null;
				this.bookmarkSource = null;
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x000B14DC File Offset: 0x000AF6DC
		private void Bookmarks_ChildrenChanged(object sender, EventArgs e)
		{
			this.UpdateBookmarkLeaves();
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x000B14E4 File Offset: 0x000AF6E4
		private void UpdateBookmarkLeaves()
		{
			try
			{
				if (this.bookmarkSource != null)
				{
					this.bookmarkLeaves = this.CreateLeaves(this.bookmarkSource.Children);
					return;
				}
			}
			catch
			{
			}
			this.bookmarkLeaves = null;
			this.UpdateHighlightBookmark();
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x000B1538 File Offset: 0x000AF738
		private void UpdateExpandState(global::System.Collections.Generic.IReadOnlyList<BookmarkModel> bookmarks, global::System.Collections.Generic.IReadOnlyList<BookmarkModel> oldBookmarks)
		{
			if (bookmarks == null || bookmarks.Count == 0)
			{
				return;
			}
			if (oldBookmarks == null || oldBookmarks.Count == 0)
			{
				return;
			}
			Dictionary<IntPtr, global::System.ValueTuple<bool, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>>> dictionary = oldBookmarks.ToDictionary((BookmarkModel c) => c.RawBookmark.Handle, (BookmarkModel c) => new global::System.ValueTuple<bool, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>>(c.IsExpanded, c.Children));
			foreach (BookmarkModel bookmarkModel in bookmarks)
			{
				global::System.ValueTuple<bool, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>> valueTuple;
				if (dictionary.TryGetValue(bookmarkModel.RawBookmark.Handle, out valueTuple))
				{
					bookmarkModel.IsExpanded = valueTuple.Item1;
					if (bookmarkModel.Children != null && bookmarkModel.Children.Count > 0)
					{
						this.UpdateExpandState(bookmarkModel.Children, valueTuple.Item2);
					}
				}
			}
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x000B1620 File Offset: 0x000AF820
		public void ExpandAll()
		{
			this.SetAllIsExpanded(true);
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x000B1629 File Offset: 0x000AF829
		public void CollapseAll()
		{
			this.SetAllIsExpanded(false);
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x000B1634 File Offset: 0x000AF834
		private void SetAllIsExpanded(bool isExpanded)
		{
			if (this.bookmarkSource != null)
			{
				HashSet<BookmarkModel> hashSet = new HashSet<BookmarkModel>();
				BookmarkModel selectedItem = this.BookmarkTreeView.SelectedItem as BookmarkModel;
				if (!isExpanded)
				{
					for (BookmarkModel bookmarkModel = selectedItem; bookmarkModel != null; bookmarkModel = bookmarkModel.Parent)
					{
						hashSet.Add(bookmarkModel);
					}
				}
				foreach (BookmarkModel bookmarkModel2 in this.bookmarkSource.Children)
				{
					BookmarkControl.<SetAllIsExpanded>g__SetIsExpandedCore|18_0(bookmarkModel2, isExpanded, hashSet);
				}
				if (selectedItem != null)
				{
					base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						this.ScrollIntoViewAsync(selectedItem);
					}));
				}
			}
		}

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06002605 RID: 9733 RVA: 0x000B16F8 File Offset: 0x000AF8F8
		// (set) Token: 0x06002606 RID: 9734 RVA: 0x000B170A File Offset: 0x000AF90A
		public BookmarkModel SelectedItem
		{
			get
			{
				return (BookmarkModel)base.GetValue(BookmarkControl.SelectedItemProperty);
			}
			set
			{
				base.SetValue(BookmarkControl.SelectedItemProperty, value);
			}
		}

		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06002607 RID: 9735 RVA: 0x000B1718 File Offset: 0x000AF918
		// (set) Token: 0x06002608 RID: 9736 RVA: 0x000B172A File Offset: 0x000AF92A
		public BookmarkModel Bookmarks
		{
			get
			{
				return (BookmarkModel)base.GetValue(BookmarkControl.BookmarksProperty);
			}
			set
			{
				base.SetValue(BookmarkControl.BookmarksProperty, value);
			}
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x000B1738 File Offset: 0x000AF938
		private static async void OnBookmarksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				BookmarkControl bookmarkControl = d as BookmarkControl;
				if (bookmarkControl != null)
				{
					bookmarkControl.UpdateBookmarks();
					if (!bookmarkControl.UpdateHighlightBookmark() && bookmarkControl.lastHighlightBookmark != null)
					{
						await bookmarkControl.ScrollIntoViewAsync(bookmarkControl.lastHighlightBookmark);
					}
				}
			}
		}

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x0600260A RID: 9738 RVA: 0x000B1777 File Offset: 0x000AF977
		// (set) Token: 0x0600260B RID: 9739 RVA: 0x000B1789 File Offset: 0x000AF989
		public PdfViewer PdfViewer
		{
			get
			{
				return (PdfViewer)base.GetValue(BookmarkControl.PdfViewerProperty);
			}
			set
			{
				base.SetValue(BookmarkControl.PdfViewerProperty, value);
			}
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000B1798 File Offset: 0x000AF998
		private static void OnPdfViewerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BookmarkControl bookmarkControl = d as BookmarkControl;
			if (bookmarkControl != null && !object.Equals(e.NewValue, e.OldValue))
			{
				bookmarkControl.UpdatePdfViewer();
				bookmarkControl.UpdateHighlightBookmark();
			}
		}

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x0600260D RID: 9741 RVA: 0x000B17D4 File Offset: 0x000AF9D4
		// (remove) Token: 0x0600260E RID: 9742 RVA: 0x000B180C File Offset: 0x000AFA0C
		public event BookmarkControlSelectionChangedEventHandler SelectionChanged;

		// Token: 0x0600260F RID: 9743 RVA: 0x000B1841 File Offset: 0x000AFA41
		private void OnSelectionChanged(BookmarkModel bookmark)
		{
			BookmarkControlSelectionChangedEventHandler selectionChanged = this.SelectionChanged;
			if (selectionChanged == null)
			{
				return;
			}
			selectionChanged(this, new BookmarkControlSelectionChangedEventArgs(bookmark));
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000B185C File Offset: 0x000AFA5C
		private void UpdatePdfViewer()
		{
			BookmarkControl.ScrollViewerHelper scrollViewerHelper = this.pdfViewerScrollHelper;
			if (scrollViewerHelper != null)
			{
				scrollViewerHelper.Dispose();
			}
			this.pdfViewerScrollHelper = null;
			if (this.PdfViewer != null)
			{
				this.pdfViewerScrollHelper = new BookmarkControl.ScrollViewerHelper(this.PdfViewer);
				this.pdfViewerScrollHelper.DelayScrollChanged += this.<UpdatePdfViewer>g__ScrollViewerHelper_DelayScrollChanged|37_0;
			}
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x000B18B4 File Offset: 0x000AFAB4
		private bool UpdateHighlightBookmark()
		{
			if (this.BookmarkTreeView == null)
			{
				return false;
			}
			BookmarkControl.ScrollViewerHelper scrollViewerHelper = this.pdfViewerScrollHelper;
			if (((scrollViewerHelper != null) ? scrollViewerHelper.ScrollViewer : null) == null)
			{
				return false;
			}
			PdfViewer pdfViewer = this.PdfViewer;
			if (pdfViewer == null)
			{
				return false;
			}
			if (this.Bookmarks == null)
			{
				return false;
			}
			BookmarkModel bookmarkModel = null;
			double actualHeight = pdfViewer.ActualHeight;
			if (this.bookmarkLeaves != null && this.bookmarkLeaves.Count > 0)
			{
				global::System.ValueTuple<int, int> visiblePageRange = pdfViewer.GetVisiblePageRange();
				int item = visiblePageRange.Item1;
				int item2 = visiblePageRange.Item2;
				double num = double.NaN;
				BookmarkModel bookmarkModel2 = null;
				double num2 = double.NaN;
				for (int i = item; i <= item2; i++)
				{
					global::System.Collections.Generic.IReadOnlyList<BookmarkModel> readOnlyList;
					if (this.bookmarkLeaves.TryGetValue(i, out readOnlyList))
					{
						bool flag = false;
						for (int j = 0; j < readOnlyList.Count; j++)
						{
							if (readOnlyList[j].Position != null)
							{
								Point value = readOnlyList[j].Position.Value;
								Point point = pdfViewer.PageToClient(i, value);
								double num3 = point.X * point.X + point.Y * point.Y;
								if (num3 < num || double.IsNaN(num))
								{
									num = num3;
									bookmarkModel = readOnlyList[j];
								}
							}
							else if (!flag)
							{
								PdfPage pdfPage = pdfViewer.Document.Pages[i];
								FS_SIZEF effectiveSize = pdfPage.GetEffectiveSize(pdfPage.Rotation, false);
								Point point2 = pdfViewer.PageToClient(i, new Point(0.0, 0.0));
								Point point3 = pdfViewer.PageToClient(i, new Point((double)effectiveSize.Width, (double)effectiveSize.Height));
								Point point4 = new Point(Math.Min(point2.X, point3.X), Math.Min(point2.Y, point3.Y));
								double num4 = point4.X * point4.X + point4.Y * point4.Y;
								if (num4 < num2 || double.IsNaN(num2))
								{
									num2 = num4;
									bookmarkModel2 = readOnlyList[j];
								}
							}
						}
					}
				}
				if (bookmarkModel == null)
				{
					bookmarkModel = bookmarkModel2;
				}
			}
			if (this.lastHighlightBookmark != bookmarkModel)
			{
				if (bookmarkModel != null)
				{
					if (this.lastHighlightBookmark != null)
					{
						this.lastHighlightBookmark.IsHighlighted = false;
					}
					this.lastHighlightBookmark = bookmarkModel;
					bookmarkModel.IsHighlighted = true;
					this.ScrollIntoViewAsync(bookmarkModel);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x000B1B28 File Offset: 0x000AFD28
		internal void UpdateSelectedPageIndex()
		{
			PdfViewer pdfViewer = this.PdfViewer;
			if (pdfViewer == null)
			{
				return;
			}
			if (this.Bookmarks == null)
			{
				return;
			}
			if (this.BookmarkTreeView == null)
			{
				return;
			}
			BookmarkModel bookmarkModel = this.BookmarkTreeView.SelectedItem as BookmarkModel;
			if (bookmarkModel != null && bookmarkModel.PageIndex >= 0)
			{
				bool flag = false;
				if (bookmarkModel.Position != null)
				{
					Point value = bookmarkModel.Position.Value;
					double num;
					double num2;
					Pdfium.FPDF_GetPageSizeByIndex(pdfViewer.Document.Handle, bookmarkModel.PageIndex, out num, out num2);
					if (Math.Abs(value.X) < num && Math.Abs(value.Y) < num2)
					{
						flag = true;
						pdfViewer.ScrollToPoint(bookmarkModel.PageIndex, value);
					}
				}
				if (!flag)
				{
					pdfViewer.ScrollToPage(bookmarkModel.PageIndex);
				}
			}
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000B1BF0 File Offset: 0x000AFDF0
		private async Task ScrollIntoViewAsync(BookmarkModel model)
		{
			if (this.BookmarkTreeView != null)
			{
				await this.BookmarkTreeView.ScrollIntoViewAsync(model, ScrollIntoViewOrientation.Vertical).ConfigureAwait(false);
			}
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x000B1C3B File Offset: 0x000AFE3B
		public void ScrollIntoView(BookmarkModel item)
		{
			this.ScrollIntoViewAsync(item);
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x000B1C48 File Offset: 0x000AFE48
		private static BookmarkModel FindBookmarkModel(global::System.Collections.Generic.IReadOnlyList<BookmarkModel> bookmarks, IntPtr bookmarkHandle)
		{
			if (bookmarkHandle == IntPtr.Zero)
			{
				return null;
			}
			foreach (BookmarkModel bookmarkModel in bookmarks)
			{
				if (bookmarkModel == null)
				{
					goto IL_0068;
				}
				PdfBookmark rawBookmark = bookmarkModel.RawBookmark;
				if (!(((rawBookmark != null) ? new IntPtr?(rawBookmark.Handle) : null) == bookmarkHandle))
				{
					goto IL_0068;
				}
				BookmarkModel bookmarkModel2 = bookmarkModel;
				IL_0075:
				if (bookmarkModel2 != null)
				{
					return bookmarkModel2;
				}
				continue;
				IL_0068:
				bookmarkModel2 = BookmarkControl.FindBookmarkModel(bookmarkModel.Children, bookmarkHandle);
				goto IL_0075;
			}
			return null;
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x000B1CFC File Offset: 0x000AFEFC
		private IReadOnlyDictionary<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>> CreateLeaves(global::System.Collections.Generic.IReadOnlyList<BookmarkModel> bookmarks)
		{
			return new SortedDictionary<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>>((from c in bookmarks.SelectMany((BookmarkModel c) => BookmarkControl.<CreateLeaves>g__Flatten|43_6(c))
				group c by c.PageIndex into c
				select new global::System.ValueTuple<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>>(c.Key, c.OrderByDescending((BookmarkModel x) => x.Level).ToList<BookmarkModel>()) into c
				where c.Item2 != null
				select c).ToDictionary(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "key", "value" })] global::System.ValueTuple<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>> c) => c.Item1, ([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "key", "value" })] global::System.ValueTuple<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>> c) => c.Item2));
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x000B1DE4 File Offset: 0x000AFFE4
		private async Task ApplyScrollPositionSnapshotAsync(BookmarkControl.ScrollPositionSnapshot snapshot)
		{
			BookmarkControl.<>c__DisplayClass44_0 CS$<>8__locals1 = new BookmarkControl.<>c__DisplayClass44_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.snapshot = snapshot;
			BookmarkControl.ScrollPositionSnapshot snapshot2 = CS$<>8__locals1.snapshot;
			if (snapshot2 != null && snapshot2.IsValid && this.bookmarkSource != null && this.bookmarkSource.Children.Count > 0)
			{
				BookmarkControl.<>c__DisplayClass44_1 CS$<>8__locals2 = new BookmarkControl.<>c__DisplayClass44_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.model = BookmarkControl.FindBookmarkModel(this.bookmarkSource.Children, CS$<>8__locals2.CS$<>8__locals1.snapshot.RawBookmarkHandle);
				if (CS$<>8__locals2.model != null)
				{
					await this.ScrollIntoViewAsync(CS$<>8__locals2.model);
					Action func = delegate
					{
						BookmarkTreeViewItem bookmarkTreeViewItem = CS$<>8__locals2.CS$<>8__locals1.<ApplyScrollPositionSnapshotAsync>g__GetContainerFromItem|1(CS$<>8__locals2.model);
						if (bookmarkTreeViewItem != null)
						{
							FrameworkElement frameworkElement = VisualTreeHelper.GetChild(CS$<>8__locals2.CS$<>8__locals1.<>4__this.BookmarkTreeView, 0) as FrameworkElement;
							ScrollViewer scrollViewer = ((frameworkElement != null) ? frameworkElement.FindName("_tv_scrollviewer_") : null) as ScrollViewer;
							if (scrollViewer != null)
							{
								Point point = bookmarkTreeViewItem.TransformToVisual(scrollViewer).Transform(default(Point));
								double num = scrollViewer.HorizontalOffset + CS$<>8__locals2.CS$<>8__locals1.snapshot.ItemBounds.Left - point.X;
								double num2 = scrollViewer.VerticalOffset + CS$<>8__locals2.CS$<>8__locals1.snapshot.ItemBounds.Top - point.Y;
								scrollViewer.ScrollToHorizontalOffset(num);
								scrollViewer.ScrollToVerticalOffset(num2);
								scrollViewer.UpdateLayout();
							}
						}
					};
					await base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, func);
					await base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, func);
					func = null;
				}
				CS$<>8__locals2 = null;
			}
		}

		// Token: 0x06002618 RID: 9752 RVA: 0x000B1E30 File Offset: 0x000B0030
		private BookmarkControl.ScrollPositionSnapshot TakeScrollPositionSnapshot()
		{
			if (this.BookmarkTreeView == null || VisualTreeHelper.GetChildrenCount(this.BookmarkTreeView) == 0)
			{
				return null;
			}
			FrameworkElement frameworkElement = VisualTreeHelper.GetChild(this.BookmarkTreeView, 0) as FrameworkElement;
			BookmarkControl.<>c__DisplayClass45_0 CS$<>8__locals1;
			CS$<>8__locals1.scrollViewer = ((frameworkElement != null) ? frameworkElement.FindName("_tv_scrollviewer_") : null) as ScrollViewer;
			if (CS$<>8__locals1.scrollViewer == null)
			{
				return null;
			}
			CS$<>8__locals1.topElement = null;
			CS$<>8__locals1.topBounds = null;
			BookmarkControl.<TakeScrollPositionSnapshot>g__FindTopElement|45_0(this.BookmarkTreeView, ref CS$<>8__locals1);
			if (CS$<>8__locals1.topBounds != null)
			{
				FrameworkElement topElement = CS$<>8__locals1.topElement;
				BookmarkModel bookmarkModel = ((topElement != null) ? topElement.DataContext : null) as BookmarkModel;
				if (bookmarkModel != null && bookmarkModel.RawBookmark != null && bookmarkModel.RawBookmark.Handle != IntPtr.Zero)
				{
					return new BookmarkControl.ScrollPositionSnapshot(bookmarkModel.RawBookmark.Handle, CS$<>8__locals1.topBounds.Value);
				}
			}
			return null;
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x000B1F14 File Offset: 0x000B0114
		public bool CheckIfCollapseAll()
		{
			return this.bookmarkSource.Children.All((BookmarkModel item) => this.CheckIfAllExpanded(item));
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x000B1F34 File Offset: 0x000B0134
		private bool CheckIfAllExpanded(BookmarkModel item)
		{
			if (!item.IsExpanded && item.Children.Count != 0)
			{
				return false;
			}
			if (item.Children != null && item.Children.Count > 0)
			{
				foreach (BookmarkModel bookmarkModel in item.Children)
				{
					if (!this.CheckIfAllExpanded(bookmarkModel))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000B1FB8 File Offset: 0x000B01B8
		[CompilerGenerated]
		internal static void <SetAllIsExpanded>g__SetIsExpandedCore|18_0(BookmarkModel _model, bool _isExpanded, HashSet<BookmarkModel> _expandedItems)
		{
			if (_model != null && _model.Children != null && _model.Children.Count > 0)
			{
				if (_isExpanded || !_expandedItems.Contains(_model))
				{
					_model.IsExpanded = _isExpanded;
				}
				foreach (BookmarkModel bookmarkModel in _model.Children)
				{
					BookmarkControl.<SetAllIsExpanded>g__SetIsExpandedCore|18_0(bookmarkModel, _isExpanded, _expandedItems);
				}
			}
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x000B2030 File Offset: 0x000B0230
		[CompilerGenerated]
		private void <UpdatePdfViewer>g__ScrollViewerHelper_DelayScrollChanged|37_0(object sender, EventArgs e)
		{
			this.UpdateHighlightBookmark();
		}

		// Token: 0x0600261D RID: 9757 RVA: 0x000B203C File Offset: 0x000B023C
		[CompilerGenerated]
		internal static IEnumerable<BookmarkModel> <CreateLeaves>g__Flatten|43_6(BookmarkModel _model)
		{
			if (_model == null)
			{
				return Enumerable.Empty<BookmarkModel>();
			}
			if (_model.Children != null && _model.Children.Count > 0)
			{
				return new BookmarkModel[] { _model }.Concat(_model.Children.SelectMany((BookmarkModel c) => BookmarkControl.<CreateLeaves>g__Flatten|43_6(c)));
			}
			return new BookmarkModel[] { _model };
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x000B20AC File Offset: 0x000B02AC
		[CompilerGenerated]
		internal static void <TakeScrollPositionSnapshot>g__FindTopElement|45_0(ItemsControl _itemsControl, ref BookmarkControl.<>c__DisplayClass45_0 A_1)
		{
			Panel panel = BookmarkControl.<TakeScrollPositionSnapshot>g__GetItemsControlPanel|45_1(_itemsControl);
			if (panel == null)
			{
				return;
			}
			foreach (BookmarkTreeViewItem bookmarkTreeViewItem in panel.Children.OfType<BookmarkTreeViewItem>())
			{
				BookmarkModel bookmarkModel = bookmarkTreeViewItem.DataContext as BookmarkModel;
				if (bookmarkModel != null)
				{
					Rect rect = bookmarkTreeViewItem.TransformToVisual(A_1.scrollViewer).TransformBounds(new Rect(0.0, 0.0, bookmarkTreeViewItem.ActualWidth, bookmarkTreeViewItem.ActualHeight));
					if (rect.Top > 0.0 && ((A_1.topBounds != null && A_1.topBounds.Value.Top > rect.Top) || A_1.topBounds == null))
					{
						A_1.topBounds = new Rect?(rect);
						A_1.topElement = bookmarkTreeViewItem;
					}
					if (bookmarkModel.Children.Count > 0)
					{
						BookmarkControl.<TakeScrollPositionSnapshot>g__FindTopElement|45_0(bookmarkTreeViewItem, ref A_1);
					}
				}
			}
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x000B21C4 File Offset: 0x000B03C4
		[CompilerGenerated]
		internal static Panel <TakeScrollPositionSnapshot>g__GetItemsControlPanel|45_1(ItemsControl _itemsControl)
		{
			if (_itemsControl == null)
			{
				return null;
			}
			FrameworkElement frameworkElement = VisualTreeHelper.GetChild(_itemsControl, 0) as FrameworkElement;
			if (frameworkElement == null)
			{
				return null;
			}
			ItemsPresenter itemsPresenter = frameworkElement.FindName("ItemsHost") as ItemsPresenter;
			if (itemsPresenter == null)
			{
				itemsPresenter = (frameworkElement.FindName("_tv_scrollviewer_") as ScrollViewer).Content as ItemsPresenter;
			}
			if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
			{
				return VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
			}
			return null;
		}

		// Token: 0x04001068 RID: 4200
		private BookmarkModel bookmarkSource;

		// Token: 0x04001069 RID: 4201
		private IReadOnlyDictionary<int, global::System.Collections.Generic.IReadOnlyList<BookmarkModel>> bookmarkLeaves;

		// Token: 0x0400106A RID: 4202
		private BookmarkModel lastHighlightBookmark;

		// Token: 0x0400106B RID: 4203
		private BookmarkTreeView bookmarkTreeView;

		// Token: 0x0400106C RID: 4204
		private BookmarkControl.ScrollViewerHelper pdfViewerScrollHelper;

		// Token: 0x0400106D RID: 4205
		private static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(BookmarkModel), typeof(BookmarkControl), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			if (a.NewValue != a.OldValue)
			{
				BookmarkControl bookmarkControl = s as BookmarkControl;
				if (bookmarkControl != null)
				{
					BookmarkModel bookmarkModel = null;
					BookmarkModel bookmarkModel2 = a.OldValue as BookmarkModel;
					if (bookmarkModel2 != null)
					{
						bookmarkModel2.IsSelected = false;
					}
					BookmarkModel bookmarkModel3 = a.NewValue as BookmarkModel;
					if (bookmarkModel3 != null && bookmarkControl.bookmarkSource != null)
					{
						global::System.Collections.Generic.IReadOnlyList<BookmarkModel> children = bookmarkControl.bookmarkSource.Children;
						IntPtr? intPtr;
						if (bookmarkModel3 == null)
						{
							intPtr = null;
						}
						else
						{
							PdfBookmark rawBookmark = bookmarkModel3.RawBookmark;
							intPtr = ((rawBookmark != null) ? new IntPtr?(rawBookmark.Handle) : null);
						}
						IntPtr? intPtr2 = intPtr;
						bookmarkModel = BookmarkControl.FindBookmarkModel(children, intPtr2.GetValueOrDefault());
					}
					if (bookmarkModel != null)
					{
						bookmarkModel.IsSelected = true;
						bookmarkControl.ScrollIntoViewAsync(bookmarkModel);
					}
					BookmarkTreeView bookmarkTreeView = bookmarkControl.BookmarkTreeView;
					BookmarkTreeViewItem bookmarkTreeViewItem = ((bookmarkTreeView != null) ? bookmarkTreeView.TreeViewItemFromElement(bookmarkModel) : null) as BookmarkTreeViewItem;
					if (bookmarkTreeViewItem == null || bookmarkTreeViewItem.ShouldUpdatePageIndex)
					{
						bookmarkControl.UpdateSelectedPageIndex();
					}
					bookmarkControl.OnSelectionChanged(bookmarkModel);
				}
			}
		}));

		// Token: 0x0400106E RID: 4206
		public static readonly DependencyProperty BookmarksProperty = DependencyProperty.Register("Bookmarks", typeof(BookmarkModel), typeof(BookmarkControl), new PropertyMetadata(null, new PropertyChangedCallback(BookmarkControl.OnBookmarksPropertyChanged)));

		// Token: 0x0400106F RID: 4207
		public static readonly DependencyProperty PdfViewerProperty = DependencyProperty.Register("PdfViewer", typeof(PdfViewer), typeof(BookmarkControl), new PropertyMetadata(null, new PropertyChangedCallback(BookmarkControl.OnPdfViewerPropertyChanged)));

		// Token: 0x0200075E RID: 1886
		private class ScrollPositionSnapshot
		{
			// Token: 0x060036DB RID: 14043 RVA: 0x00113E30 File Offset: 0x00112030
			public ScrollPositionSnapshot(IntPtr rawBookmarkHandle, Rect itemBounds)
			{
				if (rawBookmarkHandle == IntPtr.Zero)
				{
					throw new ArgumentException("rawBookmarkHandle");
				}
				if (itemBounds.IsEmpty)
				{
					throw new ArgumentException("itemBounds");
				}
				this.RawBookmarkHandle = rawBookmarkHandle;
				this.ItemBounds = itemBounds;
				this.CreateTime = DateTime.UtcNow;
			}

			// Token: 0x17000D98 RID: 3480
			// (get) Token: 0x060036DC RID: 14044 RVA: 0x00113E88 File Offset: 0x00112088
			public DateTime CreateTime { get; }

			// Token: 0x17000D99 RID: 3481
			// (get) Token: 0x060036DD RID: 14045 RVA: 0x00113E90 File Offset: 0x00112090
			public IntPtr RawBookmarkHandle { get; }

			// Token: 0x17000D9A RID: 3482
			// (get) Token: 0x060036DE RID: 14046 RVA: 0x00113E98 File Offset: 0x00112098
			public Rect ItemBounds { get; }

			// Token: 0x17000D9B RID: 3483
			// (get) Token: 0x060036DF RID: 14047 RVA: 0x00113EA0 File Offset: 0x001120A0
			public bool IsValid
			{
				get
				{
					return (DateTime.UtcNow - this.CreateTime).TotalSeconds < 10.0;
				}
			}
		}

		// Token: 0x0200075F RID: 1887
		private class ScrollViewerHelper : IDisposable
		{
			// Token: 0x060036E0 RID: 14048 RVA: 0x00113ED0 File Offset: 0x001120D0
			public ScrollViewerHelper(IScrollInfo scrollInfo)
			{
				this.scrollInfo = scrollInfo;
				this.EnsureScrollViewer(false);
				FrameworkElement frameworkElement = scrollInfo as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.Loaded += this.ScrollInfo_Loaded;
					frameworkElement.Unloaded += this.ScrollInfo_Unloaded;
				}
			}

			// Token: 0x17000D9C RID: 3484
			// (get) Token: 0x060036E1 RID: 14049 RVA: 0x00113F1F File Offset: 0x0011211F
			public ScrollViewer ScrollViewer
			{
				get
				{
					return this.scrollViewer;
				}
			}

			// Token: 0x060036E2 RID: 14050 RVA: 0x00113F27 File Offset: 0x00112127
			private void ScrollInfo_Loaded(object sender, RoutedEventArgs e)
			{
				if (((FrameworkElement)sender).IsLoaded)
				{
					this.RemoveScrollViewer();
					this.EnsureScrollViewer(true);
				}
			}

			// Token: 0x060036E3 RID: 14051 RVA: 0x00113F43 File Offset: 0x00112143
			private void ScrollInfo_Unloaded(object sender, RoutedEventArgs e)
			{
				if (!((FrameworkElement)sender).IsLoaded)
				{
					this.RemoveScrollViewer();
				}
			}

			// Token: 0x060036E4 RID: 14052 RVA: 0x00113F58 File Offset: 0x00112158
			private void EnsureScrollViewer(bool raiseLoaded)
			{
				if (this.scrollInfo == null)
				{
					return;
				}
				if (this.scrollViewer == null)
				{
					IScrollInfo scrollInfo = this.scrollInfo;
					this.scrollViewer = ((scrollInfo != null) ? scrollInfo.ScrollOwner : null);
					if (this.scrollViewer != null)
					{
						if (this.delayTimer == null)
						{
							this.delayTimer = new DispatcherTimer(DispatcherPriority.Normal)
							{
								Interval = TimeSpan.FromMilliseconds(500.0)
							};
							this.delayTimer.Tick += delegate(object s, EventArgs a)
							{
								((DispatcherTimer)s).Stop();
								if (this.scrollViewer != null)
								{
									EventHandler delayScrollChanged2 = this.DelayScrollChanged;
									if (delayScrollChanged2 == null)
									{
										return;
									}
									delayScrollChanged2(this.scrollViewer, EventArgs.Empty);
								}
							};
						}
						this.scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
						this.scrollViewer.ScrollChanged += this.ScrollViewer_ScrollChanged;
						this.scrollViewer.Unloaded -= this.ScrollViewer_Unloaded;
						this.scrollViewer.Unloaded += this.ScrollViewer_Unloaded;
						if (this.scrollViewer.IsLoaded)
						{
							if (raiseLoaded)
							{
								EventHandler scrollViewerLoaded = this.ScrollViewerLoaded;
								if (scrollViewerLoaded != null)
								{
									scrollViewerLoaded(this.scrollViewer, EventArgs.Empty);
								}
								EventHandler delayScrollChanged = this.DelayScrollChanged;
								if (delayScrollChanged == null)
								{
									return;
								}
								delayScrollChanged(this.scrollViewer, EventArgs.Empty);
								return;
							}
						}
						else
						{
							this.scrollViewer.Loaded -= this.ScrollViewer_Loaded;
							this.scrollViewer.Loaded += this.ScrollViewer_Loaded;
						}
					}
				}
			}

			// Token: 0x060036E5 RID: 14053 RVA: 0x001140B0 File Offset: 0x001122B0
			private void RemoveScrollViewer()
			{
				ScrollViewer scrollViewer = this.scrollViewer;
				if (scrollViewer == null)
				{
					return;
				}
				this.scrollViewer = null;
				DispatcherTimer dispatcherTimer = this.delayTimer;
				if (dispatcherTimer != null)
				{
					dispatcherTimer.Stop();
				}
				scrollViewer.Loaded -= this.ScrollViewer_Loaded;
				scrollViewer.Unloaded -= this.ScrollViewer_Unloaded;
				scrollViewer.ScrollChanged -= this.ScrollViewer_ScrollChanged;
			}

			// Token: 0x060036E6 RID: 14054 RVA: 0x00114118 File Offset: 0x00112318
			private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
			{
				if (this.scrollViewer != null)
				{
					DateTime now = DateTime.Now;
					ScrollChangedEventHandler scrollChanged = this.ScrollChanged;
					if (scrollChanged != null)
					{
						scrollChanged(sender, e);
					}
					if (!this.delayTimer.IsEnabled || (now - this.lastScrollTime).TotalMilliseconds > 500.0)
					{
						this.lastScrollTime = now;
						EventHandler delayScrollChanged = this.DelayScrollChanged;
						if (delayScrollChanged != null)
						{
							delayScrollChanged(this.scrollViewer, EventArgs.Empty);
						}
					}
					this.delayTimer.Stop();
					this.delayTimer.Start();
				}
			}

			// Token: 0x060036E7 RID: 14055 RVA: 0x001141AB File Offset: 0x001123AB
			private void ScrollViewer_Unloaded(object sender, RoutedEventArgs e)
			{
				this.RemoveScrollViewer();
			}

			// Token: 0x060036E8 RID: 14056 RVA: 0x001141B4 File Offset: 0x001123B4
			private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
			{
				((FrameworkElement)sender).Loaded -= this.ScrollViewer_Loaded;
				if (this.scrollViewer != null)
				{
					EventHandler scrollViewerLoaded = this.ScrollViewerLoaded;
					if (scrollViewerLoaded != null)
					{
						scrollViewerLoaded(this.scrollViewer, EventArgs.Empty);
					}
					EventHandler delayScrollChanged = this.DelayScrollChanged;
					if (delayScrollChanged == null)
					{
						return;
					}
					delayScrollChanged(this.scrollViewer, EventArgs.Empty);
				}
			}

			// Token: 0x14000051 RID: 81
			// (add) Token: 0x060036E9 RID: 14057 RVA: 0x00114218 File Offset: 0x00112418
			// (remove) Token: 0x060036EA RID: 14058 RVA: 0x00114250 File Offset: 0x00112450
			public event EventHandler ScrollViewerLoaded;

			// Token: 0x14000052 RID: 82
			// (add) Token: 0x060036EB RID: 14059 RVA: 0x00114288 File Offset: 0x00112488
			// (remove) Token: 0x060036EC RID: 14060 RVA: 0x001142C0 File Offset: 0x001124C0
			public event ScrollChangedEventHandler ScrollChanged;

			// Token: 0x14000053 RID: 83
			// (add) Token: 0x060036ED RID: 14061 RVA: 0x001142F8 File Offset: 0x001124F8
			// (remove) Token: 0x060036EE RID: 14062 RVA: 0x00114330 File Offset: 0x00112530
			public event EventHandler DelayScrollChanged;

			// Token: 0x060036EF RID: 14063 RVA: 0x00114368 File Offset: 0x00112568
			public void Dispose()
			{
				FrameworkElement frameworkElement = this.scrollInfo as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.Loaded -= this.ScrollInfo_Loaded;
					frameworkElement.Unloaded -= this.ScrollInfo_Unloaded;
				}
				this.scrollInfo = null;
				DispatcherTimer dispatcherTimer = this.delayTimer;
				if (dispatcherTimer != null)
				{
					dispatcherTimer.Stop();
				}
				this.delayTimer = null;
				this.ScrollChanged = null;
				this.ScrollViewerLoaded = null;
				this.DelayScrollChanged = null;
				this.RemoveScrollViewer();
			}

			// Token: 0x04002558 RID: 9560
			private IScrollInfo scrollInfo;

			// Token: 0x04002559 RID: 9561
			private ScrollViewer scrollViewer;

			// Token: 0x0400255A RID: 9562
			private DispatcherTimer delayTimer;

			// Token: 0x0400255B RID: 9563
			private DateTime lastScrollTime;
		}
	}
}
