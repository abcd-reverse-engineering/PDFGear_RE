using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Models.Annotations;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x0200008F RID: 143
	internal class PDFEraserUtil
	{
		// Token: 0x06000972 RID: 2418 RVA: 0x0002F124 File Offset: 0x0002D324
		public bool DeleteInk(PdfDocument Document, int pageIdx, Point pos, ToolbarSettingInkEraserModel inkEraserModel)
		{
			int i;
			Predicate<IndexedPdfInkAnnotation> <>9__1;
			int j;
			for (i = 0; i < Document.Pages[pageIdx].Annots.Count; i = j + 1)
			{
				PdfAnnotation pdfAnnotation = Document.Pages[pageIdx].Annots[i];
				PdfInkAnnotation pdfInkAnnotation = pdfAnnotation as PdfInkAnnotation;
				if (pdfInkAnnotation != null && AnnotationHitTestHelper.HitTest(pdfAnnotation, pos))
				{
					PdfPage page = pdfAnnotation.Page;
					IPdfScrollInfoInternal pdfControl = global::PDFKit.PdfControl.GetPdfControl((page != null) ? page.Document : null);
					InkAnnotation inkAnnotation = (InkAnnotation)AnnotationFactory.Create(pdfInkAnnotation);
					Point point;
					pdfControl.TryGetPagePoint(pdfAnnotation.Page.PageIndex, pos, out point);
					int radius = inkEraserModel.SelectSize;
					Func<FS_POINTF, bool> <>9__0;
					foreach (PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection in pdfInkAnnotation.InkList)
					{
						if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Partial)
						{
							IEnumerable<FS_POINTF> enumerable = pdfLinePointCollection;
							Func<FS_POINTF, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (FS_POINTF pt) => this.IsInRangeOfPoint(pt, point, (float)radius));
							}
							List<FS_POINTF> list = enumerable.Where(func).ToList<FS_POINTF>();
							if (list.Count > 0)
							{
								IndexedPdfInkAnnotation indexedPdfInkAnnotation = new IndexedPdfInkAnnotation
								{
									PdfInkAnnotation = pdfInkAnnotation,
									Index = i
								};
								List<IndexedPdfInkAnnotation> list2 = this.pdfInks;
								Predicate<IndexedPdfInkAnnotation> predicate;
								if ((predicate = <>9__1) == null)
								{
									predicate = (<>9__1 = (IndexedPdfInkAnnotation X) => X.Index == i);
								}
								if (list2.FindAll(predicate).Count <= 0)
								{
									this.pdfInks.Add(indexedPdfInkAnnotation);
								}
								this.DeleteInk(list, pdfInkAnnotation, Document.Pages[pageIdx], pdfLinePointCollection, point, (float)radius);
							}
						}
						else if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Whole && this.HitTest(pdfLinePointCollection, point))
						{
							this.DeleteWhole(Document, pdfInkAnnotation);
						}
					}
				}
				j = i;
			}
			return this.deletePageIndex != -1;
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0002F368 File Offset: 0x0002D568
		public void MouseStyle(PdfDocument Document, AnnotationCanvas annotationCanvas, ToolbarSettingInkEraserModel inkEraserModel, MainViewModel VM)
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(Document);
			if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Partial)
			{
				int num = (int)((float)(inkEraserModel.SelectSize * 2) * VM.ViewToolbar.DocZoom);
				annotationCanvas.Cursor = BitmapCursor.CreateCustomCursor(inkEraserModel, VM, num);
				pdfControl.Viewer.OverrideCursor = BitmapCursor.CreateCustomCursor(inkEraserModel, VM, num);
				return;
			}
			if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Whole)
			{
				Cursor cursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/Style/Resources/Annonate/Eraser.cur")).Stream);
				annotationCanvas.Cursor = cursor;
				pdfControl.Viewer.OverrideCursor = cursor;
			}
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0002F3F8 File Offset: 0x0002D5F8
		public void MouseDownRecord(int pageindex, MainViewModel VM, PdfDocument Document, ToolbarSettingInkEraserModel inkEraserModel, Point pos, Point Point)
		{
			this.changeRecord.Clear();
			if (pageindex < 0)
			{
				pageindex = VM.SelectedPageIndex;
			}
			this.pdfAnnotations = Document.Pages[pageindex].Annots;
			if (this.pdfAnnotations != null)
			{
				int i;
				Predicate<IndexedPdfInkAnnotation> <>9__1;
				int j;
				for (i = 0; i < Document.Pages[pageindex].Annots.Count; i = j + 1)
				{
					PdfAnnotation pdfAnnotation = Document.Pages[pageindex].Annots[i];
					PdfInkAnnotation pdfInkAnnotation = pdfAnnotation as PdfInkAnnotation;
					if (pdfInkAnnotation != null)
					{
						this.AddToChangeRecord(pdfInkAnnotation, i);
					}
					PdfInkAnnotation pdfInkAnnotation2 = pdfAnnotation as PdfInkAnnotation;
					if (pdfInkAnnotation2 != null && AnnotationHitTestHelper.HitTest(pdfAnnotation, pos))
					{
						PdfPage page = pdfAnnotation.Page;
						global::PDFKit.PdfControl.GetPdfControl((page != null) ? page.Document : null);
						InkAnnotation inkAnnotation = (InkAnnotation)AnnotationFactory.Create(pdfInkAnnotation2);
						int radius = inkEraserModel.SelectSize / 2;
						if (radius == 0)
						{
							radius = 1;
						}
						Func<FS_POINTF, bool> <>9__0;
						foreach (PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection in pdfInkAnnotation2.InkList)
						{
							if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Partial)
							{
								IEnumerable<FS_POINTF> enumerable = pdfLinePointCollection;
								Func<FS_POINTF, bool> func;
								if ((func = <>9__0) == null)
								{
									func = (<>9__0 = (FS_POINTF pt) => this.IsInRangeOfPoint(pt, Point, (float)radius));
								}
								List<FS_POINTF> list = enumerable.Where(func).ToList<FS_POINTF>();
								if (list.Count > 0)
								{
									IndexedPdfInkAnnotation indexedPdfInkAnnotation = new IndexedPdfInkAnnotation
									{
										PdfInkAnnotation = pdfInkAnnotation2,
										Index = i
									};
									List<IndexedPdfInkAnnotation> list2 = this.pdfInks;
									Predicate<IndexedPdfInkAnnotation> predicate;
									if ((predicate = <>9__1) == null)
									{
										predicate = (<>9__1 = (IndexedPdfInkAnnotation X) => X.Index == i);
									}
									if (list2.FindAll(predicate).Count <= 0)
									{
										this.pdfInks.Add(indexedPdfInkAnnotation);
									}
									this.DeleteInk(list, pdfInkAnnotation2, Document.Pages[pageindex], pdfLinePointCollection, Point, (float)radius);
								}
							}
							else if (inkEraserModel.IsPartial == ToolbarSettingInkEraserModel.EraserType.Whole && this.HitTest(pdfLinePointCollection, Point))
							{
								this.DeleteWhole(Document, pdfInkAnnotation2);
							}
						}
					}
					j = i;
				}
			}
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0002F69C File Offset: 0x0002D89C
		public async Task ReflashPage(PdfPage page)
		{
			await page.TryRedrawPageAsync(default(CancellationToken));
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0002F6E0 File Offset: 0x0002D8E0
		private bool IsInRangeOfPoint(FS_POINTF pt1, Point pt2, float distance)
		{
			return Math.Pow(Math.Abs((double)pt1.X - pt2.X), 2.0) + Math.Pow(Math.Abs((double)pt1.Y - pt2.Y), 2.0) < Math.Pow((double)distance, 2.0);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0002F748 File Offset: 0x0002D948
		public async Task DeleteWhole(PdfDocument Document, PdfInkAnnotation inkannot)
		{
			if (Document != null)
			{
				if (!(inkannot == null))
				{
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(Document);
					if (pdfControl != null)
					{
						AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
						if (annotationHolderManager != null)
						{
							await annotationHolderManager.DeleteAnnotationAsync(inkannot, false);
						}
					}
				}
			}
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0002F794 File Offset: 0x0002D994
		public async void CommitRedoRecords(MainViewModel VM, PdfDocument Document)
		{
			int num = this.deletePageIndex;
			if (num != -1)
			{
				this.record2.Clear();
				this.record.AddRange(this.changeRecord);
				if (VM != null)
				{
					PageEditorViewModel pageEditors = VM.PageEditors;
					if (pageEditors != null)
					{
						pageEditors.NotifyPageAnnotationChanged(num);
					}
				}
				await this.RedoUndoRecord(VM, Document.Pages[num]);
				this.deletePageIndex = -1;
			}
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0002F7DC File Offset: 0x0002D9DC
		private void DeleteInk(List<FS_POINTF> matchedPoints, PdfInkAnnotation inkannot, PdfPage page, PdfLinePointCollection<PdfInkAnnotation> list, Point Mousepoint, float radius)
		{
			PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection = new PdfLinePointCollection<PdfInkAnnotation>();
			PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection2 = new PdfLinePointCollection<PdfInkAnnotation>();
			try
			{
				FS_POINTF fs_POINTF = new FS_POINTF(Mousepoint.X, Mousepoint.Y);
				FS_POINTF[] array = this.MathGetPoints(list, Mousepoint, radius, inkannot, page);
				this.Distance(fs_POINTF, matchedPoints[0]);
				if (array != null)
				{
					if (array.Length == 1)
					{
						int num = list.IndexOf(matchedPoints[matchedPoints.Count - 1]);
						if (matchedPoints[0] == array[0])
						{
							list.Remove(matchedPoints[0]);
						}
						else if (num - list.Count + 1 < 0)
						{
							foreach (FS_POINTF fs_POINTF2 in matchedPoints)
							{
								list.Remove(fs_POINTF2);
							}
							list.Insert(0, array[0]);
						}
						else
						{
							foreach (FS_POINTF fs_POINTF3 in matchedPoints)
							{
								list.Remove(fs_POINTF3);
							}
							list.Add(array[0]);
						}
						inkannot.RegenerateAppearances();
						this.deletePageIndex = page.PageIndex;
					}
					else if (array.Length == 2)
					{
						List<int> list2 = new List<int>();
						for (int i = 0; i < matchedPoints.Count; i++)
						{
							if (!list2.Contains(list.IndexOf(matchedPoints[i])))
							{
								list2.Add(list.IndexOf(matchedPoints[i]));
							}
						}
						if (list2[0] == 0)
						{
							for (int j = list2.Count - 1; j >= 0; j--)
							{
								list.RemoveAt(list2[j]);
							}
							if (list.Count != 0)
							{
								if (this.Distance(array[0], list[0]) < this.Distance(array[1], list[0]))
								{
									list.Insert(0, new FS_POINTF(array[0].X, array[0].Y));
									list.Add(array[1]);
								}
								else
								{
									list.Insert(0, new FS_POINTF(array[1].X, array[1].Y));
									list.Add(array[0]);
								}
							}
						}
						else if (list2[0] == list.Count - 1)
						{
							for (int k = list2.Count - 1; k >= 0; k--)
							{
								list.RemoveAt(list2[k]);
							}
						}
						else
						{
							double num2 = this.Distance(array[0], list[list2[0] - 1]);
							double num3 = this.Distance(array[1], list[list2[0] - 1]);
							double num4 = ((list2[list2.Count - 1] + 1 == list.Count) ? 0.0 : this.Distance(array[0], list[list2[list2.Count - 1] + 1]));
							double num5 = ((list2[list2.Count - 1] + 1 == list.Count) ? 0.0 : this.Distance(array[1], list[list2[list2.Count - 1] + 1]));
							if (num2 + num5 - num3 - num4 > 0.0)
							{
								for (int l = 0; l < list2[0]; l++)
								{
									pdfLinePointCollection.Add(list[l]);
								}
								pdfLinePointCollection.Add(array[1]);
								pdfLinePointCollection2.Add(array[0]);
								for (int m = list2[list2.Count - 1] + 1; m < list.Count; m++)
								{
									pdfLinePointCollection2.Add(list[m]);
								}
							}
							else
							{
								for (int n = 0; n < list2[0]; n++)
								{
									pdfLinePointCollection.Add(list[n]);
								}
								pdfLinePointCollection.Add(array[0]);
								pdfLinePointCollection2.Add(array[1]);
								for (int num6 = list2[list2.Count - 1] + 1; num6 < list.Count; num6++)
								{
									pdfLinePointCollection2.Add(list[num6]);
								}
							}
							inkannot.InkList.Remove(list);
							inkannot.InkList.Add(pdfLinePointCollection);
							inkannot.InkList.Add(pdfLinePointCollection2);
						}
						inkannot.RegenerateAppearances();
						this.deletePageIndex = page.PageIndex;
					}
					else if (array.Length >= 3)
					{
						int num7 = list.IndexOf(matchedPoints[0]);
						if (num7 >= 0)
						{
							for (int num8 = 0; num8 < num7; num8++)
							{
								pdfLinePointCollection.Add(list[num8]);
							}
						}
						if (num7 < list.Count)
						{
							for (int num9 = num7 + 1; num9 < list.Count; num9++)
							{
								pdfLinePointCollection2.Add(list[num9]);
							}
						}
						inkannot.InkList.Remove(list);
						if (pdfLinePointCollection.Count > 0)
						{
							inkannot.InkList.Add(pdfLinePointCollection);
						}
						if (pdfLinePointCollection2.Count > 0)
						{
							inkannot.InkList.Add(pdfLinePointCollection2);
						}
						inkannot.RegenerateAppearances();
						this.deletePageIndex = page.PageIndex;
					}
				}
				else
				{
					foreach (FS_POINTF fs_POINTF4 in matchedPoints)
					{
						list.Remove(fs_POINTF4);
					}
					if (list.Count <= 0)
					{
						inkannot.InkList.Remove(list);
					}
					inkannot.RegenerateAppearances();
					this.deletePageIndex = page.PageIndex;
				}
			}
			catch
			{
				int num10 = list.IndexOf(matchedPoints[0]);
				if (num10 >= 0)
				{
					for (int num11 = 0; num11 < num10; num11++)
					{
						pdfLinePointCollection.Add(list[num11]);
					}
				}
				if (num10 < list.Count)
				{
					for (int num12 = num10 + 1; num12 < list.Count; num12++)
					{
						pdfLinePointCollection2.Add(list[num12]);
					}
				}
				inkannot.InkList.Remove(list);
				if (pdfLinePointCollection.Count > 0)
				{
					inkannot.InkList.Add(pdfLinePointCollection);
				}
				if (pdfLinePointCollection2.Count > 0)
				{
					inkannot.InkList.Add(pdfLinePointCollection2);
				}
				inkannot.RegenerateAppearances();
				this.deletePageIndex = page.PageIndex;
			}
			finally
			{
				foreach (PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection3 in inkannot.InkList)
				{
					if (pdfLinePointCollection3.Count == 1)
					{
						inkannot.InkList.Remove(pdfLinePointCollection3);
					}
				}
			}
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0002FF68 File Offset: 0x0002E168
		private void TestInk(PdfLinePointCollection<PdfInkAnnotation> list)
		{
			if (this.Distance(list[list.Count - 2], list[list.Count - 1]) >= 3.0)
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0002FFA8 File Offset: 0x0002E1A8
		public bool HitTest(PdfLinePointCollection<PdfInkAnnotation> ink, Point point)
		{
			EllipseGeometry ellipseGeometry = new EllipseGeometry
			{
				Center = point,
				RadiusX = 2.0,
				RadiusY = 2.0
			};
			PolyLineSegment polyLineSegment = new PolyLineSegment();
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = ink[0].ToPoint(),
				Segments = { polyLineSegment }
			};
			for (int i = 1; i < ink.Count; i++)
			{
				polyLineSegment.Points.Add(ink[i].ToPoint());
			}
			PathGeometry pathGeometry = new PathGeometry
			{
				Figures = { pathFigure }
			};
			PathGeometry widenedPathGeometry = pathGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 0.01), 0.5, ToleranceType.Absolute);
			IntersectionDetail intersectionDetail = ellipseGeometry.FillContainsWithDetail(pathGeometry, 0.5, ToleranceType.Absolute);
			if (intersectionDetail == IntersectionDetail.Intersects || intersectionDetail == IntersectionDetail.FullyContains)
			{
				PathGeometry widenedPathGeometry2 = ellipseGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 0.01), 0.5, ToleranceType.Absolute);
				if (Geometry.Combine(widenedPathGeometry, widenedPathGeometry2, GeometryCombineMode.Intersect, null, 0.5, ToleranceType.Absolute).Figures.Select(delegate(PathFigure c)
				{
					Rect bounds = new PathGeometry(new PathFigure[] { c.Clone() }).Bounds;
					return new Point(bounds.X + bounds.Width / 2.0, bounds.Y + bounds.Height / 2.0);
				}).ToArray<Point>().Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x000300FC File Offset: 0x0002E2FC
		public FS_POINTF[] MathGetPoints(PdfLinePointCollection<PdfInkAnnotation> ink, Point point, float radius, PdfInkAnnotation inkannot, PdfPage page)
		{
			EllipseGeometry ellipseGeometry = new EllipseGeometry
			{
				Center = point,
				RadiusX = (double)radius + 0.2,
				RadiusY = (double)radius + 0.2
			};
			PolyLineSegment polyLineSegment = new PolyLineSegment();
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = ink[0].ToPoint(),
				Segments = { polyLineSegment }
			};
			for (int i = 1; i < ink.Count; i++)
			{
				polyLineSegment.Points.Add(ink[i].ToPoint());
			}
			PathGeometry pathGeometry = new PathGeometry
			{
				Figures = { pathFigure }
			};
			PathGeometry widenedPathGeometry = pathGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 0.01), 0.5, ToleranceType.Absolute);
			if (ellipseGeometry.FillContainsWithDetail(pathGeometry, 0.5, ToleranceType.Absolute) == IntersectionDetail.Intersects)
			{
				PathGeometry widenedPathGeometry2 = ellipseGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 0.01), 0.5, ToleranceType.Absolute);
				Point[] array = Geometry.Combine(widenedPathGeometry, widenedPathGeometry2, GeometryCombineMode.Intersect, null, 0.5, ToleranceType.Absolute).Figures.Select(delegate(PathFigure c)
				{
					Rect bounds = new PathGeometry(new PathFigure[] { c.Clone() }).Bounds;
					return new Point(bounds.X + bounds.Width / 2.0, bounds.Y + bounds.Height / 2.0);
				}).ToArray<Point>();
				FS_POINTF[] array2 = new FS_POINTF[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = new FS_POINTF(array[j].X, array[j].Y);
				}
				return array2;
			}
			return null;
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x00030295 File Offset: 0x0002E495
		public void AddToChangeRecord(PdfInkAnnotation inkannot, int i)
		{
			this.changeRecord.Add((InkAnnotation)AnnotationFactory.Create(inkannot));
			this.changeRecord[this.changeRecord.Count - 1].AnnotIndex = i;
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x000302CB File Offset: 0x0002E4CB
		public void AddToRecord2(PdfInkAnnotation inkannot, int i)
		{
			this.record2.Add((InkAnnotation)AnnotationFactory.Create(inkannot));
			this.record2[this.record2.Count - 1].AnnotIndex = i;
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x00030301 File Offset: 0x0002E501
		public void AddToRecord3(PdfInkAnnotation inkannot, int i)
		{
			this.record.Add((InkAnnotation)AnnotationFactory.Create(inkannot));
			this.record[this.record.Count - 1].AnnotIndex = i;
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x00030338 File Offset: 0x0002E538
		public async Task RedoUndoRecord(MainViewModel vm, PdfPage page)
		{
			PDFEraserUtil.<>c__DisplayClass20_0 CS$<>8__locals1 = new PDFEraserUtil.<>c__DisplayClass20_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.page = page;
			CS$<>8__locals1.pageIndex = CS$<>8__locals1.page.PageIndex;
			await vm.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
			{
				PDFEraserUtil.<>c__DisplayClass20_0.<<RedoUndoRecord>b__0>d <<RedoUndoRecord>b__0>d;
				<<RedoUndoRecord>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<RedoUndoRecord>b__0>d.<>4__this = CS$<>8__locals1;
				<<RedoUndoRecord>b__0>d.doc = doc;
				<<RedoUndoRecord>b__0>d.<>1__state = -1;
				<<RedoUndoRecord>b__0>d.<>t__builder.Start<PDFEraserUtil.<>c__DisplayClass20_0.<<RedoUndoRecord>b__0>d>(ref <<RedoUndoRecord>b__0>d);
				return <<RedoUndoRecord>b__0>d.<>t__builder.Task;
			}, delegate(PdfDocument doc)
			{
				PDFEraserUtil.<>c__DisplayClass20_0.<<RedoUndoRecord>b__1>d <<RedoUndoRecord>b__1>d;
				<<RedoUndoRecord>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<RedoUndoRecord>b__1>d.<>4__this = CS$<>8__locals1;
				<<RedoUndoRecord>b__1>d.doc = doc;
				<<RedoUndoRecord>b__1>d.<>1__state = -1;
				<<RedoUndoRecord>b__1>d.<>t__builder.Start<PDFEraserUtil.<>c__DisplayClass20_0.<<RedoUndoRecord>b__1>d>(ref <<RedoUndoRecord>b__1>d);
				return <<RedoUndoRecord>b__1>d.<>t__builder.Task;
			}, "");
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0003038C File Offset: 0x0002E58C
		public FS_POINTF GetIntersectionPoint(FS_POINTF point1, FS_POINTF point2, FS_POINTF center, double radius)
		{
			if (this.Distance(center, point1, point2) > radius)
			{
				return new FS_POINTF(float.NaN, float.NaN);
			}
			if ((double)Math.Abs(point2.X - point1.X) < 1E-06)
			{
				double num = (double)point1.X;
				double num2 = Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow((double)(point1.X - center.X), 2.0));
				if (point1.Y - point2.Y > 0f)
				{
					num2 += (double)center.Y;
				}
				else
				{
					num2 = (double)center.Y - num2;
				}
				return new FS_POINTF((float)num, (float)num2);
			}
			if ((double)Math.Abs(point2.Y - point1.Y) < 1E-06)
			{
				double num2 = (double)point2.Y;
				double num = Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow((double)(point1.Y - center.Y), 2.0));
				if (point1.X - point2.X > 0f)
				{
					num = (double)center.X + num;
				}
				else
				{
					num = (double)center.X - num;
				}
				return new FS_POINTF((float)num, (float)num2);
			}
			double num3 = (double)((point2.Y - point1.Y) / (point2.X - point1.X));
			double num4 = (double)point1.Y - num3 * (double)point1.X;
			float y = center.Y;
			FS_POINTF[] intersectionPoints = PDFEraserUtil.GetIntersectionPoints(num3, num4, (double)center.X, (double)center.Y, radius);
			if (intersectionPoints.Length == 2)
			{
				if (intersectionPoints[0].X >= Math.Min(point1.X, point2.X) && intersectionPoints[0].X <= Math.Max(point1.X, point2.X) && intersectionPoints[0].Y >= Math.Min(point1.Y, point2.Y) && intersectionPoints[0].Y <= Math.Max(point1.Y, point2.Y))
				{
					return intersectionPoints[0];
				}
				if (intersectionPoints[1].X >= Math.Min(point1.X, point2.X) && intersectionPoints[1].X <= Math.Max(point1.X, point2.X) && intersectionPoints[1].Y >= Math.Min(point1.Y, point2.Y) && intersectionPoints[1].Y <= Math.Max(point1.Y, point2.Y))
				{
					return intersectionPoints[1];
				}
				return new FS_POINTF(float.NaN, float.NaN);
			}
			else
			{
				if (intersectionPoints.Length != 1)
				{
					return new FS_POINTF(float.NaN, float.NaN);
				}
				if (intersectionPoints[0].X >= Math.Min(point1.X, point2.X) && intersectionPoints[0].X <= Math.Max(point1.X, point2.X) && intersectionPoints[0].Y >= Math.Min(point1.Y, point2.Y) && intersectionPoints[0].Y <= Math.Max(point1.Y, point2.Y))
				{
					return intersectionPoints[0];
				}
				return new FS_POINTF(float.NaN, float.NaN);
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00030730 File Offset: 0x0002E930
		public static FS_POINTF[] GetIntersectionPoints(double k, double b, double xc, double yc, double r)
		{
			double num = k * k + 1.0;
			double num2 = 2.0 * (k * b - k * yc - xc);
			double num3 = yc * yc + b * b + xc * xc - 2.0 * b * yc - r * r;
			double num4 = num2 * num2 - 4.0 * num * num3;
			if (num4 < 0.0)
			{
				return new FS_POINTF[0];
			}
			if (num4 == 0.0)
			{
				double num5 = -num2 / (2.0 * num);
				double num6 = k * num5 + b;
				return new FS_POINTF[]
				{
					new FS_POINTF((float)num5, (float)num6)
				};
			}
			double num7 = (-num2 - Math.Sqrt(num4)) / (2.0 * num);
			double num8 = k * num7 + b;
			double num9 = (-num2 + Math.Sqrt(num4)) / (2.0 * num);
			double num10 = k * num9 + b;
			return new FS_POINTF[]
			{
				new FS_POINTF((float)num7, (float)num8),
				new FS_POINTF((float)num9, (float)num10)
			};
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0003084C File Offset: 0x0002EA4C
		private double Distance(FS_POINTF point1, FS_POINTF point2)
		{
			return Math.Sqrt(Math.Pow((double)(point1.X - point2.X), 2.0) + Math.Pow((double)(point1.Y - point2.Y), 2.0));
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0003089C File Offset: 0x0002EA9C
		private double Distance(FS_POINTF point, FS_POINTF point1, FS_POINTF point2)
		{
			double num = (double)point.X;
			double num2 = (double)point.Y;
			double num3 = (double)point1.X;
			double num4 = (double)point1.Y;
			double num5 = (double)point2.X;
			double num6 = (double)point2.Y;
			double num7 = num6 - num4;
			double num8 = num3 - num5;
			double num9 = num5 * num4 - num3 * num6;
			return Math.Abs(num7 * num + num8 * num2 + num9) / Math.Sqrt(num7 * num7 + num8 * num8);
		}

		// Token: 0x0400046E RID: 1134
		private List<InkAnnotation> record = new List<InkAnnotation>();

		// Token: 0x0400046F RID: 1135
		private List<IndexedPdfInkAnnotation> pdfInks = new List<IndexedPdfInkAnnotation>();

		// Token: 0x04000470 RID: 1136
		private List<InkAnnotation> record2 = new List<InkAnnotation>();

		// Token: 0x04000471 RID: 1137
		private List<InkAnnotation> changeRecord = new List<InkAnnotation>();

		// Token: 0x04000472 RID: 1138
		private PdfAnnotationCollection pdfAnnotations;

		// Token: 0x04000473 RID: 1139
		private int deletePageIndex = -1;
	}
}
