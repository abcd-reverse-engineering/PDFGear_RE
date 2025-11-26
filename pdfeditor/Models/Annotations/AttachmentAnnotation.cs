using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x02000195 RID: 405
	public class AttachmentAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x0600174C RID: 5964 RVA: 0x00059E81 File Offset: 0x00058081
		// (set) Token: 0x0600174D RID: 5965 RVA: 0x00059E89 File Offset: 0x00058089
		public string ExtendedIconName { get; protected set; }

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x00059E92 File Offset: 0x00058092
		// (set) Token: 0x0600174F RID: 5967 RVA: 0x00059E9A File Offset: 0x0005809A
		public PdfFileSpecificationModel FileSpecification { get; protected set; }

		// Token: 0x06001750 RID: 5968 RVA: 0x00059EA4 File Offset: 0x000580A4
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfFileAttachmentAnnotation annot = pdfAnnotation as PdfFileAttachmentAnnotation;
			if (annot != null)
			{
				this.ExtendedIconName = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.ExtendedIconName);
				this.FileSpecification = new PdfFileSpecificationModel(annot.FileSpecification);
			}
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x00059F00 File Offset: 0x00058100
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = annot as PdfFileAttachmentAnnotation;
			if (pdfFileAttachmentAnnotation != null)
			{
				pdfFileAttachmentAnnotation.Rectangle = base.Rectangle;
				pdfFileAttachmentAnnotation.ExtendedIconName = this.ExtendedIconName;
				if (this.FileSpecification != null)
				{
					MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
					PdfDocument pdfDocument = ((requiredService != null) ? requiredService.Document : null);
					if (pdfDocument != null)
					{
						PdfTypeBase stream = this.FileSpecification.EmbeddedFile.Stream;
						if (stream != null)
						{
							PdfTypeStream pdfTypeStream = stream.Clone(false) as PdfTypeStream;
							if (pdfTypeStream != null)
							{
								pdfFileAttachmentAnnotation.FileSpecification = new PdfFileSpecification(pdfDocument)
								{
									FileName = this.FileSpecification.FileName,
									IsUrl = this.FileSpecification.IsUrl,
									IsVolatile = this.FileSpecification.isVolatile,
									Description = this.FileSpecification.Description,
									EmbeddedFile = new PdfFile(pdfTypeStream)
								};
							}
						}
					}
				}
			}
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x00059FE4 File Offset: 0x000581E4
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				AttachmentAnnotation attachmentAnnotation = other as AttachmentAnnotation;
				if (attachmentAnnotation != null && attachmentAnnotation.ExtendedIconName == this.ExtendedIconName && attachmentAnnotation.FileSpecification == this.FileSpecification)
				{
					return true;
				}
			}
			return false;
		}
	}
}
