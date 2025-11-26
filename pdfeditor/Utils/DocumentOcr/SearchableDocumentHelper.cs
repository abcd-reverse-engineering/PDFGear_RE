using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using PDFKit.Utils;

namespace pdfeditor.Utils.DocumentOcr
{
	// Token: 0x020000D7 RID: 215
	internal class SearchableDocumentHelper
	{
		// Token: 0x06000BD5 RID: 3029 RVA: 0x0003E8DD File Offset: 0x0003CADD
		public SearchableDocumentHelper(string filePath, string password)
		{
			this.filePath = filePath;
			this.password = password;
			this.cts = new CancellationTokenSource();
			this.GetDocumentContentTypeAsync();
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x0003E905 File Offset: 0x0003CB05
		public bool Completed
		{
			get
			{
				return this.task != null && this.task.IsCompleted;
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0003E91C File Offset: 0x0003CB1C
		public Task<PdfContentType> GetDocumentContentTypeAsync()
		{
			Task<PdfContentType> task = this.task;
			if (this.cts.Token.IsCancellationRequested)
			{
				return Task.FromCanceled<PdfContentType>(this.cts.Token);
			}
			if (task == null)
			{
				this.cts = new CancellationTokenSource();
				this.task = Task.Run<PdfContentType>(async delegate
				{
					try
					{
						using (FileStream fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
						{
							using (PdfDocument doc = PdfDocument.Load(fs, null, this.password, true))
							{
								PdfContentType pdfContentType = await SearchableDocumentHelper.GetDocumentType(doc, this.cts.Token);
								this.result = pdfContentType;
							}
							PdfDocument doc = null;
						}
						FileStream fs = null;
					}
					catch (Exception ex) when (!(ex is OperationCanceledException))
					{
						this.result = PdfContentType.Text;
					}
					return this.result;
				}, this.cts.Token);
				return this.task;
			}
			if (task.Status == TaskStatus.RanToCompletion)
			{
				return Task.FromResult<PdfContentType>(this.result);
			}
			return task;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0003E9A5 File Offset: 0x0003CBA5
		public void Cancel()
		{
			this.cts.Cancel();
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0003E9B4 File Offset: 0x0003CBB4
		public static async Task<PdfContentType> GetDocumentType(PdfDocument document, CancellationToken cancellationToken = default(CancellationToken))
		{
			int pageCount = document.Pages.Count;
			int[] dict = new int[3];
			for (int i = 0; i < document.Pages.Count; i++)
			{
				IntPtr pageHandle = IntPtr.Zero;
				PdfPage page = null;
				try
				{
					if (i != 0)
					{
						await Task.Delay(100, cancellationToken);
					}
					cancellationToken.ThrowIfCancellationRequested();
					pageHandle = Pdfium.FPDF_LoadPage(document.Handle, i);
					if (pageHandle != IntPtr.Zero)
					{
						cancellationToken.ThrowIfCancellationRequested();
						page = PdfPage.FromHandle(document, pageHandle, i, true);
						PdfContentType pageType = SearchableDocumentHelper.GetPageType(page, cancellationToken);
						dict[(int)pageType]++;
						if (dict[(int)pageType] > 10)
						{
							if (pageType != PdfContentType.Text)
							{
								return pageType;
							}
							if (dict[(int)pageType] > 20)
							{
								return pageType;
							}
						}
						cancellationToken.ThrowIfCancellationRequested();
					}
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				finally
				{
					PdfPage pdfPage = page;
					if (pdfPage != null)
					{
						pdfPage.Dispose();
					}
					if (pageHandle != IntPtr.Zero)
					{
						Pdfium.FPDF_ClosePage(pageHandle);
					}
				}
				page = null;
			}
			if (dict[1] + dict[2] < (pageCount + 1) / 2)
			{
				return PdfContentType.Text;
			}
			if (dict[2] > dict[1])
			{
				return PdfContentType.ImageOrPathWithTextLayer;
			}
			return PdfContentType.ImageOrPath;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0003EA00 File Offset: 0x0003CC00
		public static PdfContentType GetPageType(PdfPage page, CancellationToken cancellationToken = default(CancellationToken))
		{
			FS_SIZEF effectiveSize = page.GetEffectiveSize(PageRotate.Normal, false);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			bool flag2 = false;
			foreach (PdfPageObject pdfPageObject in SearchableDocumentHelper.WalkPageObjects(page.PageObjects))
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (pdfPageObject.ObjectType == PageObjectTypes.PDFPAGE_IMAGE && SearchableDocumentHelper.IsMainImageForPage(effectiveSize, (PdfImageObject)pdfPageObject))
				{
					flag = true;
				}
				else if (pdfPageObject.ObjectType == PageObjectTypes.PDFPAGE_TEXT)
				{
					if (SearchableDocumentHelper.IsVisibleTextObject((PdfTextObject)pdfPageObject))
					{
						num2++;
						if (num2 > 200)
						{
							return PdfContentType.Text;
						}
					}
					else
					{
						num3++;
						if (num3 > 200)
						{
							return PdfContentType.ImageOrPathWithTextLayer;
						}
					}
				}
				else if (pdfPageObject.ObjectType == PageObjectTypes.PDFPAGE_PATH)
				{
					num++;
					if (num > 200)
					{
						flag2 = true;
					}
				}
			}
			if (num2 > 50 || (!flag && !flag2))
			{
				return PdfContentType.Text;
			}
			if (num3 > 50)
			{
				return PdfContentType.ImageOrPathWithTextLayer;
			}
			return PdfContentType.ImageOrPath;
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0003EB00 File Offset: 0x0003CD00
		private static bool IsMainImageForPage(FS_SIZEF pageSize, PdfImageObject imageObject)
		{
			FS_RECTF boundingBox = SearchableDocumentHelper.GetBoundingBox(imageObject);
			if (boundingBox.left < 0f)
			{
				boundingBox.left = 0f;
			}
			if (boundingBox.bottom < 0f)
			{
				boundingBox.bottom = 0f;
			}
			if (boundingBox.right > pageSize.Width)
			{
				boundingBox.right = pageSize.Width;
			}
			if (boundingBox.top > pageSize.Height)
			{
				boundingBox.top = pageSize.Height;
			}
			return boundingBox.Width > pageSize.Width / 3f * 2f && boundingBox.Height > pageSize.Height / 3f * 2f;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0003EBB4 File Offset: 0x0003CDB4
		private static IEnumerable<PdfPageObject> WalkPageObjects(PdfPageObjectsCollection objects)
		{
			SearchableDocumentHelper.<WalkPageObjects>d__13 <WalkPageObjects>d__ = new SearchableDocumentHelper.<WalkPageObjects>d__13(-2);
			<WalkPageObjects>d__.<>3__objects = objects;
			return <WalkPageObjects>d__;
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0003EBC4 File Offset: 0x0003CDC4
		private static FS_RECTF GetBoundingBox(PdfPageObject pageObject)
		{
			FS_MATRIX fs_MATRIX = null;
			PdfFormObject pdfFormObject;
			if (pageObject == null)
			{
				pdfFormObject = null;
			}
			else
			{
				PdfPageObjectsCollection container = pageObject.Container;
				pdfFormObject = ((container != null) ? container.Form : null);
			}
			PdfPageObjectsCollection container2;
			for (PdfFormObject pdfFormObject2 = pdfFormObject; pdfFormObject2 != null; pdfFormObject2 = ((container2 != null) ? container2.Form : null))
			{
				FS_MATRIX fs_MATRIX2 = Pdfium.FPDFFormObj_GetFormMatrix(pdfFormObject2.Handle);
				if (fs_MATRIX == null)
				{
					fs_MATRIX = fs_MATRIX2;
				}
				else
				{
					fs_MATRIX.Concat(fs_MATRIX2, false);
				}
				container2 = pdfFormObject2.Container;
			}
			float num;
			float num2;
			float num3;
			float num4;
			Pdfium.FPDFPageObj_GetBBox(pageObject.Handle, fs_MATRIX, out num, out num2, out num3, out num4);
			return new FS_RECTF(num, num2, num3, num4);
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0003EC44 File Offset: 0x0003CE44
		internal static bool IsVisibleTextObject(PdfTextObject textObject)
		{
			if (textObject == null)
			{
				return false;
			}
			FS_RECTF boundingBox = textObject.BoundingBox;
			if ((double)boundingBox.Width < 0.001 && (double)boundingBox.Height < 0.001)
			{
				return false;
			}
			if (textObject.RenderMode == TextRenderingModes.Nothing)
			{
				return false;
			}
			if (textObject.RenderMode == TextRenderingModes.Fill)
			{
				return textObject.FillColor.A != 0;
			}
			if (textObject.RenderMode == TextRenderingModes.Stroke)
			{
				return textObject.StrokeColor.A != 0;
			}
			if (textObject.RenderMode == TextRenderingModes.FillThenStroke)
			{
				return textObject.FillColor.A != 0 || textObject.StrokeColor.A != 0;
			}
			switch (textObject.RenderMode)
			{
			case TextRenderingModes.Fill:
				return textObject.FillColor.A != 0;
			case TextRenderingModes.Stroke:
				return textObject.StrokeColor.A != 0;
			case TextRenderingModes.FillThenStroke:
				return textObject.FillColor.A != 0 || textObject.StrokeColor.A != 0;
			case TextRenderingModes.Nothing:
				return false;
			case TextRenderingModes.FillClip:
			case TextRenderingModes.StrokeClip:
			case TextRenderingModes.FillThenStrokeClip:
			case TextRenderingModes.Clipping:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x0400056D RID: 1389
		private readonly string filePath;

		// Token: 0x0400056E RID: 1390
		private readonly string password;

		// Token: 0x0400056F RID: 1391
		private Task<PdfContentType> task;

		// Token: 0x04000570 RID: 1392
		private CancellationTokenSource cts;

		// Token: 0x04000571 RID: 1393
		private PdfContentType result;
	}
}
