using System;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x02000196 RID: 406
	public class PdfFileSpecificationModel : IEquatable<PdfFileSpecificationModel>, IDisposable
	{
		// Token: 0x06001754 RID: 5972 RVA: 0x0005A030 File Offset: 0x00058230
		public PdfFileSpecificationModel(PdfFileSpecification fileSpecification)
		{
			if (fileSpecification == null)
			{
				throw new ArgumentNullException("PdfFileSpecification");
			}
			this.FileName = fileSpecification.FileName;
			this.IsUrl = AttachmentFileUtils.IsUrl(fileSpecification);
			this.isVolatile = fileSpecification.IsVolatile;
			this.Description = fileSpecification.Description;
			this.EmbeddedFile = new PdfFileModel(fileSpecification.EmbeddedFile);
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x0005A092 File Offset: 0x00058292
		public string FileName { get; }

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x0005A09A File Offset: 0x0005829A
		public bool IsUrl { get; }

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x0005A0A2 File Offset: 0x000582A2
		public bool isVolatile { get; }

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x0005A0AA File Offset: 0x000582AA
		public string Description { get; }

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06001759 RID: 5977 RVA: 0x0005A0B2 File Offset: 0x000582B2
		public PdfFileModel EmbeddedFile { get; }

		// Token: 0x0600175A RID: 5978 RVA: 0x0005A0BA File Offset: 0x000582BA
		public bool Equals(PdfFileSpecificationModel other)
		{
			return other.EmbeddedFile == this.EmbeddedFile;
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0005A0CA File Offset: 0x000582CA
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0005A0D9 File Offset: 0x000582D9
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				PdfFileModel embeddedFile = this.EmbeddedFile;
				if (embeddedFile != null)
				{
					embeddedFile.Dispose();
				}
			}
			this.disposedValue = true;
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0005A100 File Offset: 0x00058300
		~PdfFileSpecificationModel()
		{
			this.Dispose(false);
		}

		// Token: 0x040007C3 RID: 1987
		private bool disposedValue;
	}
}
