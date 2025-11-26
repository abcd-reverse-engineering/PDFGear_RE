using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf.Net;
using pdfeditor.Controls;

namespace pdfeditor.Models.Bookmarks
{
	// Token: 0x0200018F RID: 399
	public class BookmarkModel : ObservableObject, ITreeViewNode
	{
		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x060016F0 RID: 5872 RVA: 0x00057A72 File Offset: 0x00055C72
		// (set) Token: 0x060016F1 RID: 5873 RVA: 0x00057A7A File Offset: 0x00055C7A
		public virtual string Title
		{
			get
			{
				return this.title;
			}
			private set
			{
				base.SetProperty<string>(ref this.title, value, "Title");
			}
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x060016F2 RID: 5874 RVA: 0x00057A8F File Offset: 0x00055C8F
		// (set) Token: 0x060016F3 RID: 5875 RVA: 0x00057A97 File Offset: 0x00055C97
		public virtual int PageIndex { get; private set; }

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x060016F4 RID: 5876 RVA: 0x00057AA0 File Offset: 0x00055CA0
		// (set) Token: 0x060016F5 RID: 5877 RVA: 0x00057AA8 File Offset: 0x00055CA8
		public virtual Point? Position { get; private set; }

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x060016F6 RID: 5878 RVA: 0x00057AB1 File Offset: 0x00055CB1
		public global::System.Collections.Generic.IReadOnlyList<BookmarkModel> Children
		{
			get
			{
				return this.children;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x00057AB9 File Offset: 0x00055CB9
		// (set) Token: 0x060016F8 RID: 5880 RVA: 0x00057AC1 File Offset: 0x00055CC1
		public virtual PdfBookmark RawBookmark { get; private set; }

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x00057ACA File Offset: 0x00055CCA
		ITreeViewNode ITreeViewNode.Parent
		{
			get
			{
				if (this.IsTopLevelModel)
				{
					return null;
				}
				return this.Parent;
			}
		}

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x00057ADC File Offset: 0x00055CDC
		// (set) Token: 0x060016FB RID: 5883 RVA: 0x00057AE4 File Offset: 0x00055CE4
		public virtual BookmarkModel Parent
		{
			get
			{
				return this.parent;
			}
			private set
			{
				base.SetProperty<BookmarkModel>(ref this.parent, value, "Parent");
			}
		}

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x00057AF9 File Offset: 0x00055CF9
		public bool IsTopLevelModel
		{
			get
			{
				return this.Parent == null || this.Parent is BookmarkModel.RootBookmarkModel;
			}
		}

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x060016FD RID: 5885 RVA: 0x00057B13 File Offset: 0x00055D13
		public virtual int Level
		{
			get
			{
				BookmarkModel bookmarkModel = this.parent;
				return ((bookmarkModel != null) ? bookmarkModel.Level : (-1)) + 1;
			}
		}

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x060016FE RID: 5886 RVA: 0x00057B29 File Offset: 0x00055D29
		// (set) Token: 0x060016FF RID: 5887 RVA: 0x00057B31 File Offset: 0x00055D31
		public virtual bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				if (value)
				{
					this.ExpandToRoot();
				}
				base.SetProperty<bool>(ref this.isSelected, value, "IsSelected");
			}
		}

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x00057B4F File Offset: 0x00055D4F
		// (set) Token: 0x06001701 RID: 5889 RVA: 0x00057B57 File Offset: 0x00055D57
		public virtual bool IsExpanded
		{
			get
			{
				return this.isExpanded;
			}
			set
			{
				base.SetProperty<bool>(ref this.isExpanded, value, "IsExpanded");
			}
		}

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x00057B6C File Offset: 0x00055D6C
		// (set) Token: 0x06001703 RID: 5891 RVA: 0x00057B74 File Offset: 0x00055D74
		public virtual bool IsHighlighted
		{
			get
			{
				return this.isHighlighted;
			}
			set
			{
				if (value & base.SetProperty<bool>(ref this.isHighlighted, value, "IsHighlighted"))
				{
					this.ExpandToRoot();
				}
			}
		}

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06001704 RID: 5892 RVA: 0x00057B94 File Offset: 0x00055D94
		// (remove) Token: 0x06001705 RID: 5893 RVA: 0x00057BCC File Offset: 0x00055DCC
		public event EventHandler ChildrenChanged;

		// Token: 0x06001706 RID: 5894 RVA: 0x00057C01 File Offset: 0x00055E01
		public virtual bool UpdateTitle(string newTitle)
		{
			newTitle = newTitle ?? string.Empty;
			if (newTitle != this.title)
			{
				this.RawBookmark.Title = newTitle;
				this.Title = newTitle;
				return true;
			}
			return false;
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x00057C34 File Offset: 0x00055E34
		public void ExpandToRoot()
		{
			for (BookmarkModel bookmarkModel = this.Parent; bookmarkModel != null; bookmarkModel = bookmarkModel.Parent)
			{
				bookmarkModel.IsExpanded = true;
			}
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x00057C5C File Offset: 0x00055E5C
		private void NotifyChildrenChanged(bool raiseCollectionEvent)
		{
			if (raiseCollectionEvent)
			{
				this.children.NotifyReset();
			}
			EventHandler childrenChanged = this.ChildrenChanged;
			if (childrenChanged != null)
			{
				childrenChanged(this, EventArgs.Empty);
			}
			for (BookmarkModel bookmarkModel = this.Parent; bookmarkModel != null; bookmarkModel = bookmarkModel.Parent)
			{
				bookmarkModel.NotifyChildrenChanged(false);
			}
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x00057CA8 File Offset: 0x00055EA8
		public BookmarkModel InsertChild(PdfDocument document, int index, BookmarkRecord record)
		{
			if (index < 0 || index > this.Children.Count)
			{
				throw new ArgumentException("index");
			}
			record.Index = index;
			PdfBookmarkCollections rawBookmarkCollection = this.GetRawBookmarkCollection(document);
			PdfBookmark pdfBookmark = BookmarkRecordFactory.Insert(document, record, rawBookmarkCollection);
			if (pdfBookmark != null)
			{
				BookmarkModel bookmarkModel = BookmarkModel.Create(pdfBookmark);
				this.children.Insert(index, bookmarkModel);
				bookmarkModel.Parent = this;
				this.NotifyChildrenChanged(true);
				return bookmarkModel;
			}
			return null;
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x00057D14 File Offset: 0x00055F14
		public BookmarkRecord RemoveChild(PdfDocument document, BookmarkModel model)
		{
			if (model.Parent != this)
			{
				return null;
			}
			BookmarkRecord bookmarkRecord = BookmarkRecordFactory.CreateRecord(document, model.RawBookmark);
			if (bookmarkRecord != null && this.children.Remove(model))
			{
				PdfBookmarkCollections rawBookmarkCollection = this.GetRawBookmarkCollection(document);
				BookmarkRecordFactory.Remove(document, model.RawBookmark, rawBookmarkCollection);
				model.Parent = null;
				this.NotifyChildrenChanged(true);
				return bookmarkRecord;
			}
			return null;
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00057D70 File Offset: 0x00055F70
		public BookmarkRecord RemoveChildAt(PdfDocument document, int index)
		{
			if (index < 0 || index >= this.children.Count)
			{
				throw new ArgumentException("index");
			}
			BookmarkModel bookmarkModel = this.Children[index];
			BookmarkRecord bookmarkRecord = BookmarkRecordFactory.CreateRecord(document, bookmarkModel.RawBookmark);
			this.children.RemoveAt(index);
			PdfBookmarkCollections rawBookmarkCollection = this.GetRawBookmarkCollection(document);
			BookmarkRecordFactory.Remove(document, rawBookmarkCollection[bookmarkRecord.Index], rawBookmarkCollection);
			bookmarkModel.Parent = null;
			this.NotifyChildrenChanged(true);
			return bookmarkRecord;
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x00057DEA File Offset: 0x00055FEA
		public virtual PdfBookmarkCollections GetRawBookmarkCollection(PdfDocument document)
		{
			return this.RawBookmark.Childs;
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00057DF8 File Offset: 0x00055FF8
		public static BookmarkModel Create(PdfDocument document)
		{
			if (document == null)
			{
				throw new ArgumentException("document");
			}
			BookmarkModel.RootBookmarkModel rootBookmarkModel = new BookmarkModel.RootBookmarkModel(document);
			if (document.Bookmarks != null)
			{
				IEnumerable<BookmarkModel> enumerable = document.Bookmarks.Select((PdfBookmark c) => BookmarkModel.<Create>g__CreateCore|51_0(c));
				rootBookmarkModel.children = new BookmarkModel.NotifyResetList<BookmarkModel>(enumerable);
				for (int i = 0; i < rootBookmarkModel.children.Count; i++)
				{
					rootBookmarkModel.children[i].parent = rootBookmarkModel;
				}
			}
			else
			{
				rootBookmarkModel.children = new BookmarkModel.NotifyResetList<BookmarkModel>();
			}
			return rootBookmarkModel;
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x00057E90 File Offset: 0x00056090
		public static BookmarkModel Create(PdfBookmark bookmark)
		{
			if (bookmark == null)
			{
				throw new ArgumentException("bookmark");
			}
			Point? point = null;
			if (bookmark.Destination != null)
			{
				float? left = bookmark.Destination.Left;
				float? top = bookmark.Destination.Top;
				if (left != null || top != null)
				{
					point = new Point?(new Point((double)left.GetValueOrDefault(), (double)top.GetValueOrDefault()));
				}
			}
			BookmarkModel bookmarkModel = new BookmarkModel();
			bookmarkModel.Title = bookmark.Title ?? string.Empty;
			PdfDestination destination = bookmark.Destination;
			bookmarkModel.PageIndex = ((destination != null) ? destination.PageIndex : 0);
			bookmarkModel.Position = point;
			bookmarkModel.RawBookmark = bookmark;
			BookmarkModel bookmarkModel2 = bookmarkModel;
			BookmarkModel.NotifyResetList<BookmarkModel> notifyResetList = BookmarkModel.CreateChild(bookmark);
			foreach (BookmarkModel bookmarkModel3 in notifyResetList)
			{
				bookmarkModel3.Parent = bookmarkModel2;
			}
			bookmarkModel2.children = notifyResetList;
			return bookmarkModel2;
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x00057F94 File Offset: 0x00056194
		private static BookmarkModel.NotifyResetList<BookmarkModel> CreateChild(PdfBookmark bookmark)
		{
			if (bookmark == null)
			{
				throw new ArgumentException("bookmark");
			}
			if (bookmark.Childs != null && bookmark.Childs.Count > 0)
			{
				return new BookmarkModel.NotifyResetList<BookmarkModel>(bookmark.Childs.Select((PdfBookmark c) => BookmarkModel.Create(c)));
			}
			return new BookmarkModel.NotifyResetList<BookmarkModel>();
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x00058004 File Offset: 0x00056204
		[CompilerGenerated]
		internal static BookmarkModel <Create>g__CreateCore|51_0(PdfBookmark bookmark)
		{
			if (bookmark == null)
			{
				throw new ArgumentException("bookmark");
			}
			Point? point = null;
			if (bookmark.Destination != null)
			{
				float? left = bookmark.Destination.Left;
				float? top = bookmark.Destination.Top;
				if (left != null || top != null)
				{
					point = new Point?(new Point((double)left.GetValueOrDefault(), (double)top.GetValueOrDefault()));
				}
			}
			BookmarkModel bookmarkModel = new BookmarkModel();
			bookmarkModel.Title = bookmark.Title ?? string.Empty;
			PdfDestination destination = bookmark.Destination;
			bookmarkModel.PageIndex = ((destination != null) ? destination.PageIndex : 0);
			bookmarkModel.Position = point;
			bookmarkModel.RawBookmark = bookmark;
			BookmarkModel bookmarkModel2 = bookmarkModel;
			BookmarkModel.NotifyResetList<BookmarkModel> notifyResetList = BookmarkModel.CreateChild(bookmark);
			foreach (BookmarkModel bookmarkModel3 in notifyResetList)
			{
				bookmarkModel3.Parent = bookmarkModel2;
			}
			bookmarkModel2.children = notifyResetList;
			return bookmarkModel2;
		}

		// Token: 0x040007A7 RID: 1959
		private string title;

		// Token: 0x040007A8 RID: 1960
		private BookmarkModel.NotifyResetList<BookmarkModel> children;

		// Token: 0x040007A9 RID: 1961
		private BookmarkModel parent;

		// Token: 0x040007AA RID: 1962
		private bool isSelected;

		// Token: 0x040007AB RID: 1963
		private bool isExpanded;

		// Token: 0x040007AC RID: 1964
		private bool isHighlighted;

		// Token: 0x020005A2 RID: 1442
		private class NotifyResetList<T> : List<T>, INotifyCollectionChanged
		{
			// Token: 0x060031A3 RID: 12707 RVA: 0x000F37A2 File Offset: 0x000F19A2
			public NotifyResetList()
			{
			}

			// Token: 0x060031A4 RID: 12708 RVA: 0x000F37AA File Offset: 0x000F19AA
			public NotifyResetList(IEnumerable<T> items)
				: base(items)
			{
			}

			// Token: 0x14000050 RID: 80
			// (add) Token: 0x060031A5 RID: 12709 RVA: 0x000F37B4 File Offset: 0x000F19B4
			// (remove) Token: 0x060031A6 RID: 12710 RVA: 0x000F37EC File Offset: 0x000F19EC
			public event NotifyCollectionChangedEventHandler CollectionChanged;

			// Token: 0x060031A7 RID: 12711 RVA: 0x000F3821 File Offset: 0x000F1A21
			public void NotifyReset()
			{
				NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
				if (collectionChanged == null)
				{
					return;
				}
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		// Token: 0x020005A3 RID: 1443
		private class RootBookmarkModel : BookmarkModel
		{
			// Token: 0x060031A8 RID: 12712 RVA: 0x000F383A File Offset: 0x000F1A3A
			public RootBookmarkModel(PdfDocument document)
			{
				this.document = document;
			}

			// Token: 0x17000D23 RID: 3363
			// (get) Token: 0x060031A9 RID: 12713 RVA: 0x000F3849 File Offset: 0x000F1A49
			public override bool IsExpanded
			{
				get
				{
					throw new NotImplementedException("IsExpanded");
				}
			}

			// Token: 0x17000D24 RID: 3364
			// (get) Token: 0x060031AA RID: 12714 RVA: 0x000F3855 File Offset: 0x000F1A55
			public override bool IsHighlighted
			{
				get
				{
					throw new NotImplementedException("IsHighlighted");
				}
			}

			// Token: 0x17000D25 RID: 3365
			// (get) Token: 0x060031AB RID: 12715 RVA: 0x000F3861 File Offset: 0x000F1A61
			public override bool IsSelected
			{
				get
				{
					throw new NotImplementedException("IsSelected");
				}
			}

			// Token: 0x17000D26 RID: 3366
			// (get) Token: 0x060031AC RID: 12716 RVA: 0x000F386D File Offset: 0x000F1A6D
			public override int PageIndex
			{
				get
				{
					throw new NotImplementedException("PageIndex");
				}
			}

			// Token: 0x17000D27 RID: 3367
			// (get) Token: 0x060031AD RID: 12717 RVA: 0x000F3879 File Offset: 0x000F1A79
			public override PdfBookmark RawBookmark
			{
				get
				{
					throw new NotImplementedException("RawBookmark");
				}
			}

			// Token: 0x17000D28 RID: 3368
			// (get) Token: 0x060031AE RID: 12718 RVA: 0x000F3885 File Offset: 0x000F1A85
			public override string Title
			{
				get
				{
					throw new NotImplementedException("Title");
				}
			}

			// Token: 0x17000D29 RID: 3369
			// (get) Token: 0x060031AF RID: 12719 RVA: 0x000F3891 File Offset: 0x000F1A91
			public override BookmarkModel Parent
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000D2A RID: 3370
			// (get) Token: 0x060031B0 RID: 12720 RVA: 0x000F3894 File Offset: 0x000F1A94
			public override Point? Position
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000D2B RID: 3371
			// (get) Token: 0x060031B1 RID: 12721 RVA: 0x000F38AA File Offset: 0x000F1AAA
			public override int Level
			{
				get
				{
					return -1;
				}
			}

			// Token: 0x060031B2 RID: 12722 RVA: 0x000F38AD File Offset: 0x000F1AAD
			public override bool UpdateTitle(string newTitle)
			{
				throw new NotImplementedException("UpdateTitle");
			}

			// Token: 0x060031B3 RID: 12723 RVA: 0x000F38B9 File Offset: 0x000F1AB9
			public override PdfBookmarkCollections GetRawBookmarkCollection(PdfDocument document)
			{
				return document.Bookmarks;
			}

			// Token: 0x04001EA3 RID: 7843
			private readonly PdfDocument document;
		}
	}
}
