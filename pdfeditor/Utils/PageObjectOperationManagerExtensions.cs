using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Models.Annotations;
using pdfeditor.Models.Operations;
using pdfeditor.Models.PageContents;
using pdfeditor.Services;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Utils
{
	// Token: 0x0200008B RID: 139
	public static class PageObjectOperationManagerExtensions
	{
		// Token: 0x06000932 RID: 2354 RVA: 0x0002DA1C File Offset: 0x0002BC1C
		public static async Task<PdfTextObject> MoveTextObjectAsync(this OperationManager manager, PdfPage page, PdfTextObject textObject, FS_POINTF destLocation, string tag = "")
		{
			PageObjectOperationManagerExtensions.<>c__DisplayClass0_0 CS$<>8__locals1 = new PageObjectOperationManagerExtensions.<>c__DisplayClass0_0();
			CS$<>8__locals1.destLocation = destLocation;
			PdfTextObject pdfTextObject;
			if (manager == null || page == null || textObject == null)
			{
				pdfTextObject = null;
			}
			else
			{
				CS$<>8__locals1.parentAccessor = PageObjectOperationManagerExtensions.TextObjectParentAccessor.Create(page, textObject);
				if (CS$<>8__locals1.parentAccessor == null)
				{
					pdfTextObject = null;
				}
				else
				{
					CS$<>8__locals1.sourceLocation = textObject.Location;
					CS$<>8__locals1.pageIdx = page.PageIndex;
					CS$<>8__locals1.objIdx = textObject.Container.IndexOf(textObject);
					textObject.Location = CS$<>8__locals1.destLocation;
					page.GenerateContentAdvance(false);
					await manager.AddOperationAsync(delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass0_0.<<MoveTextObjectAsync>b__0>d <<MoveTextObjectAsync>b__0>d;
						<<MoveTextObjectAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<MoveTextObjectAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<MoveTextObjectAsync>b__0>d.doc = doc;
						<<MoveTextObjectAsync>b__0>d.<>1__state = -1;
						<<MoveTextObjectAsync>b__0>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass0_0.<<MoveTextObjectAsync>b__0>d>(ref <<MoveTextObjectAsync>b__0>d);
						return <<MoveTextObjectAsync>b__0>d.<>t__builder.Task;
					}, delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass0_0.<<MoveTextObjectAsync>b__1>d <<MoveTextObjectAsync>b__1>d;
						<<MoveTextObjectAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<MoveTextObjectAsync>b__1>d.<>4__this = CS$<>8__locals1;
						<<MoveTextObjectAsync>b__1>d.doc = doc;
						<<MoveTextObjectAsync>b__1>d.<>1__state = -1;
						<<MoveTextObjectAsync>b__1>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass0_0.<<MoveTextObjectAsync>b__1>d>(ref <<MoveTextObjectAsync>b__1>d);
						return <<MoveTextObjectAsync>b__1>d.<>t__builder.Task;
					}, tag);
					await page.TryRedrawPageAsync(default(CancellationToken));
					PdfPageObjectsCollection parent = CS$<>8__locals1.parentAccessor.GetParent(page);
					pdfTextObject = ((parent != null) ? parent[CS$<>8__locals1.objIdx] : null) as PdfTextObject;
				}
			}
			return pdfTextObject;
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x0002DA80 File Offset: 0x0002BC80
		public static async Task<PdfTextObject[]> ModifyTextObjectAsync(this OperationManager manager, PdfPage page, PdfTextObject textObject, string newText, string tag = "")
		{
			PageObjectOperationManagerExtensions.<>c__DisplayClass1_0 CS$<>8__locals1 = new PageObjectOperationManagerExtensions.<>c__DisplayClass1_0();
			PdfTextObject[] array;
			if (manager == null || page == null || textObject == null)
			{
				array = null;
			}
			else if (textObject.TextUnicode == newText)
			{
				array = null;
			}
			else
			{
				CS$<>8__locals1.parentAccessor = PageObjectOperationManagerExtensions.TextObjectParentAccessor.Create(page, textObject);
				if (CS$<>8__locals1.parentAccessor == null)
				{
					array = null;
				}
				else
				{
					PdfPageObjectsCollection container = textObject.Container;
					CS$<>8__locals1.pageIdx = page.PageIndex;
					CS$<>8__locals1.objIdx = container.IndexOf(textObject);
					CS$<>8__locals1.sourceModel = (PageTextObject)PageContentFactory.Create(textObject);
					PdfTextObject[] objs = PageContentUtils.UpdateTextObjectContent(page, textObject, newText);
					CS$<>8__locals1.destModels = objs.Select((PdfTextObject c) => PageContentFactory.Create(c)).OfType<PageTextObject>().ToArray<PageTextObject>();
					container.RemoveAt(CS$<>8__locals1.objIdx);
					for (int i = 0; i < objs.Length; i++)
					{
						container.Insert(i + CS$<>8__locals1.objIdx, objs[i]);
					}
					page.GenerateContentAdvance(false);
					await manager.AddOperationAsync(delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass1_0.<<ModifyTextObjectAsync>b__1>d <<ModifyTextObjectAsync>b__1>d;
						<<ModifyTextObjectAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<ModifyTextObjectAsync>b__1>d.<>4__this = CS$<>8__locals1;
						<<ModifyTextObjectAsync>b__1>d.doc = doc;
						<<ModifyTextObjectAsync>b__1>d.<>1__state = -1;
						<<ModifyTextObjectAsync>b__1>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass1_0.<<ModifyTextObjectAsync>b__1>d>(ref <<ModifyTextObjectAsync>b__1>d);
						return <<ModifyTextObjectAsync>b__1>d.<>t__builder.Task;
					}, delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass1_0.<<ModifyTextObjectAsync>b__2>d <<ModifyTextObjectAsync>b__2>d;
						<<ModifyTextObjectAsync>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<ModifyTextObjectAsync>b__2>d.<>4__this = CS$<>8__locals1;
						<<ModifyTextObjectAsync>b__2>d.doc = doc;
						<<ModifyTextObjectAsync>b__2>d.<>1__state = -1;
						<<ModifyTextObjectAsync>b__2>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass1_0.<<ModifyTextObjectAsync>b__2>d>(ref <<ModifyTextObjectAsync>b__2>d);
						return <<ModifyTextObjectAsync>b__2>d.<>t__builder.Task;
					}, tag);
					await page.TryRedrawPageAsync(default(CancellationToken));
					PdfPageObjectsCollection parent = CS$<>8__locals1.parentAccessor.GetParent(page);
					array = ((parent != null) ? parent.Skip(CS$<>8__locals1.objIdx).Take(objs.Length).OfType<PdfTextObject>()
						.ToArray<PdfTextObject>() : null);
				}
			}
			return array;
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0002DAE4 File Offset: 0x0002BCE4
		public static async Task<bool> DeleteTextObjectAsync(this OperationManager manager, PdfPage page, PdfTextObject textObject, string tag = "")
		{
			PageObjectOperationManagerExtensions.<>c__DisplayClass2_0 CS$<>8__locals1 = new PageObjectOperationManagerExtensions.<>c__DisplayClass2_0();
			bool flag;
			if (manager == null || page == null || textObject == null)
			{
				flag = false;
			}
			else
			{
				CS$<>8__locals1.parentAccessor = PageObjectOperationManagerExtensions.TextObjectParentAccessor.Create(page, textObject);
				if (CS$<>8__locals1.parentAccessor == null)
				{
					flag = false;
				}
				else
				{
					PdfPageObjectsCollection container = textObject.Container;
					CS$<>8__locals1.pageIdx = page.PageIndex;
					CS$<>8__locals1.objIdx = container.IndexOf(textObject);
					CS$<>8__locals1.sourceModel = (PageTextObject)PageContentFactory.Create(textObject);
					container.RemoveAt(CS$<>8__locals1.objIdx);
					page.GenerateContentAdvance(false);
					await manager.AddOperationAsync(delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass2_0.<<DeleteTextObjectAsync>b__0>d <<DeleteTextObjectAsync>b__0>d;
						<<DeleteTextObjectAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<DeleteTextObjectAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<DeleteTextObjectAsync>b__0>d.doc = doc;
						<<DeleteTextObjectAsync>b__0>d.<>1__state = -1;
						<<DeleteTextObjectAsync>b__0>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass2_0.<<DeleteTextObjectAsync>b__0>d>(ref <<DeleteTextObjectAsync>b__0>d);
						return <<DeleteTextObjectAsync>b__0>d.<>t__builder.Task;
					}, delegate(PdfDocument doc)
					{
						PageObjectOperationManagerExtensions.<>c__DisplayClass2_0.<<DeleteTextObjectAsync>b__1>d <<DeleteTextObjectAsync>b__1>d;
						<<DeleteTextObjectAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<DeleteTextObjectAsync>b__1>d.<>4__this = CS$<>8__locals1;
						<<DeleteTextObjectAsync>b__1>d.doc = doc;
						<<DeleteTextObjectAsync>b__1>d.<>1__state = -1;
						<<DeleteTextObjectAsync>b__1>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass2_0.<<DeleteTextObjectAsync>b__1>d>(ref <<DeleteTextObjectAsync>b__1>d);
						return <<DeleteTextObjectAsync>b__1>d.<>t__builder.Task;
					}, tag);
					await page.TryRedrawPageAsync(default(CancellationToken));
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0002DB40 File Offset: 0x0002BD40
		public static async Task<bool> RedactionTextObjectAsync(this OperationManager manager, PdfPage page, FS_RECTF rect, string tag = "Redact")
		{
			return await manager.RedactionTextObjectAsync(page, rect, new FS_COLOR?(FS_COLOR.Black), tag);
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x0002DB9C File Offset: 0x0002BD9C
		public static async Task<bool> RedactionTextObjectAsync(this OperationManager manager, PdfPage page, FS_RECTF rect, FS_COLOR? fillColor, string tag = "Redact")
		{
			PageObjectOperationManagerExtensions.<>c__DisplayClass4_0 CS$<>8__locals1 = new PageObjectOperationManagerExtensions.<>c__DisplayClass4_0();
			CS$<>8__locals1.fillColor = fillColor;
			PdfTextObject[] intersectingTextObjects = PageContentUtils.GetIntersectingTextObjects(page, rect);
			CS$<>8__locals1.oldModels = PageObjectOperationManagerExtensions.CreateRemoveTextObjectRedoRecordModel(page, intersectingTextObjects);
			RemoveIntersectingTextResult removeIntersectingTextResult = PageContentUtils.RemoveIntersectingText(intersectingTextObjects.OfType<PdfTextObject>().ToArray<PdfTextObject>(), rect);
			CS$<>8__locals1.newModels = PageObjectOperationManagerExtensions.CreateRemoveTextObjectRedoRecordModel(page, (removeIntersectingTextResult != null) ? removeIntersectingTextResult.NewTextObjects : null);
			CS$<>8__locals1.pageIdx = page.PageIndex;
			int count = page.PageObjects.Count;
			CS$<>8__locals1.pathProperties = null;
			CS$<>8__locals1.fillRects = new FS_RECTF[] { rect };
			if (CS$<>8__locals1.fillColor != null)
			{
				PdfPathObject pdfPathObject = PdfPathObject.Create(FillModes.Alternate, false);
				pdfPathObject.FillColor = CS$<>8__locals1.fillColor.Value;
				pdfPathObject.Path.Add(new FS_PATHPOINTF(rect.left, rect.top, PathPointFlags.MoveTo));
				pdfPathObject.Path.Add(new FS_PATHPOINTF(rect.right, rect.top, PathPointFlags.LineTo));
				pdfPathObject.Path.Add(new FS_PATHPOINTF(rect.right, rect.bottom, PathPointFlags.LineTo));
				pdfPathObject.Path.Add(new FS_PATHPOINTF(rect.left, rect.bottom, PathPointFlags.CloseFigure | PathPointFlags.LineTo));
				page.PageObjects.Add(pdfPathObject);
				pdfPathObject.CalcBoundingBox();
				if (CS$<>8__locals1.pathProperties == null)
				{
					CS$<>8__locals1.pathProperties = new List<global::System.ValueTuple<int, FS_RECTF>>();
				}
				CS$<>8__locals1.pathProperties.Add(new global::System.ValueTuple<int, FS_RECTF>(count, pdfPathObject.BoundingBox));
			}
			await manager.AddOperationAsync(delegate(PdfDocument doc)
			{
				PageObjectOperationManagerExtensions.<>c__DisplayClass4_0.<<RedactionTextObjectAsync>b__0>d <<RedactionTextObjectAsync>b__0>d;
				<<RedactionTextObjectAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<RedactionTextObjectAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<RedactionTextObjectAsync>b__0>d.doc = doc;
				<<RedactionTextObjectAsync>b__0>d.<>1__state = -1;
				<<RedactionTextObjectAsync>b__0>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass4_0.<<RedactionTextObjectAsync>b__0>d>(ref <<RedactionTextObjectAsync>b__0>d);
				return <<RedactionTextObjectAsync>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				PageObjectOperationManagerExtensions.<>c__DisplayClass4_0.<<RedactionTextObjectAsync>b__1>d <<RedactionTextObjectAsync>b__1>d;
				<<RedactionTextObjectAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<RedactionTextObjectAsync>b__1>d.<>4__this = CS$<>8__locals1;
				<<RedactionTextObjectAsync>b__1>d.doc = doc;
				<<RedactionTextObjectAsync>b__1>d.<>1__state = -1;
				<<RedactionTextObjectAsync>b__1>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass4_0.<<RedactionTextObjectAsync>b__1>d>(ref <<RedactionTextObjectAsync>b__1>d);
				return <<RedactionTextObjectAsync>b__1>d.<>t__builder.Task;
			}, tag);
			page.GenerateContentAdvance(false);
			await page.TryRedrawPageAsync(default(CancellationToken));
			Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshThumbnail(new int[] { CS$<>8__locals1.pageIdx });
			return true;
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x0002DC00 File Offset: 0x0002BE00
		public static async Task<bool> RemoveSelectedTextAsync(this OperationManager manager, PdfPage page, int selectRangeStartIndex, int selectRangeEndIndex, string tag = "RemoveSelectedText")
		{
			PageObjectOperationManagerExtensions.<>c__DisplayClass5_0 CS$<>8__locals1 = new PageObjectOperationManagerExtensions.<>c__DisplayClass5_0();
			PdfTextObject pdfTextObject;
			int num;
			PdfTextObject pdfTextObject2;
			int num2;
			global::System.Collections.Generic.IReadOnlyList<PdfTextObject> selectedTextObject = PageContentUtils.GetSelectedTextObject(page, selectRangeStartIndex, selectRangeEndIndex, out pdfTextObject, out num, out pdfTextObject2, out num2);
			bool flag;
			if (selectedTextObject.Count == 0)
			{
				flag = false;
			}
			else
			{
				CS$<>8__locals1.oldModels = PageObjectOperationManagerExtensions.CreateRemoveTextObjectRedoRecordModel(page, selectedTextObject);
				RemoveIntersectingTextResult removeIntersectingTextResult = PageContentUtils.RemoveSelectedText(selectedTextObject.OfType<PdfTextObject>().ToArray<PdfTextObject>(), pdfTextObject, num, pdfTextObject2, num2);
				CS$<>8__locals1.newModels = PageObjectOperationManagerExtensions.CreateRemoveTextObjectRedoRecordModel(page, (removeIntersectingTextResult != null) ? removeIntersectingTextResult.NewTextObjects : null);
				CS$<>8__locals1.pageIdx = page.PageIndex;
				await manager.AddOperationAsync(delegate(PdfDocument doc)
				{
					PageObjectOperationManagerExtensions.<>c__DisplayClass5_0.<<RemoveSelectedTextAsync>b__0>d <<RemoveSelectedTextAsync>b__0>d;
					<<RemoveSelectedTextAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<RemoveSelectedTextAsync>b__0>d.<>4__this = CS$<>8__locals1;
					<<RemoveSelectedTextAsync>b__0>d.doc = doc;
					<<RemoveSelectedTextAsync>b__0>d.<>1__state = -1;
					<<RemoveSelectedTextAsync>b__0>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass5_0.<<RemoveSelectedTextAsync>b__0>d>(ref <<RemoveSelectedTextAsync>b__0>d);
					return <<RemoveSelectedTextAsync>b__0>d.<>t__builder.Task;
				}, delegate(PdfDocument doc)
				{
					PageObjectOperationManagerExtensions.<>c__DisplayClass5_0.<<RemoveSelectedTextAsync>b__1>d <<RemoveSelectedTextAsync>b__1>d;
					<<RemoveSelectedTextAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<RemoveSelectedTextAsync>b__1>d.<>4__this = CS$<>8__locals1;
					<<RemoveSelectedTextAsync>b__1>d.doc = doc;
					<<RemoveSelectedTextAsync>b__1>d.<>1__state = -1;
					<<RemoveSelectedTextAsync>b__1>d.<>t__builder.Start<PageObjectOperationManagerExtensions.<>c__DisplayClass5_0.<<RemoveSelectedTextAsync>b__1>d>(ref <<RemoveSelectedTextAsync>b__1>d);
					return <<RemoveSelectedTextAsync>b__1>d.<>t__builder.Task;
				}, tag);
				page.GenerateContentAdvance(false);
				await page.TryRedrawPageAsync(default(CancellationToken));
				Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshThumbnail(new int[] { CS$<>8__locals1.pageIdx });
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x0002DC64 File Offset: 0x0002BE64
		private static async Task RemoveTextObjectRedoOperationAsync(PdfDocument document, int pageIndex, global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel> oldModels, global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel> newModels, FS_RECTF[] fillRects, FS_COLOR? fillColor)
		{
			PdfPage pdfPage = document.Pages[pageIndex];
			for (int i = oldModels.Count - 1; i >= 0; i--)
			{
				PdfPageObjectsCollection parent = oldModels[i].Parent.GetParent(pdfPage);
				for (int j = oldModels[i].Items.Count - 1; j >= 0; j--)
				{
					parent.RemoveAt(oldModels[i].Items[j].Index);
				}
			}
			for (int k = 0; k < newModels.Count; k++)
			{
				PdfPageObjectsCollection parent2 = newModels[k].Parent.GetParent(pdfPage);
				for (int l = 0; l < newModels[k].Items.Count; l++)
				{
					PageTextObject textObject = newModels[k].Items[l].TextObject;
					PdfTextObject pdfTextObject = PdfTextObject.Create("", 0f, 0f, textObject.Font, textObject.FontSize);
					textObject.Apply(pdfTextObject);
					parent2.Insert(newModels[k].Items[l].Index, pdfTextObject);
				}
			}
			if (fillColor != null && fillRects != null && fillRects.Length != 0)
			{
				foreach (FS_RECTF fs_RECTF in fillRects)
				{
					PdfPathObject pdfPathObject = PdfPathObject.Create(FillModes.Alternate, false);
					pdfPathObject.FillColor = fillColor.Value;
					pdfPathObject.Path.Add(new FS_PATHPOINTF(fs_RECTF.left, fs_RECTF.top, PathPointFlags.MoveTo));
					pdfPathObject.Path.Add(new FS_PATHPOINTF(fs_RECTF.right, fs_RECTF.top, PathPointFlags.LineTo));
					pdfPathObject.Path.Add(new FS_PATHPOINTF(fs_RECTF.right, fs_RECTF.bottom, PathPointFlags.LineTo));
					pdfPathObject.Path.Add(new FS_PATHPOINTF(fs_RECTF.left, fs_RECTF.bottom, PathPointFlags.CloseFigure | PathPointFlags.LineTo));
					pdfPage.PageObjects.Add(pdfPathObject);
				}
			}
			pdfPage.GenerateContentAdvance(false);
			await pdfPage.TryRedrawPageAsync(default(CancellationToken));
			Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshThumbnail(new int[] { pageIndex });
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0002DCD4 File Offset: 0x0002BED4
		private static async Task RemoveTextObjectUndoOperationAsync(PdfDocument document, int pageIndex, global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel> oldModels, global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel> newModels, FS_COLOR? fillColor, [global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "index", "bbox" })] global::System.Collections.Generic.IReadOnlyList<global::System.ValueTuple<int, FS_RECTF>> pathProperties)
		{
			PdfPage pdfPage = document.Pages[pageIndex];
			if (fillColor != null && pathProperties != null)
			{
				for (int i = 0; i < pathProperties.Count; i++)
				{
					PageObjectOperationManagerExtensions.<RemoveTextObjectUndoOperationAsync>g__RemoveRedactionPathObject|7_0(pdfPage, pathProperties[i].Item1, pathProperties[i].Item2);
				}
			}
			for (int j = newModels.Count - 1; j >= 0; j--)
			{
				PdfPageObjectsCollection parent = newModels[j].Parent.GetParent(pdfPage);
				for (int k = newModels[j].Items.Count - 1; k >= 0; k--)
				{
					parent.RemoveAt(newModels[j].Items[k].Index);
				}
			}
			for (int l = 0; l < oldModels.Count; l++)
			{
				PdfPageObjectsCollection parent2 = oldModels[l].Parent.GetParent(pdfPage);
				for (int m = 0; m < oldModels[l].Items.Count; m++)
				{
					PageTextObject textObject = oldModels[l].Items[m].TextObject;
					PdfTextObject pdfTextObject = PdfTextObject.Create("", 0f, 0f, textObject.Font, textObject.FontSize);
					textObject.Apply(pdfTextObject);
					parent2.Insert(oldModels[l].Items[m].Index, pdfTextObject);
				}
			}
			pdfPage.GenerateContentAdvance(false);
			await pdfPage.TryRedrawPageAsync(default(CancellationToken));
			Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshThumbnail(new int[] { pageIndex });
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0002DD44 File Offset: 0x0002BF44
		private static global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel> CreateRemoveTextObjectRedoRecordModel(PdfPage page, IEnumerable<PdfTextObject> textObjects)
		{
			if (textObjects == null)
			{
				return Array.Empty<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel>();
			}
			return (from PdfTextObject c in textObjects
				group c by PageObjectOperationManagerExtensions.TextObjectParentAccessor.Create(page, c) into @group
				select new PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel(@group.Key, new List<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordItemModel>((from c in @group
					select new PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordItemModel(c.Container.IndexOf(c), (PageTextObject)PageContentFactory.Create(c)) into c
					orderby c.Index
					select c).ToList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordItemModel>()))).ToList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordModel>();
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0002DDA8 File Offset: 0x0002BFA8
		[CompilerGenerated]
		internal static void <RemoveTextObjectUndoOperationAsync>g__RemoveRedactionPathObject|7_0(PdfPage _page, int pathObjectIndex, FS_RECTF _rect)
		{
			if (pathObjectIndex >= 0 && _page.PageObjects.Count > pathObjectIndex && _page.PageObjects[pathObjectIndex] is PdfPathObject)
			{
				_page.PageObjects.RemoveAt(pathObjectIndex);
				return;
			}
			for (int i = _page.PageObjects.Count - 1; i >= 0; i--)
			{
				PdfPathObject pdfPathObject = _page.PageObjects[i] as PdfPathObject;
				if (pdfPathObject != null)
				{
					FS_RECTF boundingBox = pdfPathObject.BoundingBox;
					if (Math.Abs(boundingBox.left - _rect.left) < 1f && Math.Abs(boundingBox.top - _rect.top) < 1f && Math.Abs(boundingBox.right - _rect.right) < 1f && Math.Abs(boundingBox.bottom - _rect.bottom) < 1f)
					{
						_page.PageObjects.RemoveAt(i);
						return;
					}
				}
			}
		}

		// Token: 0x02000448 RID: 1096
		private class TextObjectParentAccessor : IEquatable<PageObjectOperationManagerExtensions.TextObjectParentAccessor>
		{
			// Token: 0x06002D27 RID: 11559 RVA: 0x000DC9C4 File Offset: 0x000DABC4
			private TextObjectParentAccessor(int pageIndex, global::System.Collections.Generic.IReadOnlyList<int> objectIndexes)
			{
				this.PageIndex = pageIndex;
				this.objectIndexes = objectIndexes;
				if (objectIndexes == null)
				{
					this._hashCode = HashCode.Combine<int>(this.PageIndex);
					return;
				}
				this._hashCode = HashCode.Combine<int>(this.PageIndex);
				for (int i = 0; i < objectIndexes.Count; i++)
				{
					this._hashCode = HashCode.Combine<int, int>(this._hashCode, objectIndexes[i]);
				}
			}

			// Token: 0x17000C91 RID: 3217
			// (get) Token: 0x06002D28 RID: 11560 RVA: 0x000DCA34 File Offset: 0x000DAC34
			public int PageIndex { get; }

			// Token: 0x06002D29 RID: 11561 RVA: 0x000DCA3C File Offset: 0x000DAC3C
			public PdfPageObjectsCollection GetParent(PdfPage page)
			{
				if (page == null)
				{
					return null;
				}
				PdfPageObjectsCollection pdfPageObjectsCollection = page.PageObjects;
				for (int i = this.objectIndexes.Count - 1; i >= 0; i--)
				{
					int num = this.objectIndexes[i];
					PdfFormObject pdfFormObject = pdfPageObjectsCollection[num] as PdfFormObject;
					if (pdfFormObject == null)
					{
						throw new ArgumentException("formObj");
					}
					pdfPageObjectsCollection = pdfFormObject.PageObjects;
				}
				return pdfPageObjectsCollection;
			}

			// Token: 0x06002D2A RID: 11562 RVA: 0x000DCA9C File Offset: 0x000DAC9C
			public static PageObjectOperationManagerExtensions.TextObjectParentAccessor Create(PdfPage page, PdfTextObject textObj)
			{
				if (page == null || textObj == null || textObj.Container == null)
				{
					return null;
				}
				List<int> list = new List<int>();
				PdfPageObjectsCollection pdfPageObjectsCollection = textObj.Container;
				PdfPageObject pdfPageObject = textObj;
				for (;;)
				{
					if (pdfPageObject != textObj)
					{
						int num = PageObjectOperationManagerExtensions.TextObjectParentAccessor.IndexOf(pdfPageObjectsCollection, pdfPageObject);
						if (num == -1)
						{
							break;
						}
						list.Add(num);
					}
					if (pdfPageObjectsCollection.Handle == page.PageObjects.Handle)
					{
						goto IL_0072;
					}
					pdfPageObject = pdfPageObjectsCollection.Form;
					pdfPageObjectsCollection = ((pdfPageObject != null) ? pdfPageObject.Container : null);
					if (pdfPageObjectsCollection == null)
					{
						goto Block_7;
					}
				}
				throw new ArgumentException("idx");
				Block_7:
				return null;
				IL_0072:
				return new PageObjectOperationManagerExtensions.TextObjectParentAccessor(page.PageIndex, list);
			}

			// Token: 0x06002D2B RID: 11563 RVA: 0x000DCB28 File Offset: 0x000DAD28
			private static int IndexOf(PdfPageObjectsCollection objCollection, PdfPageObject textObj)
			{
				if (objCollection == null || textObj == null)
				{
					return -1;
				}
				for (int i = 0; i < objCollection.Count; i++)
				{
					if (textObj.Handle == objCollection[i].Handle)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x06002D2C RID: 11564 RVA: 0x000DCB6A File Offset: 0x000DAD6A
			public bool Equals(PageObjectOperationManagerExtensions.TextObjectParentAccessor other)
			{
				return other != null && this.PageIndex == other.PageIndex && BaseAnnotation.CollectionEqual<int>(this.objectIndexes, other.objectIndexes);
			}

			// Token: 0x06002D2D RID: 11565 RVA: 0x000DCB98 File Offset: 0x000DAD98
			public override bool Equals(object obj)
			{
				PageObjectOperationManagerExtensions.TextObjectParentAccessor textObjectParentAccessor = obj as PageObjectOperationManagerExtensions.TextObjectParentAccessor;
				return textObjectParentAccessor != null && ((IEquatable<PageObjectOperationManagerExtensions.TextObjectParentAccessor>)this).Equals(textObjectParentAccessor);
			}

			// Token: 0x06002D2E RID: 11566 RVA: 0x000DCBB8 File Offset: 0x000DADB8
			public override int GetHashCode()
			{
				return this._hashCode;
			}

			// Token: 0x06002D2F RID: 11567 RVA: 0x000DCBC0 File Offset: 0x000DADC0
			public static bool operator ==(PageObjectOperationManagerExtensions.TextObjectParentAccessor left, PageObjectOperationManagerExtensions.TextObjectParentAccessor right)
			{
				return object.Equals(left, right);
			}

			// Token: 0x06002D30 RID: 11568 RVA: 0x000DCBC9 File Offset: 0x000DADC9
			public static bool operator !=(PageObjectOperationManagerExtensions.TextObjectParentAccessor left, PageObjectOperationManagerExtensions.TextObjectParentAccessor right)
			{
				return !object.Equals(left, right);
			}

			// Token: 0x04001888 RID: 6280
			private int _hashCode;

			// Token: 0x04001889 RID: 6281
			private global::System.Collections.Generic.IReadOnlyList<int> objectIndexes;
		}

		// Token: 0x02000449 RID: 1097
		private class RemoveTextObjectRedoRecordModel
		{
			// Token: 0x06002D31 RID: 11569 RVA: 0x000DCBD5 File Offset: 0x000DADD5
			public RemoveTextObjectRedoRecordModel(PageObjectOperationManagerExtensions.TextObjectParentAccessor parent, global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordItemModel> items)
			{
				this.Parent = parent;
				this.Items = items;
			}

			// Token: 0x17000C92 RID: 3218
			// (get) Token: 0x06002D32 RID: 11570 RVA: 0x000DCBEB File Offset: 0x000DADEB
			public PageObjectOperationManagerExtensions.TextObjectParentAccessor Parent { get; }

			// Token: 0x17000C93 RID: 3219
			// (get) Token: 0x06002D33 RID: 11571 RVA: 0x000DCBF3 File Offset: 0x000DADF3
			public global::System.Collections.Generic.IReadOnlyList<PageObjectOperationManagerExtensions.RemoveTextObjectRedoRecordItemModel> Items { get; }
		}

		// Token: 0x0200044A RID: 1098
		private class RemoveTextObjectRedoRecordItemModel
		{
			// Token: 0x06002D34 RID: 11572 RVA: 0x000DCBFB File Offset: 0x000DADFB
			public RemoveTextObjectRedoRecordItemModel(int index, PageTextObject textObject)
			{
				this.Index = index;
				this.TextObject = textObject;
			}

			// Token: 0x17000C94 RID: 3220
			// (get) Token: 0x06002D35 RID: 11573 RVA: 0x000DCC11 File Offset: 0x000DAE11
			public int Index { get; }

			// Token: 0x17000C95 RID: 3221
			// (get) Token: 0x06002D36 RID: 11574 RVA: 0x000DCC19 File Offset: 0x000DAE19
			public PageTextObject TextObject { get; }
		}
	}
}
