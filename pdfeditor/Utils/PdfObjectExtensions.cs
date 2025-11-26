using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Models.Annotations;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Services;
using PDFKit.Utils;
using PDFKit.Utils.PdfRichTextStrings;

namespace pdfeditor.Utils
{
	// Token: 0x02000092 RID: 146
	public static class PdfObjectExtensions
	{
		// Token: 0x06000991 RID: 2449 RVA: 0x00030ADC File Offset: 0x0002ECDC
		public static AnnotationHolderManager GetAnnotationHolderManager(PdfViewer viewer)
		{
			return PdfObjectExtensions.GetAnnotationHolderManager((viewer != null) ? viewer.GetPdfControl() : null);
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00030AF0 File Offset: 0x0002ECF0
		public static AnnotationHolderManager GetAnnotationHolderManager(global::PDFKit.PdfControl pdfControl)
		{
			if (pdfControl == null)
			{
				return null;
			}
			if (DispatcherHelper.UIDispatcher.CheckAccess())
			{
				return PdfObjectExtensions.<GetAnnotationHolderManager>g__GetAnnotationHolderManagerCore|1_0(pdfControl);
			}
			AnnotationHolderManager holders = null;
			DispatcherHelper.UIDispatcher.Invoke<AnnotationHolderManager>(() => holders = PdfObjectExtensions.<GetAnnotationHolderManager>g__GetAnnotationHolderManagerCore|1_0(pdfControl));
			return holders;
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x00030B50 File Offset: 0x0002ED50
		public static AnnotationCanvas GetAnnotationCanvas(PdfViewer viewer)
		{
			return PdfObjectExtensions.GetAnnotationCanvas((viewer != null) ? viewer.GetPdfControl() : null);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x00030B64 File Offset: 0x0002ED64
		public static AnnotationCanvas GetAnnotationCanvas(global::PDFKit.PdfControl pdfControl)
		{
			Panel panel = ((pdfControl != null) ? pdfControl.Parent : null) as Panel;
			if (panel != null)
			{
				return panel.Children.OfType<AnnotationCanvas>().FirstOrDefault<AnnotationCanvas>();
			}
			return null;
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00030B98 File Offset: 0x0002ED98
		public static void DeleteAnnotation(this PdfAnnotation annot)
		{
			if (annot == null)
			{
				return;
			}
			if (annot.Page == null || annot.Page.Annots == null)
			{
				return;
			}
			PdfPage page = annot.Page;
			string text = "";
			IEnumerable<BaseAnnotation> item = AnnotationModelUtils.CreateFlattenedRecord(annot).Item2;
			PdfPopupAnnotation pdfPopupAnnotation = annot as PdfPopupAnnotation;
			if (pdfPopupAnnotation != null)
			{
				PdfMarkupAnnotation pdfMarkupAnnotation = pdfPopupAnnotation.Parent as PdfMarkupAnnotation;
				if (pdfMarkupAnnotation != null)
				{
					pdfMarkupAnnotation.Popup = null;
					goto IL_006E;
				}
			}
			PdfFreeTextAnnotation pdfFreeTextAnnotation = annot as PdfFreeTextAnnotation;
			if (pdfFreeTextAnnotation != null)
			{
				text = pdfFreeTextAnnotation.Name;
			}
			IL_006E:
			foreach (BaseAnnotation baseAnnotation in item.Reverse<BaseAnnotation>())
			{
				page.Annots.RemoveAt(baseAnnotation.AnnotIndex);
			}
			if (!string.IsNullOrEmpty(text))
			{
				PdfRichTextString.RemoveCacheByName(text, 0);
			}
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x00030C74 File Offset: 0x0002EE74
		public static async Task<PdfAnnotation> DuplicateAnnotationAsync(this PdfAnnotation annot, FS_POINTF? markupOffset = null, FS_POINTF? popupOffset = null)
		{
			FS_POINTF _markupOffset = markupOffset ?? new FS_POINTF(20f, -20f);
			FS_POINTF fs_POINTF = popupOffset ?? new FS_POINTF(0f, -20f);
			PdfAnnotation pdfAnnotation;
			if (annot != null)
			{
				PdfObjectExtensions.<>c__DisplayClass5_1 CS$<>8__locals2 = new PdfObjectExtensions.<>c__DisplayClass5_1();
				PdfPage page = annot.Page;
				global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> readOnlyList;
				CS$<>8__locals2.record = AnnotationModelUtils.CloneRecord(annot, out readOnlyList);
				CS$<>8__locals2.flattens = AnnotationModelUtils.FlattenRecord(CS$<>8__locals2.record, readOnlyList);
				CS$<>8__locals2.hasPopup = false;
				Func<FS_POINTF, FS_POINTF> <>9__2;
				Func<FS_POINTF, FS_POINTF> <>9__4;
				Func<global::System.Collections.Generic.IReadOnlyList<FS_POINTF>, global::System.Collections.Generic.IReadOnlyList<FS_POINTF>> <>9__3;
				foreach (BaseAnnotation baseAnnotation in CS$<>8__locals2.flattens)
				{
					LineAnnotation lineAnnotation = baseAnnotation as LineAnnotation;
					if (lineAnnotation != null)
					{
						IEnumerable<FS_POINTF> line = lineAnnotation.Line;
						Func<FS_POINTF, FS_POINTF> func;
						if ((func = <>9__2) == null)
						{
							func = (<>9__2 = delegate(FS_POINTF c)
							{
								c.X += _markupOffset.X;
								c.Y += _markupOffset.Y;
								return c;
							});
						}
						FS_POINTF[] array = line.Select(func).ToArray<FS_POINTF>();
						lineAnnotation.Line = array;
					}
					else
					{
						InkAnnotation inkAnnotation = baseAnnotation as InkAnnotation;
						if (inkAnnotation != null)
						{
							IEnumerable<global::System.Collections.Generic.IReadOnlyList<FS_POINTF>> inkList = inkAnnotation.InkList;
							Func<global::System.Collections.Generic.IReadOnlyList<FS_POINTF>, global::System.Collections.Generic.IReadOnlyList<FS_POINTF>> func2;
							if ((func2 = <>9__3) == null)
							{
								func2 = (<>9__3 = delegate(global::System.Collections.Generic.IReadOnlyList<FS_POINTF> x)
								{
									Func<FS_POINTF, FS_POINTF> func3;
									if ((func3 = <>9__4) == null)
									{
										func3 = (<>9__4 = delegate(FS_POINTF c)
										{
											c.X += _markupOffset.X;
											c.Y += _markupOffset.Y;
											return c;
										});
									}
									return x.Select(func3).ToArray<FS_POINTF>();
								});
							}
							global::System.Collections.Generic.IReadOnlyList<FS_POINTF>[] array2 = inkList.Select(func2).ToArray<global::System.Collections.Generic.IReadOnlyList<FS_POINTF>>();
							inkAnnotation.InkList = array2;
						}
						else
						{
							BaseMarkupAnnotation baseMarkupAnnotation = baseAnnotation as BaseMarkupAnnotation;
							if (baseMarkupAnnotation != null)
							{
								if (baseMarkupAnnotation.Relationship != RelationTypes.Reply)
								{
									FS_RECTF rectangle = baseMarkupAnnotation.Rectangle;
									rectangle.left += _markupOffset.X;
									rectangle.right += _markupOffset.X;
									rectangle.top += _markupOffset.Y;
									rectangle.bottom += _markupOffset.Y;
									baseMarkupAnnotation.Rectangle = rectangle;
								}
							}
							else
							{
								PopupAnnotation popupAnnotation = baseAnnotation as PopupAnnotation;
								if (popupAnnotation != null)
								{
									FS_RECTF rectangle2 = popupAnnotation.Rectangle;
									rectangle2.top += fs_POINTF.Y;
									rectangle2.bottom += fs_POINTF.Y;
									popupAnnotation.Rectangle = rectangle2;
								}
								else
								{
									LinkAnnotation linkAnnotation = baseAnnotation as LinkAnnotation;
									if (linkAnnotation != null)
									{
										FS_RECTF rectangle3 = linkAnnotation.Rectangle;
										rectangle3.left += _markupOffset.X;
										rectangle3.right += _markupOffset.X;
										rectangle3.top += _markupOffset.Y;
										rectangle3.bottom += _markupOffset.Y;
										linkAnnotation.Rectangle = rectangle3;
									}
								}
							}
						}
					}
					if (baseAnnotation is PopupAnnotation)
					{
						CS$<>8__locals2.hasPopup = true;
					}
				}
				AnnotationModelUtils.InsertRecord(page.Document, CS$<>8__locals2.record, CS$<>8__locals2.flattens);
				global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(page.Document);
				MainViewModel mainViewModel = viewer.DataContext as MainViewModel;
				if (!mainViewModel.IsAnnotationVisible)
				{
					mainViewModel.IsAnnotationVisible = true;
				}
				await mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
				{
					PdfObjectExtensions.<>c__DisplayClass5_1.<<DuplicateAnnotationAsync>b__0>d <<DuplicateAnnotationAsync>b__0>d;
					<<DuplicateAnnotationAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<DuplicateAnnotationAsync>b__0>d.<>4__this = CS$<>8__locals2;
					<<DuplicateAnnotationAsync>b__0>d.doc = doc;
					<<DuplicateAnnotationAsync>b__0>d.<>1__state = -1;
					<<DuplicateAnnotationAsync>b__0>d.<>t__builder.Start<PdfObjectExtensions.<>c__DisplayClass5_1.<<DuplicateAnnotationAsync>b__0>d>(ref <<DuplicateAnnotationAsync>b__0>d);
					return <<DuplicateAnnotationAsync>b__0>d.<>t__builder.Task;
				}, delegate(PdfDocument doc)
				{
					PdfObjectExtensions.<>c__DisplayClass5_1.<<DuplicateAnnotationAsync>b__1>d <<DuplicateAnnotationAsync>b__1>d;
					<<DuplicateAnnotationAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<DuplicateAnnotationAsync>b__1>d.<>4__this = CS$<>8__locals2;
					<<DuplicateAnnotationAsync>b__1>d.doc = doc;
					<<DuplicateAnnotationAsync>b__1>d.<>1__state = -1;
					<<DuplicateAnnotationAsync>b__1>d.<>t__builder.Start<PdfObjectExtensions.<>c__DisplayClass5_1.<<DuplicateAnnotationAsync>b__1>d>(ref <<DuplicateAnnotationAsync>b__1>d);
					return <<DuplicateAnnotationAsync>b__1>d.<>t__builder.Task;
				}, "");
				await page.TryRedrawPageAsync(default(CancellationToken));
				if (CS$<>8__locals2.hasPopup)
				{
					AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(viewer);
					if (annotationCanvas != null)
					{
						annotationCanvas.PopupHolder.FlushAnnotationPopup();
					}
				}
				pdfAnnotation = page.Annots[CS$<>8__locals2.record.AnnotIndex];
			}
			else
			{
				pdfAnnotation = null;
			}
			return pdfAnnotation;
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x00030CC8 File Offset: 0x0002EEC8
		public static async Task TryRedrawPageAsync(this PdfPage page, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (page != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(page.Document);
				int num3;
				if (pdfControl != null)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
					int? num;
					if (annotationHolderManager == null)
					{
						num = null;
					}
					else
					{
						IAnnotationHolder currentHolder = annotationHolderManager.CurrentHolder;
						if (currentHolder == null)
						{
							num = null;
						}
						else
						{
							PdfPage currentPage = currentHolder.CurrentPage;
							num = ((currentPage != null) ? new int?(currentPage.PageIndex) : null);
						}
					}
					int? num2 = num;
					num3 = page.PageIndex;
					if ((num2.GetValueOrDefault() == num3) & (num2 != null))
					{
						annotationHolderManager.CurrentHolder.Cancel();
					}
				}
				for (int i = 0; i < 3; i = num3 + 1)
				{
					bool flag = page.IsDisposed;
					if (!flag)
					{
						flag = PdfDocumentStateService.CanDisposePage(page);
					}
					ProgressiveStatus progressiveStatus;
					if (!flag && PdfObjectExtensions.TryGetProgressiveStatus(page, out progressiveStatus))
					{
						flag = progressiveStatus != ProgressiveStatus.ToBeContinued && progressiveStatus != ProgressiveStatus.Failed;
					}
					if (flag)
					{
						try
						{
							PageDisposeHelper.DisposePage(page);
							PdfDocumentStateService.TryRedrawViewerCurrentPage(page);
						}
						catch
						{
						}
						break;
					}
					await Task.Delay(150, cancellationToken);
					num3 = i;
				}
			}
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00030D14 File Offset: 0x0002EF14
		public static async Task TryRedrawVisiblePageAsync(this PdfViewer viewer, CancellationToken cancellationToken = default(CancellationToken))
		{
			global::PDFKit.PdfControl pdfControl = ((viewer != null) ? viewer.GetPdfControl() : null);
			if (pdfControl != null)
			{
				await pdfControl.TryRedrawVisiblePageAsync(cancellationToken);
			}
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00030D60 File Offset: 0x0002EF60
		public static async Task TryRedrawVisiblePageAsync(this global::PDFKit.PdfControl pdfControl, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (pdfControl != null)
			{
				global::System.ValueTuple<int, int> visiblePageRange = pdfControl.GetVisiblePageRange();
				int item = visiblePageRange.Item1;
				int endPage = visiblePageRange.Item2;
				if (item != -1 && endPage != -1)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
					int? num;
					if (annotationHolderManager == null)
					{
						num = null;
					}
					else
					{
						IAnnotationHolder currentHolder = annotationHolderManager.CurrentHolder;
						if (currentHolder == null)
						{
							num = null;
						}
						else
						{
							PdfPage currentPage = currentHolder.CurrentPage;
							num = ((currentPage != null) ? new int?(currentPage.PageIndex) : null);
						}
					}
					int? num2 = num;
					int valueOrDefault = num2.GetValueOrDefault(-1);
					if (valueOrDefault != -1 && (valueOrDefault <= item || valueOrDefault <= endPage))
					{
						annotationHolderManager.CurrentHolder.Cancel();
					}
					for (int pageIdx = item; pageIdx <= endPage; pageIdx++)
					{
						int num3 = pageIdx;
						PdfDocument document = pdfControl.Document;
						int? num4;
						if (document == null)
						{
							num4 = null;
						}
						else
						{
							PdfPageCollection pages = document.Pages;
							num4 = ((pages != null) ? new int?(pages.Count) : null);
						}
						num2 = num4;
						if (num3 >= num2.GetValueOrDefault(-1))
						{
							break;
						}
						PdfPage page = pdfControl.Document.Pages[pageIdx];
						for (int i = 0; i < 3; i++)
						{
							bool flag = page.IsDisposed;
							if (!flag)
							{
								flag = PdfDocumentStateService.CanDisposePage(page);
							}
							ProgressiveStatus progressiveStatus;
							if (!flag && PdfObjectExtensions.TryGetProgressiveStatus(page, out progressiveStatus))
							{
								flag = progressiveStatus != ProgressiveStatus.ToBeContinued && progressiveStatus != ProgressiveStatus.Failed;
							}
							if (flag)
							{
								try
								{
									PageDisposeHelper.DisposePage(page);
									PdfDocumentStateService.TryRedrawViewerCurrentPage(page);
									break;
								}
								catch
								{
									break;
								}
							}
							await Task.Delay(150, cancellationToken);
						}
						page = null;
					}
				}
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00030DAC File Offset: 0x0002EFAC
		public static bool ShouldDispose(this PdfPage page)
		{
			if (page == null)
			{
				return false;
			}
			PdfDocument document = page.Document;
			if (((document != null) ? document.Pages : null) != null && page.PageIndex == page.Document.Pages.CurrentIndex)
			{
				return false;
			}
			if (page.IsDisposed)
			{
				return false;
			}
			ProgressiveStatus progressiveStatus;
			if (PdfObjectExtensions.TryGetProgressiveStatus(page, out progressiveStatus) && (progressiveStatus == ProgressiveStatus.ToBeContinued || progressiveStatus == ProgressiveStatus.Failed))
			{
				return false;
			}
			object obj;
			if (PdfObjectExtensions.TryGetText(page, out obj) && obj != null)
			{
				return false;
			}
			if (page.Document.FormFill != null && !page.Document.FormFill.IsDisposed && page.Document.FormFill.InterForm != null)
			{
				PdfControlCollections pageControls = page.Document.FormFill.InterForm.GetPageControls(page);
				if (((pageControls != null) ? pageControls.GetFocused() : null) != null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00030E74 File Offset: 0x0002F074
		public static async Task<bool> WaitProgressiveDoneAsync(this PdfPage page, CancellationToken cancellationToken)
		{
			return await page.WaitProgressiveDoneAsync(TimeSpan.Zero, cancellationToken);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00030EC0 File Offset: 0x0002F0C0
		public static Task<bool> WaitProgressiveDoneAsync(this PdfPage page, TimeSpan timeout, CancellationToken cancellationToken)
		{
			PdfObjectExtensions.<WaitProgressiveDoneAsync>d__11 <WaitProgressiveDoneAsync>d__;
			<WaitProgressiveDoneAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<WaitProgressiveDoneAsync>d__.page = page;
			<WaitProgressiveDoneAsync>d__.timeout = timeout;
			<WaitProgressiveDoneAsync>d__.cancellationToken = cancellationToken;
			<WaitProgressiveDoneAsync>d__.<>1__state = -1;
			<WaitProgressiveDoneAsync>d__.<>t__builder.Start<PdfObjectExtensions.<WaitProgressiveDoneAsync>d__11>(ref <WaitProgressiveDoneAsync>d__);
			return <WaitProgressiveDoneAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00030F14 File Offset: 0x0002F114
		public static bool TryGetProgressiveStatus(PdfPage page, out ProgressiveStatus progressiveStatus)
		{
			progressiveStatus = ProgressiveStatus.Ready;
			if (page == null)
			{
				return false;
			}
			if (!page.IsParsed)
			{
				return false;
			}
			if (page.IsDisposed)
			{
				return true;
			}
			bool flag;
			try
			{
				Func<PdfPage, ProgressiveStatus> func = TypeHelper.CreateFieldOrPropertyGetter<PdfPage, ProgressiveStatus>("_progressiveStatus", BindingFlags.Instance | BindingFlags.NonPublic);
				if (func == null)
				{
					flag = false;
				}
				else
				{
					progressiveStatus = func(page);
					flag = true;
				}
			}
			catch
			{
				progressiveStatus = ProgressiveStatus.Ready;
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00030F78 File Offset: 0x0002F178
		private static bool TryGetText(PdfPage page, out object texts)
		{
			texts = null;
			if (page == null)
			{
				return false;
			}
			bool flag;
			try
			{
				Func<PdfPage, object> func = TypeHelper.CreateFieldOrPropertyGetter<PdfPage, object>("_text", BindingFlags.Instance | BindingFlags.NonPublic);
				if (func == null)
				{
					flag = false;
				}
				else
				{
					texts = func(page);
					flag = true;
				}
			}
			catch
			{
				texts = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00030FC8 File Offset: 0x0002F1C8
		public static bool TryGetModificationDate(this PdfAnnotation annot, out DateTimeOffset modificationDate)
		{
			modificationDate = default(DateTimeOffset);
			try
			{
				if (annot == null)
				{
					return false;
				}
				if (string.IsNullOrEmpty(annot.ModificationDate))
				{
					return false;
				}
				return PdfObjectExtensions.TryParseModificationDate(annot.ModificationDate, out modificationDate);
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00031020 File Offset: 0x0002F220
		public static string ToModificationDateString(this DateTimeOffset dateTime)
		{
			return PdfAttributeUtils.ConvertToModificationDateString(dateTime);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00031028 File Offset: 0x0002F228
		public static bool TryParseModificationDate(string modificationDate, out DateTimeOffset dateTime)
		{
			return PdfAttributeUtils.TryParseModificationDate(modificationDate, out dateTime);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00031034 File Offset: 0x0002F234
		public static bool TryParsePageRange(string range, out int[] pageIndexes, out int errorCharIndex)
		{
			pageIndexes = null;
			int[][] array;
			if (PdfObjectExtensions.TryParsePageRangeCore(range, out array, out errorCharIndex))
			{
				pageIndexes = (from c in array.SelectMany((int[] c) => c).Distinct<int>()
					orderby c
					select c).ToArray<int>();
				return true;
			}
			return false;
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x000310A7 File Offset: 0x0002F2A7
		public static bool TryParsePageRange2(string range, out int[][] pageIndexes, out int errorCharIndex)
		{
			return PdfObjectExtensions.TryParsePageRangeCore(range, out pageIndexes, out errorCharIndex);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x000310B4 File Offset: 0x0002F2B4
		private static bool TryParsePageRangeCore(string range, out int[][] pageIndexes, out int errorCharIndex)
		{
			pageIndexes = null;
			errorCharIndex = -1;
			if (string.IsNullOrEmpty(range))
			{
				return false;
			}
			PdfObjectExtensions.<>c__DisplayClass19_0 CS$<>8__locals1;
			CS$<>8__locals1.list = new List<List<int>>();
			PdfObjectExtensions.PageRangeReader pageRangeReader = new PdfObjectExtensions.PageRangeReader(range);
			CS$<>8__locals1.from = -1;
			CS$<>8__locals1.to = -1;
			CS$<>8__locals1.isTo = false;
			int num = -1;
			while (pageRangeReader.HasMore)
			{
				global::System.ValueTuple<PdfObjectExtensions.PageRangeTokenType, string, int> nextToken = pageRangeReader.GetNextToken();
				PdfObjectExtensions.PageRangeTokenType item = nextToken.Item1;
				string item2 = nextToken.Item2;
				int item3 = nextToken.Item3;
				num = item3;
				switch (item)
				{
				case PdfObjectExtensions.PageRangeTokenType.Number:
					if (!CS$<>8__locals1.isTo)
					{
						if (CS$<>8__locals1.from != -1)
						{
							errorCharIndex = item3;
							return false;
						}
						if (!int.TryParse(item2, out CS$<>8__locals1.from))
						{
							errorCharIndex = item3;
							return false;
						}
						continue;
					}
					else
					{
						if (CS$<>8__locals1.to != -1)
						{
							errorCharIndex = item3;
							return false;
						}
						if (!int.TryParse(item2, out CS$<>8__locals1.to))
						{
							errorCharIndex = item3;
							return false;
						}
						if (CS$<>8__locals1.to < CS$<>8__locals1.from)
						{
							errorCharIndex = item3;
							return false;
						}
						continue;
					}
					break;
				case PdfObjectExtensions.PageRangeTokenType.Dash:
					if (((CS$<>8__locals1.from == -1) | CS$<>8__locals1.isTo) || CS$<>8__locals1.to != -1)
					{
						errorCharIndex = item3;
						return false;
					}
					CS$<>8__locals1.isTo = true;
					continue;
				case PdfObjectExtensions.PageRangeTokenType.Comma:
					if (!PdfObjectExtensions.<TryParsePageRangeCore>g__Complete|19_1(ref CS$<>8__locals1))
					{
						errorCharIndex = item3;
						return false;
					}
					continue;
				}
				if (pageRangeReader.HasMore)
				{
					errorCharIndex = item3;
					return false;
				}
			}
			if (!PdfObjectExtensions.<TryParsePageRangeCore>g__Complete|19_1(ref CS$<>8__locals1))
			{
				errorCharIndex = num;
				return false;
			}
			pageIndexes = CS$<>8__locals1.list.Select((List<int> c) => c.OrderBy((int x) => x).ToArray<int>()).ToArray<int[]>();
			return true;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0003123C File Offset: 0x0002F43C
		public static string ConvertToRange(this IEnumerable<int> pageIndexes)
		{
			int[] array;
			return pageIndexes.ConvertToRange(out array);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00031254 File Offset: 0x0002F454
		public static string ConvertToRange(this IEnumerable<int> pageIndexes, out int[] sortedPageIndexes)
		{
			sortedPageIndexes = null;
			if (pageIndexes == null)
			{
				return string.Empty;
			}
			int[] array = pageIndexes.ToArray<int>();
			if (array.Length == 0)
			{
				return string.Empty;
			}
			Array.Sort<int>(array);
			sortedPageIndexes = array;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(array[0] + 1);
			bool flag = false;
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] != array[i - 1])
				{
					if (array[i] - 1 == array[i - 1])
					{
						flag = true;
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append('-').Append(array[i - 1] + 1);
							flag = false;
						}
						stringBuilder.Append(',').Append(array[i] + 1);
					}
				}
			}
			if (flag)
			{
				stringBuilder.Append('-').Append(array[array.Length - 1] + 1);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00031310 File Offset: 0x0002F510
		public static PdfBorderStyle GetBorderStyle(this PdfAnnotation annot)
		{
			if (!annot.IsExists("BS"))
			{
				return null;
			}
			try
			{
				PdfBorderStyle pdfBorderStyle = new PdfBorderStyle(annot.Dictionary["BS"]);
				PdfBorderStyle pdfBorderStyle2 = new PdfBorderStyle();
				float[] dashPattern = pdfBorderStyle.DashPattern;
				pdfBorderStyle2.DashPattern = ((dashPattern != null) ? dashPattern.ToArray<float>() : null);
				pdfBorderStyle2.Style = pdfBorderStyle.Style;
				pdfBorderStyle2.Width = pdfBorderStyle.Width;
				return pdfBorderStyle2;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00031394 File Offset: 0x0002F594
		public static void SetBorderStyle(this PdfAnnotation annot, PdfBorderStyle borderStyle)
		{
			if (annot.Dictionary.ContainsKey("BS") && borderStyle == null)
			{
				annot.Dictionary.Remove("BS");
				return;
			}
			annot.Dictionary["BS"] = borderStyle.Dictionary;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x000313E4 File Offset: 0x0002F5E4
		public static T DeepClone<T>(this T value) where T : PdfTypeBase
		{
			return (T)((object)PdfObjectExtensions.DeepClone(value));
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x000313F8 File Offset: 0x0002F5F8
		public static PdfTypeBase DeepClone(PdfTypeBase value)
		{
			if (value is PdfTypeUnknown)
			{
				return null;
			}
			if (value is PdfTypeNumber || value is PdfTypeString || value is PdfTypeName || value is PdfTypeBoolean || value is PdfTypeNull || value is PdfTypeUnknown)
			{
				return value.Clone(false);
			}
			PdfTypeIndirect pdfTypeIndirect = value as PdfTypeIndirect;
			if (pdfTypeIndirect != null)
			{
				return PdfTypeIndirect.Create(pdfTypeIndirect.List, pdfTypeIndirect.Number);
			}
			PdfTypeArray pdfTypeArray = value as PdfTypeArray;
			if (pdfTypeArray != null)
			{
				PdfTypeArray pdfTypeArray2 = PdfTypeArray.Create();
				for (int i = 0; i < pdfTypeArray.Count; i++)
				{
					pdfTypeArray2.Add(PdfObjectExtensions.DeepClone(pdfTypeArray[i]));
				}
				return pdfTypeArray2;
			}
			PdfTypeDictionary pdfTypeDictionary = value as PdfTypeDictionary;
			if (pdfTypeDictionary != null)
			{
				PdfTypeDictionary pdfTypeDictionary2 = PdfTypeDictionary.Create();
				using (IEnumerator<KeyValuePair<string, PdfTypeBase>> enumerator = pdfTypeDictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, PdfTypeBase> keyValuePair = enumerator.Current;
						pdfTypeDictionary2[keyValuePair.Key] = PdfObjectExtensions.DeepClone(keyValuePair.Value);
					}
					goto IL_0191;
				}
			}
			PdfTypeStream pdfTypeStream = value as PdfTypeStream;
			if (pdfTypeStream != null)
			{
				IntPtr rawData = pdfTypeStream.RawData;
				int length = pdfTypeStream.Length;
				PdfTypeStream pdfTypeStream2 = PdfTypeStream.Create();
				using (PdfTypeDictionary pdfTypeDictionary3 = pdfTypeStream.Dictionary.DeepClone<PdfTypeDictionary>())
				{
					pdfTypeStream2.SetRawData(rawData, length, false);
					if (pdfTypeDictionary3 != null)
					{
						foreach (KeyValuePair<string, PdfTypeBase> keyValuePair2 in pdfTypeDictionary3)
						{
							pdfTypeStream2.Dictionary[keyValuePair2.Key] = keyValuePair2.Value;
						}
						pdfTypeDictionary3.Clear();
					}
				}
				return pdfTypeStream2;
			}
			IL_0191:
			return null;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x000315C0 File Offset: 0x0002F7C0
		[CompilerGenerated]
		internal static AnnotationHolderManager <GetAnnotationHolderManager>g__GetAnnotationHolderManagerCore|1_0(global::PDFKit.PdfControl _pdfControl)
		{
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(_pdfControl);
			if (annotationCanvas == null)
			{
				return null;
			}
			return annotationCanvas.HolderManager;
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x000315D4 File Offset: 0x0002F7D4
		[CompilerGenerated]
		internal static bool <TryParsePageRangeCore>g__Complete|19_1(ref PdfObjectExtensions.<>c__DisplayClass19_0 A_0)
		{
			if (A_0.from == -1)
			{
				return false;
			}
			if (A_0.to == -1)
			{
				if (A_0.isTo)
				{
					return false;
				}
				A_0.list.Add(new List<int> { A_0.from - 1 });
			}
			else if (A_0.from < A_0.to)
			{
				A_0.list.Add(new List<int>(Enumerable.Range(A_0.from - 1, A_0.to - A_0.from + 1)));
			}
			else
			{
				if (A_0.from != A_0.to)
				{
					return false;
				}
				A_0.list.Add(new List<int> { A_0.from - 1 });
			}
			A_0.from = -1;
			A_0.to = -1;
			A_0.isTo = false;
			return true;
		}

		// Token: 0x0200046B RID: 1131
		private class PageRangeReader
		{
			// Token: 0x06002D9F RID: 11679 RVA: 0x000DEDFA File Offset: 0x000DCFFA
			public PageRangeReader(string pageRange)
			{
				if (string.IsNullOrEmpty(pageRange))
				{
					throw new ArgumentException("pageRange");
				}
				this.pageRange = pageRange;
				this.sb = new StringBuilder();
				this.curType = PdfObjectExtensions.PageRangeTokenType.None;
			}

			// Token: 0x17000CA1 RID: 3233
			// (get) Token: 0x06002DA0 RID: 11680 RVA: 0x000DEE2E File Offset: 0x000DD02E
			public bool HasMore
			{
				get
				{
					return this.curIdx < this.pageRange.Length;
				}
			}

			// Token: 0x06002DA1 RID: 11681 RVA: 0x000DEE44 File Offset: 0x000DD044
			[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "type", "value", "startIdx" })]
			public global::System.ValueTuple<PdfObjectExtensions.PageRangeTokenType, string, int> GetNextToken()
			{
				int num = this.curIdx;
				while (this.curIdx < this.pageRange.Length)
				{
					char c = this.pageRange[this.curIdx];
					if (c >= '0' && c <= '9')
					{
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.None)
						{
							this.curType = PdfObjectExtensions.PageRangeTokenType.Number;
						}
						this.sb.Append(c);
					}
					else if (c == ' ')
					{
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.Number)
						{
							break;
						}
						num++;
					}
					else if (c == ',')
					{
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.None)
						{
							this.curType = PdfObjectExtensions.PageRangeTokenType.Comma;
							this.sb.Append(c);
							this.curIdx++;
							break;
						}
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.Number)
						{
							break;
						}
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.Dash)
						{
							break;
						}
					}
					else
					{
						if (c != '-')
						{
							this.curType = PdfObjectExtensions.PageRangeTokenType.None;
							this.sb.Length = 0;
							break;
						}
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.None)
						{
							this.curType = PdfObjectExtensions.PageRangeTokenType.Dash;
							this.sb.Append(c);
							this.curIdx++;
							break;
						}
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.Number)
						{
							break;
						}
						if (this.curType == PdfObjectExtensions.PageRangeTokenType.Comma)
						{
							break;
						}
					}
					this.curIdx++;
				}
				PdfObjectExtensions.PageRangeTokenType pageRangeTokenType = this.curType;
				string text = this.sb.ToString();
				this.curType = PdfObjectExtensions.PageRangeTokenType.None;
				this.sb.Length = 0;
				return new global::System.ValueTuple<PdfObjectExtensions.PageRangeTokenType, string, int>(pageRangeTokenType, text, num);
			}

			// Token: 0x04001938 RID: 6456
			private readonly string pageRange;

			// Token: 0x04001939 RID: 6457
			private int curIdx;

			// Token: 0x0400193A RID: 6458
			private StringBuilder sb;

			// Token: 0x0400193B RID: 6459
			private PdfObjectExtensions.PageRangeTokenType curType;
		}

		// Token: 0x0200046C RID: 1132
		private enum PageRangeTokenType
		{
			// Token: 0x0400193D RID: 6461
			None,
			// Token: 0x0400193E RID: 6462
			Number,
			// Token: 0x0400193F RID: 6463
			Dash,
			// Token: 0x04001940 RID: 6464
			Comma
		}
	}
}
