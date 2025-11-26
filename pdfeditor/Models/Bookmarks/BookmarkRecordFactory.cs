using System;
using System.Linq.Expressions;
using System.Reflection;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;

namespace pdfeditor.Models.Bookmarks
{
	// Token: 0x02000191 RID: 401
	public static class BookmarkRecordFactory
	{
		// Token: 0x0600171F RID: 5919 RVA: 0x00058184 File Offset: 0x00056384
		public static BookmarkRecord CreateRecord(PdfDocument doc, PdfBookmark bookmark)
		{
			if (bookmark == null)
			{
				return null;
			}
			PdfBookmark parent = bookmark.Parent;
			int num = (((parent != null) ? parent.Childs : null) ?? doc.Bookmarks).IndexOf(bookmark);
			if (num == -1)
			{
				return null;
			}
			BookmarkRecord bookmarkRecord = new BookmarkRecord
			{
				Index = num,
				Title = bookmark.Title,
				Action = bookmark.Action,
				Destination = BookmarkRecordFactory.CreateDestination(bookmark.Destination)
			};
			if (bookmark.Childs != null && bookmark.Childs.Count > 0)
			{
				foreach (PdfBookmark pdfBookmark in bookmark.Childs)
				{
					BookmarkRecord bookmarkRecord2 = BookmarkRecordFactory.CreateRecord(doc, pdfBookmark);
					if (bookmarkRecord2 != null)
					{
						bookmarkRecord2.Parent = bookmarkRecord;
						bookmarkRecord.Childs.Add(bookmarkRecord2);
					}
				}
			}
			return bookmarkRecord;
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00058268 File Offset: 0x00056468
		public static PdfBookmark Insert(PdfDocument doc, BookmarkRecord record, PdfBookmarkCollections collection)
		{
			if (record == null || collection == null)
			{
				return null;
			}
			PdfBookmark pdfBookmark = null;
			if (record.Destination != null)
			{
				PdfDestination pdfDestination = new PdfDestination((record.Destination.PageIndex >= 0 && doc != null && record.Destination.PageIndex < doc.Pages.Count) ? doc : null, record.Destination.Name)
				{
					DestinationType = record.Destination.DestinationType,
					PageIndex = record.Destination.PageIndex,
					Left = record.Destination.Left,
					Top = record.Destination.Top,
					Right = record.Destination.Right,
					Bottom = record.Destination.Bottom,
					Zoom = record.Destination.Zoom
				};
				pdfBookmark = collection.InsertAt(record.Index, record.Title, pdfDestination);
			}
			else if (record.Action != null)
			{
				Pdfium.FPDFOBJ_GetParentObj(record.Action.Dictionary.Handle) != IntPtr.Zero;
				pdfBookmark = collection.InsertAt(record.Index, record.Title, record.Action);
			}
			if (record.Childs != null && record.Childs.Count > 0)
			{
				foreach (BookmarkRecord bookmarkRecord in record.Childs)
				{
					BookmarkRecordFactory.Insert(doc, bookmarkRecord, pdfBookmark.Childs);
				}
			}
			return pdfBookmark;
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x000583F4 File Offset: 0x000565F4
		public static void Remove(PdfDocument doc, PdfBookmark pdfBookmark, PdfBookmarkCollections collection)
		{
			pdfBookmark.Action = null;
			if (pdfBookmark.Childs != null && pdfBookmark.Childs.Count > 0)
			{
				foreach (PdfBookmark pdfBookmark2 in pdfBookmark.Childs)
				{
					BookmarkRecordFactory.Remove(doc, pdfBookmark2, pdfBookmark.Childs);
				}
			}
			PdfBookmark parent = pdfBookmark.Parent;
			PdfTypeDictionary pdfTypeDictionary = ((parent != null) ? parent.Dictionary : null) ?? ((doc.Root["Outlines"] as PdfTypeIndirect).Direct as PdfTypeDictionary);
			if (pdfTypeDictionary != null && !pdfTypeDictionary.ContainsKey("Count"))
			{
				pdfTypeDictionary["Count"] = PdfTypeNumber.Create(collection.Count);
			}
			collection.Remove(pdfBookmark);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x000584C8 File Offset: 0x000566C8
		private static BookmarkRecord.BookmarkDestination CreateDestination(PdfDestination destination)
		{
			if (destination == null)
			{
				return null;
			}
			return new BookmarkRecord.BookmarkDestination
			{
				DestinationType = destination.DestinationType,
				PageIndex = destination.PageIndex,
				Name = destination.Name,
				Left = destination.Left,
				Top = destination.Top,
				Right = destination.Right,
				Bottom = destination.Bottom,
				Zoom = destination.Zoom
			};
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00058540 File Offset: 0x00056740
		private static PdfDestination CreatePdfDestination(PdfDocument doc, IntPtr handle, string name = null)
		{
			if (BookmarkRecordFactory.createPdfDestinationFunc == null)
			{
				object obj = BookmarkRecordFactory.locker;
				lock (obj)
				{
					if (BookmarkRecordFactory.createPdfDestinationFunc == null)
					{
						ConstructorInfo constructor = typeof(PdfDestination).GetConstructor(BindingFlags.NonPublic, null, new Type[]
						{
							typeof(PdfDocument),
							typeof(IntPtr),
							typeof(string)
						}, null);
						if (constructor != null)
						{
							ParameterExpression parameterExpression = Expression.Parameter(typeof(PdfDocument), "doc");
							ParameterExpression parameterExpression2 = Expression.Parameter(typeof(IntPtr), "handle");
							ParameterExpression parameterExpression3 = Expression.Parameter(typeof(string), "name");
							Expression expression = Expression.New(constructor, new Expression[] { parameterExpression, parameterExpression2, parameterExpression3 });
							expression = Expression.Convert(expression, typeof(PdfDestination));
							expression = Expression.Return(Expression.Label(), expression);
							BookmarkRecordFactory.createPdfDestinationFunc = Expression.Lambda<Func<PdfDocument, IntPtr, string, PdfDestination>>(expression, new ParameterExpression[] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
						}
						else
						{
							BookmarkRecordFactory.createPdfDestinationFunc = (PdfDocument _p1, IntPtr _p2, string _p3) => null;
						}
					}
				}
			}
			return BookmarkRecordFactory.createPdfDestinationFunc(doc, handle, name);
		}

		// Token: 0x040007B7 RID: 1975
		private static Func<PdfDocument, IntPtr, string, PdfDestination> createPdfDestinationFunc;

		// Token: 0x040007B8 RID: 1976
		private static object locker = new object();
	}
}
