using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002BE RID: 702
	public abstract class TextMarkupAnnotationHolder<TMarkupAnnotation> : BaseAnnotationHolder<TMarkupAnnotation> where TMarkupAnnotation : PdfTextMarkupAnnotation
	{
		// Token: 0x0600286A RID: 10346 RVA: 0x000BE158 File Offset: 0x000BC358
		public TextMarkupAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x0600286B RID: 10347 RVA: 0x000BE183 File Offset: 0x000BC383
		public override bool IsTextMarkupAnnotation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600286C RID: 10348 RVA: 0x000BE186 File Offset: 0x000BC386
		public override void OnPageClientBoundsChanged()
		{
			AnnotationFocusControl annotationFocusControl = this.selectControl;
			if (annotationFocusControl == null)
			{
				return;
			}
			annotationFocusControl.InvalidateVisual();
		}

		// Token: 0x0600286D RID: 10349 RVA: 0x000BE198 File Offset: 0x000BC398
		protected override void OnCancel()
		{
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
			this.m_selectInfo = new SelectInfo
			{
				StartPage = -1
			};
			if (this.selectControl != null)
			{
				base.AnnotationCanvas.Children.Remove(this.selectControl);
				this.selectControl = null;
			}
		}

		// Token: 0x0600286E RID: 10350
		public abstract global::System.Collections.Generic.IReadOnlyList<TMarkupAnnotation> CreateAnnotation(PdfDocument document, SelectInfo selectInfo);

		// Token: 0x0600286F RID: 10351 RVA: 0x000BE1F9 File Offset: 0x000BC3F9
		protected virtual bool CheckPointMoved(FS_POINTF point1, FS_POINTF point2)
		{
			return Math.Abs(point1.X - point2.X) > 10f || Math.Abs(point1.Y - point2.Y) > 10f;
		}

		// Token: 0x06002870 RID: 10352 RVA: 0x000BE234 File Offset: 0x000BC434
		protected override async Task<global::System.Collections.Generic.IReadOnlyList<TMarkupAnnotation>> OnCompleteCreateNewAsync()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			PdfPage currentPage = base.CurrentPage;
			if (currentPage.Document == null)
			{
				throw new ArgumentException("Document");
			}
			global::System.Collections.Generic.IReadOnlyList<TMarkupAnnotation> list = null;
			if (this.m_selectInfo.StartPage != this.m_selectInfo.EndPage || this.CheckPointMoved(this.createStartPoint, this.createEndPoint))
			{
				list = this.CreateAnnotation(currentPage.Document, this.m_selectInfo);
			}
			if (list != null && list.Count > 0)
			{
				string text = DateTimeOffset.Now.ToModificationDateString();
				foreach (TMarkupAnnotation tmarkupAnnotation in list)
				{
					tmarkupAnnotation.ModificationDate = text;
					tmarkupAnnotation.CreationDate = text;
				}
				await requiredService.OperationManager.TraceAnnotationInsertAsync(list, "");
				foreach (PdfPage pdfPage in list.Select((TMarkupAnnotation c) => c.Page).Distinct<PdfPage>())
				{
					await pdfPage.TryRedrawPageAsync(default(CancellationToken));
				}
				IEnumerator<PdfPage> enumerator2 = null;
			}
			this.createStartPoint = default(FS_POINTF);
			this.createEndPoint = default(FS_POINTF);
			return list;
		}

		// Token: 0x06002871 RID: 10353 RVA: 0x000BE278 File Offset: 0x000BC478
		protected override void OnProcessCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			int pageIndex = page.PageIndex;
			int charIndexAtPos = page.Text.GetCharIndexAtPos(pagePoint.X, pagePoint.Y, 10f, 10f);
			if (pageIndex != -1 && charIndexAtPos != -1)
			{
				this.m_selectInfo.EndPage = pageIndex;
				this.m_selectInfo.EndIndex = charIndexAtPos;
			}
			this.createEndPoint = pagePoint;
		}

		// Token: 0x06002872 RID: 10354 RVA: 0x000BE2D9 File Offset: 0x000BC4D9
		protected override bool OnSelecting(TMarkupAnnotation annotation, bool afterCreate)
		{
			this.selectControl = new AnnotationFocusControl(base.AnnotationCanvas)
			{
				Annotation = annotation,
				IsTextMarkupFocusVisible = true
			};
			base.AnnotationCanvas.Children.Add(this.selectControl);
			return true;
		}

		// Token: 0x06002873 RID: 10355 RVA: 0x000BE318 File Offset: 0x000BC518
		protected override bool OnStartCreateNew(PdfPage page, FS_POINTF pagePoint)
		{
			int pageIndex = page.PageIndex;
			int charIndexAtPos = page.Text.GetCharIndexAtPos(pagePoint.X, pagePoint.Y, 10f, 10f);
			this.m_selectInfo = new SelectInfo
			{
				StartPage = pageIndex,
				EndPage = pageIndex,
				StartIndex = charIndexAtPos,
				EndIndex = charIndexAtPos
			};
			this.createStartPoint = pagePoint;
			this.createEndPoint = pagePoint;
			return true;
		}

		// Token: 0x0400115A RID: 4442
		private SelectInfo m_selectInfo = new SelectInfo
		{
			StartPage = -1
		};

		// Token: 0x0400115B RID: 4443
		private FS_POINTF createStartPoint;

		// Token: 0x0400115C RID: 4444
		private FS_POINTF createEndPoint;

		// Token: 0x0400115D RID: 4445
		private AnnotationFocusControl selectControl;
	}
}
