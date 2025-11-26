using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Attachments
{
	// Token: 0x02000192 RID: 402
	public class AttachmentWrappersCollection : ObservableCollection<PDFAttachmentWrapper>
	{
		// Token: 0x06001725 RID: 5925 RVA: 0x000586C8 File Offset: 0x000568C8
		public AttachmentWrappersCollection(PdfDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.Document = document;
			this.NotifyAttachmentChanged();
		}

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x000586F7 File Offset: 0x000568F7
		public PdfDocument Document { get; }

		// Token: 0x06001727 RID: 5927 RVA: 0x00058700 File Offset: 0x00056900
		public void NotifyAttachmentChanged()
		{
			if (this.Document != null)
			{
				object obj = this.notifyChangedLocker;
				lock (obj)
				{
					try
					{
						base.Clear();
						if (this.Document.Attachments != null)
						{
							foreach (PdfAttachment pdfAttachment in this.Document.Attachments)
							{
								base.Add(new PDFAttachmentWrapper(pdfAttachment));
							}
						}
						List<PDFAttachmentWrapper> list = new List<PDFAttachmentWrapper>();
						using (IEnumerator<AttachmentFileUtils.PdfFileAttachmentAnnotationResult> enumerator2 = AttachmentFileUtils.GetAllFileAttachmentAnnotations(this.Document).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AttachmentFileUtils.PdfFileAttachmentAnnotationResult result = enumerator2.Current;
								PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.Document.Pages[result.PageIndex].Annots.FirstOrDefault((PdfAnnotation t) => t.Dictionary.Handle == result.AnnotationDictionary.Handle) as PdfFileAttachmentAnnotation;
								if (pdfFileAttachmentAnnotation != null)
								{
									pdfFileAttachmentAnnotation.Flags = AnnotationFlags.Print;
									list.Add(new PDFAttachmentWrapper(pdfFileAttachmentAnnotation));
								}
							}
						}
						foreach (PDFAttachmentWrapper pdfattachmentWrapper in list.OrderBy((PDFAttachmentWrapper x) => x.OrderIndex))
						{
							base.Add(pdfattachmentWrapper);
						}
					}
					finally
					{
						Ioc.Default.GetRequiredService<MainViewModel>().AttachmentAllCount = base.Count;
					}
				}
			}
		}

		// Token: 0x040007BA RID: 1978
		private object notifyChangedLocker = new object();
	}
}
