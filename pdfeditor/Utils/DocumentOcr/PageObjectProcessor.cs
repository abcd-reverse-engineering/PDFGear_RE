using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common.ImageAnalyze;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using PDFKit.Services;
using PDFKit.Utils;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Utils.DocumentOcr
{
	// Token: 0x020000D6 RID: 214
	internal class PageObjectProcessor
	{
		// Token: 0x06000BCD RID: 3021 RVA: 0x0003E6A0 File Offset: 0x0003C8A0
		public static async Task<bool> ProcessPdfPagesAsync(PdfDocument document, IEnumerable<int> indexProvider, int count, PageRotate rotation, CultureInfo[] cultureInfoArray, IProgress<int> progress, CancellationToken cancellationToken)
		{
			PageObjectProcessor.<>c__DisplayClass5_0 CS$<>8__locals1 = new PageObjectProcessor.<>c__DisplayClass5_0();
			CS$<>8__locals1.document = document;
			CS$<>8__locals1.rotation = rotation;
			CS$<>8__locals1.cultureInfoArray = cultureInfoArray;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			int total = 0;
			PageObjectProcessor.TaskQueue<bool> queue = new PageObjectProcessor.TaskQueue<bool>();
			if (progress != null)
			{
				progress.Report(0);
			}
			foreach (int num in indexProvider)
			{
				PageObjectProcessor.<>c__DisplayClass5_1 CS$<>8__locals2 = new PageObjectProcessor.<>c__DisplayClass5_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.i = num;
				queue.Enqueue(delegate
				{
					PageObjectProcessor.<>c__DisplayClass5_1.<<ProcessPdfPagesAsync>b__0>d <<ProcessPdfPagesAsync>b__0>d;
					<<ProcessPdfPagesAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
					<<ProcessPdfPagesAsync>b__0>d.<>4__this = CS$<>8__locals2;
					<<ProcessPdfPagesAsync>b__0>d.<>1__state = -1;
					<<ProcessPdfPagesAsync>b__0>d.<>t__builder.Start<PageObjectProcessor.<>c__DisplayClass5_1.<<ProcessPdfPagesAsync>b__0>d>(ref <<ProcessPdfPagesAsync>b__0>d);
					return <<ProcessPdfPagesAsync>b__0>d.<>t__builder.Task;
				});
			}
			bool anyPageProcessSucceeded = false;
			while (queue.HasNext)
			{
				if (CS$<>8__locals1.cancellationToken.IsCancellationRequested)
				{
					return false;
				}
				TaskAwaiter<bool> taskAwaiter = queue.Next().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					anyPageProcessSucceeded = true;
				}
				total++;
				if (progress != null)
				{
					progress.Report(Math.Min(total, count));
				}
			}
			return anyPageProcessSucceeded;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x0003E718 File Offset: 0x0003C918
		public static async Task<bool> ProcessPdfPageAsync(PdfDocument document, int pageIndex, PageRotate rotation, CultureInfo[] cultureInfoArray, CancellationToken cancellationToken)
		{
			bool flag2;
			if (document == null || pageIndex < 0 || pageIndex >= document.Pages.Count)
			{
				flag2 = false;
			}
			else
			{
				bool flag = false;
				await Task.Run(delegate
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (document.IsDisposed)
					{
						return;
					}
					PdfFormObject pdfFormObject = PdfFormObject.Create(document.Pages[pageIndex]);
					IntPtr intPtr = IntPtr.Zero;
					PageRotate pageRotate;
					float width;
					float height;
					try
					{
						intPtr = Pdfium.FPDF_LoadPage(document.Handle, pageIndex);
						int i;
						for (i = (int)(Pdfium.FPDFPage_GetRotation(intPtr) + (int)rotation); i > 3; i -= 4)
						{
						}
						while (i < 0)
						{
							i += 4;
						}
						pageRotate = (PageRotate)i;
						float num;
						float num2;
						using (PdfPage pdfPage = PdfPage.FromHandle(document, intPtr, pageIndex, true))
						{
							FS_SIZEF effectiveSize = pdfPage.GetEffectiveSize(PageRotate.Normal, false);
							num = (width = effectiveSize.Width);
							num2 = (height = effectiveSize.Height);
							if (pageRotate == PageRotate.Rotate90 || pageRotate == PageRotate.Rotate270)
							{
								num = height;
								num2 = width;
							}
							PageObjectProcessor.RemoveTextObject(pdfPage.PageObjects, false);
						}
						using (MemoryStream memoryStream = new MemoryStream())
						{
							PageObjectProcessor.RenderPageToTiffImage(intPtr, (double)num, (double)num2, rotation, memoryStream);
							if (memoryStream.CanSeek)
							{
								memoryStream.Seek(0L, SeekOrigin.Begin);
							}
							Pdfium.FPDF_ClosePage(intPtr);
							intPtr = IntPtr.Zero;
							foreach (PdfPageObject pdfPageObject in PageObjectProcessor.GetTextObjects(document, memoryStream, cultureInfoArray))
							{
								cancellationToken.ThrowIfCancellationRequested();
								pdfFormObject.PageObjects.Add(pdfPageObject);
								flag = true;
							}
						}
					}
					finally
					{
						if (intPtr != IntPtr.Zero)
						{
							Pdfium.FPDF_ClosePage(intPtr);
						}
					}
					if (flag)
					{
						PageObjectProcessor.RemoveTextObject(document.Pages[pageIndex].PageObjects, true);
						FS_RECTF effectiveBox = document.Pages[pageIndex].GetEffectiveBox(PageRotate.Normal, false);
						if (pageRotate != PageRotate.Normal || effectiveBox.left != 0f || effectiveBox.bottom != 0f)
						{
							pdfFormObject.CalcBoundingBox();
							FS_RECTF boundingBox = pdfFormObject.BoundingBox;
							FS_MATRIX matrix = pdfFormObject.Matrix;
							if (pageRotate == PageRotate.Rotate90)
							{
								matrix.Translate(0f, -width, false);
							}
							else if (pageRotate == PageRotate.Rotate180)
							{
								matrix.Translate(-width, -height, false);
							}
							if (pageRotate == PageRotate.Rotate270)
							{
								matrix.Translate(-height, 0f, false);
							}
							PdfRotateUtils.RotateMatrix(pageRotate, matrix);
							if (effectiveBox.left != 0f || effectiveBox.bottom != 0f)
							{
								matrix.Translate(effectiveBox.left, effectiveBox.bottom, false);
							}
							pdfFormObject.Matrix = matrix;
						}
						document.Pages[pageIndex].PageObjects.Add(pdfFormObject);
						document.Pages[pageIndex].GenerateContentAdvance(false);
					}
				}, cancellationToken);
				if (flag)
				{
					await document.Pages[pageIndex].TryRedrawPageAsync(cancellationToken);
				}
				if (PdfDocumentStateService.CanDisposePage(document.Pages[pageIndex]))
				{
					document.Pages[pageIndex].Dispose();
				}
				flag2 = flag;
			}
			return flag2;
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x0003E77C File Offset: 0x0003C97C
		private static void RenderPageToTiffImage(IntPtr pageHandle, double rotatedPageWidth, double rotatedPageHeight, PageRotate rotation, Stream outputStream)
		{
			Size imageSize = ImageAnalyzer.GetImageSize(new Size(rotatedPageWidth, rotatedPageHeight), 72f, 400f);
			using (PdfBitmap pdfBitmap = new PdfBitmap((int)Math.Ceiling(imageSize.Width), (int)Math.Ceiling(imageSize.Height), BitmapFormats.FXDIB_Argb))
			{
				pdfBitmap.FillRect(0, 0, pdfBitmap.Width, pdfBitmap.Height, FS_COLOR.White);
				Pdfium.FPDF_RenderPageBitmap(pdfBitmap.Handle, pageHandle, 0, 0, pdfBitmap.Width, pdfBitmap.Height, rotation, RenderFlags.FPDF_NONE);
				pdfBitmap.Image.Save(outputStream, ImageFormat.Tiff);
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0003E828 File Offset: 0x0003CA28
		private static IEnumerable<PdfPageObject> GetTextObjects(PdfDocument document, Stream imageStream, CultureInfo[] cultureInfoArray)
		{
			PageObjectProcessor.<GetTextObjects>d__8 <GetTextObjects>d__ = new PageObjectProcessor.<GetTextObjects>d__8(-2);
			<GetTextObjects>d__.<>3__document = document;
			<GetTextObjects>d__.<>3__imageStream = imageStream;
			<GetTextObjects>d__.<>3__cultureInfoArray = cultureInfoArray;
			return <GetTextObjects>d__;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0003E848 File Offset: 0x0003CA48
		private static void RemoveTextObject(PdfPageObjectsCollection pageObjects, bool onlyRemoveHiddenObject)
		{
			if (pageObjects == null)
			{
				return;
			}
			for (int i = pageObjects.Count - 1; i >= 0; i--)
			{
				PdfPageObject pdfPageObject = pageObjects[i];
				if (pdfPageObject.ObjectType == PageObjectTypes.PDFPAGE_TEXT)
				{
					if (!onlyRemoveHiddenObject || !SearchableDocumentHelper.IsVisibleTextObject((PdfTextObject)pdfPageObject))
					{
						pageObjects.RemoveAt(i);
					}
				}
				else if (pdfPageObject.ObjectType == PageObjectTypes.PDFPAGE_FORM)
				{
					PageObjectProcessor.RemoveTextObject(((PdfFormObject)pdfPageObject).PageObjects, onlyRemoveHiddenObject);
					if (((PdfFormObject)pdfPageObject).PageObjects.Count == 0)
					{
						pageObjects.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06000BD2 RID: 3026
		[DllImport("kernel32.dll")]
		private unsafe static extern bool GlobalMemoryStatusEx([Out] PageObjectProcessor.MEMORYSTATUSEX* lpBuffer);

		// Token: 0x04000568 RID: 1384
		private const bool IsTextVisible = false;

		// Token: 0x04000569 RID: 1385
		private const bool IsTextBoundingBoxVisible = false;

		// Token: 0x0400056A RID: 1386
		private const bool UseKerningScaleText = false;

		// Token: 0x0400056B RID: 1387
		private static global::System.Collections.Generic.IReadOnlyList<FS_COLOR> RandomColors;

		// Token: 0x0400056C RID: 1388
		private static readonly Random rnd = new Random();

		// Token: 0x020004E9 RID: 1257
		private class TaskQueue<T>
		{
			// Token: 0x06002F17 RID: 12055 RVA: 0x000E76E0 File Offset: 0x000E58E0
			public TaskQueue()
			{
				this.globalId = 1;
				this.parallelTaskCount = PageObjectProcessor.TaskQueue<T>.GetParallelTaskCount();
			}

			// Token: 0x06002F18 RID: 12056 RVA: 0x000E7710 File Offset: 0x000E5910
			public void Enqueue(Func<Task<T>> func)
			{
				Queue<Func<Task<T>>> queue = this.queue;
				lock (queue)
				{
					this.queue.Enqueue(func);
					this.ProcessTask();
				}
			}

			// Token: 0x06002F19 RID: 12057 RVA: 0x000E775C File Offset: 0x000E595C
			public async Task<T> Next()
			{
				Task<T>[] array = null;
				Queue<Func<Task<T>>> queue = this.queue;
				lock (queue)
				{
					if (this.runningTasks.Count > 0)
					{
						array = this.runningTasks.Values.ToArray<Task<T>>();
					}
				}
				T t;
				if (array != null)
				{
					t = (await Task.WhenAny<T>(array)).Result;
				}
				else
				{
					t = default(T);
				}
				return t;
			}

			// Token: 0x17000CCA RID: 3274
			// (get) Token: 0x06002F1A RID: 12058 RVA: 0x000E77A0 File Offset: 0x000E59A0
			public bool HasNext
			{
				get
				{
					Queue<Func<Task<T>>> queue = this.queue;
					bool flag2;
					lock (queue)
					{
						flag2 = this.runningTasks.Count > 0;
					}
					return flag2;
				}
			}

			// Token: 0x06002F1B RID: 12059 RVA: 0x000E77EC File Offset: 0x000E59EC
			private void ProcessTask()
			{
				Queue<Func<Task<T>>> queue = this.queue;
				lock (queue)
				{
					while (this.queue.Count > 0 && this.runningTasks.Count < this.parallelTaskCount)
					{
						int num = Interlocked.Increment(ref this.globalId);
						this.runningTasks.Add(num, this.<ProcessTask>g__WrapTask|9_0(num, this.queue.Dequeue()));
					}
				}
			}

			// Token: 0x06002F1C RID: 12060 RVA: 0x000E7874 File Offset: 0x000E5A74
			private static int GetParallelTaskCount()
			{
				int availVirtualMemoryInMB = PageObjectProcessor.TaskQueue<T>.GetAvailVirtualMemoryInMB();
				int processorCount = Environment.ProcessorCount;
				if (availVirtualMemoryInMB == 0)
				{
					return Math.Min(processorCount / 2, 8);
				}
				int num = Math.Max(availVirtualMemoryInMB / 1024 - 1, 1);
				if (num > processorCount / 2)
				{
					num = processorCount / 2;
				}
				if (num > 8)
				{
					num = 8;
				}
				else if (num <= 0)
				{
					num = 1;
				}
				return num;
			}

			// Token: 0x06002F1D RID: 12061 RVA: 0x000E78C4 File Offset: 0x000E5AC4
			private unsafe static int GetAvailVirtualMemoryInMB()
			{
				PageObjectProcessor.MEMORYSTATUSEX memorystatusex = new PageObjectProcessor.MEMORYSTATUSEX
				{
					dwLength = (uint)Marshal.SizeOf<PageObjectProcessor.MEMORYSTATUSEX>()
				};
				if (PageObjectProcessor.GlobalMemoryStatusEx(&memorystatusex))
				{
					return (int)(memorystatusex.ullAvailPhys / 1024UL / 1024UL);
				}
				return 0;
			}

			// Token: 0x06002F1E RID: 12062 RVA: 0x000E7908 File Offset: 0x000E5B08
			[CompilerGenerated]
			private async Task<T> <ProcessTask>g__WrapTask|9_0(int id, Func<Task<T>> func)
			{
				T t = await func();
				Queue<Func<Task<T>>> queue = this.queue;
				lock (queue)
				{
					this.runningTasks.Remove(id);
					this.ProcessTask();
				}
				return t;
			}

			// Token: 0x04001B64 RID: 7012
			private int globalId;

			// Token: 0x04001B65 RID: 7013
			private int parallelTaskCount;

			// Token: 0x04001B66 RID: 7014
			private Queue<Func<Task<T>>> queue = new Queue<Func<Task<T>>>();

			// Token: 0x04001B67 RID: 7015
			private Dictionary<int, Task<T>> runningTasks = new Dictionary<int, Task<T>>();
		}

		// Token: 0x020004EA RID: 1258
		private struct MEMORYSTATUSEX
		{
			// Token: 0x04001B68 RID: 7016
			public uint dwLength;

			// Token: 0x04001B69 RID: 7017
			public uint dwMemoryLoad;

			// Token: 0x04001B6A RID: 7018
			public ulong ullTotalPhys;

			// Token: 0x04001B6B RID: 7019
			public ulong ullAvailPhys;

			// Token: 0x04001B6C RID: 7020
			public ulong ullTotalPageFile;

			// Token: 0x04001B6D RID: 7021
			public ulong ullAvailPageFile;

			// Token: 0x04001B6E RID: 7022
			public ulong ullTotalVirtual;

			// Token: 0x04001B6F RID: 7023
			public ulong ullAvailVirtual;

			// Token: 0x04001B70 RID: 7024
			public ulong ullAvailExtendedVirtual;
		}
	}
}
