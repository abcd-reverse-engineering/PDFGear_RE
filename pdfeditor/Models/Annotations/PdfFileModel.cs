using System;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x02000197 RID: 407
	public class PdfFileModel : IEquatable<PdfFileModel>, IDisposable
	{
		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x0005A130 File Offset: 0x00058330
		public PdfTypeBase Stream { get; }

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x0005A138 File Offset: 0x00058338
		public string CreationDate { get; }

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x0005A140 File Offset: 0x00058340
		public string ModificationDate { get; }

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06001761 RID: 5985 RVA: 0x0005A148 File Offset: 0x00058348
		public string CheckSum { get; }

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x0005A150 File Offset: 0x00058350
		public int FileSize { get; }

		// Token: 0x06001763 RID: 5987 RVA: 0x0005A158 File Offset: 0x00058358
		public PdfFileModel(PdfFile pdfFile)
		{
			if (pdfFile == null)
			{
				throw new ArgumentNullException("pdfFile");
			}
			PdfTypeStream pdfTypeStream = null;
			if (pdfFile.Stream != null && pdfFile.Stream.Is<PdfTypeStream>())
			{
				pdfTypeStream = pdfFile.Stream.As<PdfTypeStream>(true);
			}
			if (pdfTypeStream != null)
			{
				this.Stream = pdfTypeStream.Clone(false);
			}
			this.CreationDate = pdfFile.CreationDate;
			this.ModificationDate = pdfFile.ModificationDate;
			this.CheckSum = pdfFile.CheckSum;
			this.FileSize = pdfFile.FileSize;
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0005A1E0 File Offset: 0x000583E0
		public bool Equals(PdfFileModel other)
		{
			return other.CreationDate == this.CreationDate && other.ModificationDate == this.ModificationDate && other.CheckSum == this.CheckSum && other.FileSize == this.FileSize;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0005A236 File Offset: 0x00058436
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0005A245 File Offset: 0x00058445
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				PdfTypeBase stream = this.Stream;
				if (stream != null)
				{
					stream.Dispose();
				}
			}
			this.disposedValue = true;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0005A26C File Offset: 0x0005846C
		~PdfFileModel()
		{
			this.Dispose(false);
		}

		// Token: 0x040007C9 RID: 1993
		private bool disposedValue;
	}
}
