using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Patagames.Pdf.Net;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Models.Operations;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000076 RID: 118
	public static class BookmarkOperationManagerExtensions
	{
		// Token: 0x060008BC RID: 2236 RVA: 0x0002B4BC File Offset: 0x000296BC
		public static async Task<bool> RemoveBookmarkAsync(this OperationManager operationManager, PdfDocument document, BookmarkModel bookmark, string tag = "")
		{
			bool flag;
			if (operationManager == null || document == null || bookmark == null || bookmark.Parent == null)
			{
				flag = false;
			}
			else
			{
				BookmarkModel parent = bookmark.Parent;
				BookmarkOperationManagerExtensions.BookmarkAccessor oldParentAccessor = new BookmarkOperationManagerExtensions.BookmarkAccessor(parent);
				int oldIndex = -1;
				for (int i = 0; i < parent.Children.Count; i++)
				{
					if (parent.Children[i] == bookmark)
					{
						oldIndex = i;
						break;
					}
				}
				if (oldIndex >= 0)
				{
					BookmarkRecord oldRecord = parent.RemoveChild(document, bookmark);
					if (oldRecord != null)
					{
						await operationManager.AddOperationAsync(delegate(PdfDocument doc)
						{
							BookmarkModel bookmarkModel = oldParentAccessor.GetTarget(doc).InsertChild(doc, oldIndex, oldRecord);
							if (bookmarkModel == null)
							{
								return;
							}
							bookmarkModel.ExpandToRoot();
						}, delegate(PdfDocument doc)
						{
							oldParentAccessor.GetTarget(doc).RemoveChildAt(doc, oldIndex);
						}, tag);
						return true;
					}
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0002B518 File Offset: 0x00029718
		public static async Task<BookmarkModel> InsertBookmarkAsync(this OperationManager operationManager, PdfDocument document, BookmarkModel parent, BookmarkRecord record, string tag = "")
		{
			BookmarkModel bookmarkModel;
			if (operationManager == null || document == null || record == null || parent == null)
			{
				bookmarkModel = null;
			}
			else
			{
				int insertIndex = record.Index;
				if (insertIndex < 0 || insertIndex > parent.Children.Count)
				{
					bookmarkModel = null;
				}
				else
				{
					BookmarkModel result = parent.InsertChild(document, insertIndex, record);
					if (result == null)
					{
						throw new ArgumentException("result");
					}
					BookmarkOperationManagerExtensions.BookmarkAccessor newParentAccessor = new BookmarkOperationManagerExtensions.BookmarkAccessor(parent);
					BookmarkRecord newRecord = BookmarkRecordFactory.CreateRecord(document, result.RawBookmark);
					await operationManager.AddOperationAsync(delegate(PdfDocument doc)
					{
						newParentAccessor.GetTarget(doc).RemoveChildAt(doc, insertIndex);
					}, delegate(PdfDocument doc)
					{
						BookmarkModel bookmarkModel2 = newParentAccessor.GetTarget(doc).InsertChild(doc, insertIndex, newRecord);
						if (bookmarkModel2 == null)
						{
							return;
						}
						bookmarkModel2.ExpandToRoot();
					}, tag);
					bookmarkModel = result;
				}
			}
			return bookmarkModel;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0002B57C File Offset: 0x0002977C
		public static async Task<BookmarkModel> MoveBookmarkAsync(this OperationManager operationManager, PdfDocument document, BookmarkModel bookmark, BookmarkModel newParent, int insertIndex, string tag = "")
		{
			BookmarkModel bookmarkModel;
			if (bookmark == null || newParent == null || insertIndex < 0 || insertIndex > newParent.Children.Count)
			{
				bookmarkModel = null;
			}
			else if (bookmark.Parent == newParent && insertIndex < newParent.Children.Count && newParent.Children[insertIndex] == bookmark)
			{
				bookmarkModel = bookmark;
			}
			else
			{
				BookmarkModel parent = bookmark.Parent;
				BookmarkOperationManagerExtensions.BookmarkAccessor oldParentAccessor = new BookmarkOperationManagerExtensions.BookmarkAccessor(parent);
				int oldIndex = -1;
				for (int i = 0; i < parent.Children.Count; i++)
				{
					if (parent.Children[i] == bookmark)
					{
						oldIndex = i;
						break;
					}
				}
				if (oldIndex >= 0)
				{
					BookmarkRecord oldRecord = parent.RemoveChild(document, bookmark);
					if (oldRecord != null)
					{
						if (newParent == parent && insertIndex > oldIndex)
						{
							int insertIndex2 = insertIndex;
							insertIndex = insertIndex2 - 1;
						}
						BookmarkModel result = newParent.InsertChild(document, insertIndex, oldRecord);
						if (result == null)
						{
							throw new ArgumentException("result");
						}
						BookmarkOperationManagerExtensions.BookmarkAccessor newParentAccessor = new BookmarkOperationManagerExtensions.BookmarkAccessor(newParent);
						BookmarkRecord newRecord = BookmarkRecordFactory.CreateRecord(document, result.RawBookmark);
						result.ExpandToRoot();
						await operationManager.AddOperationAsync(delegate(PdfDocument doc)
						{
							newParentAccessor.GetTarget(doc).RemoveChildAt(doc, insertIndex);
							BookmarkModel bookmarkModel2 = oldParentAccessor.GetTarget(doc).InsertChild(doc, oldIndex, oldRecord);
							if (bookmarkModel2 == null)
							{
								return;
							}
							bookmarkModel2.ExpandToRoot();
						}, delegate(PdfDocument doc)
						{
							oldParentAccessor.GetTarget(doc).RemoveChildAt(doc, oldIndex);
							BookmarkModel bookmarkModel3 = newParentAccessor.GetTarget(doc).InsertChild(doc, insertIndex, newRecord);
							if (bookmarkModel3 == null)
							{
								return;
							}
							bookmarkModel3.ExpandToRoot();
						}, tag);
						return result;
					}
				}
				bookmarkModel = null;
			}
			return bookmarkModel;
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0002B5EC File Offset: 0x000297EC
		public static async Task<bool> UpdateBookmarkTitleAsync(this OperationManager operationManager, PdfDocument document, BookmarkModel bookmark, string newTitle, string tag = "")
		{
			newTitle = newTitle ?? string.Empty;
			string oldTitle = ((bookmark != null) ? bookmark.Title : null) ?? "";
			bool flag;
			if (string.Equals(newTitle, oldTitle))
			{
				flag = false;
			}
			else
			{
				bool? flag2 = ((bookmark != null) ? new bool?(bookmark.UpdateTitle(newTitle)) : null);
				if (flag2 != null && flag2.GetValueOrDefault())
				{
					BookmarkOperationManagerExtensions.BookmarkAccessor parentAccessor = new BookmarkOperationManagerExtensions.BookmarkAccessor(bookmark);
					await operationManager.AddOperationAsync(delegate(PdfDocument doc)
					{
						BookmarkModel target = parentAccessor.GetTarget(doc);
						if (target != null)
						{
							target.UpdateTitle(oldTitle);
						}
					}, delegate(PdfDocument doc)
					{
						BookmarkModel target2 = parentAccessor.GetTarget(doc);
						if (target2 != null)
						{
							target2.UpdateTitle(newTitle);
						}
					}, tag);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x0002B648 File Offset: 0x00029848
		private static BookmarkModel GetRootBookmarkModel(PdfDocument doc)
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
			MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			if (mainViewModel == null)
			{
				return null;
			}
			return mainViewModel.Bookmarks;
		}

		// Token: 0x02000426 RID: 1062
		private class BookmarkAccessor
		{
			// Token: 0x06002CBC RID: 11452 RVA: 0x000DABA4 File Offset: 0x000D8DA4
			public BookmarkAccessor(BookmarkModel bookmark)
			{
				BookmarkModel bookmarkModel = bookmark;
				while (bookmarkModel != null && bookmarkModel.Level != -1)
				{
					int count = this.indexList.Count;
					int num = 0;
					while (num < bookmarkModel.Parent.Children.Count && count == this.indexList.Count)
					{
						if (bookmarkModel.Parent.Children[num] == bookmarkModel)
						{
							this.indexList.Add(num);
						}
						num++;
					}
					if (count == this.indexList.Count)
					{
						throw new ArgumentException("bookmark");
					}
					bookmarkModel = bookmarkModel.Parent;
				}
			}

			// Token: 0x06002CBD RID: 11453 RVA: 0x000DAC4C File Offset: 0x000D8E4C
			public BookmarkModel GetTarget(BookmarkModel rootModel)
			{
				if (rootModel.Level != -1)
				{
					throw new ArgumentException("rootModel");
				}
				BookmarkModel bookmarkModel = rootModel;
				for (int i = this.indexList.Count - 1; i >= 0; i--)
				{
					bookmarkModel = bookmarkModel.Children[this.indexList[i]];
				}
				return bookmarkModel;
			}

			// Token: 0x06002CBE RID: 11454 RVA: 0x000DACA0 File Offset: 0x000D8EA0
			public BookmarkModel GetTarget(PdfDocument document)
			{
				return this.GetTarget(BookmarkOperationManagerExtensions.GetRootBookmarkModel(document));
			}

			// Token: 0x040017DF RID: 6111
			private List<int> indexList = new List<int>();
		}
	}
}
