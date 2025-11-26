using System;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A6 RID: 422
	public class PdfImageObjectModel : IEquatable<PdfImageObjectModel>, IDisposable
	{
		// Token: 0x0600180F RID: 6159 RVA: 0x0005BF4C File Offset: 0x0005A14C
		public PdfImageObjectModel(PdfImageObject imageObject)
		{
			if (imageObject == null)
			{
				throw new ArgumentNullException("imageObject");
			}
			this.Bitmap = imageObject.Bitmap.Clone(null);
			PdfTypeStream pdfTypeStream = null;
			PdfTypeBase pdfTypeBase;
			if (imageObject.SoftMask != null && imageObject.SoftMask.Is<PdfTypeStream>())
			{
				pdfTypeStream = imageObject.SoftMask.As<PdfTypeStream>(true);
			}
			else if (imageObject.Stream.Dictionary != null && imageObject.Stream.Dictionary.TryGetValue("SMask", out pdfTypeBase) && pdfTypeBase != null && pdfTypeBase.Is<PdfTypeStream>())
			{
				pdfTypeStream = pdfTypeBase.As<PdfTypeStream>(true);
			}
			if (pdfTypeStream != null)
			{
				this.SoftMask = pdfTypeStream.Clone(false);
			}
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0005BFEE File Offset: 0x0005A1EE
		public PdfImageObjectModel(PdfBitmap bitmap)
		{
			this.Bitmap = bitmap.Clone(null);
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x06001811 RID: 6161 RVA: 0x0005C003 File Offset: 0x0005A203
		public PdfBitmap Bitmap { get; }

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x06001812 RID: 6162 RVA: 0x0005C00B File Offset: 0x0005A20B
		public PdfTypeBase SoftMask { get; }

		// Token: 0x06001813 RID: 6163 RVA: 0x0005C013 File Offset: 0x0005A213
		public bool Equals(PdfImageObjectModel other)
		{
			return other.Bitmap == this.Bitmap;
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x0005C023 File Offset: 0x0005A223
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x0005C032 File Offset: 0x0005A232
		protected void Dispose(bool Disposing)
		{
			if (!this.IsDisposed)
			{
				this.Bitmap.Dispose();
				PdfTypeBase softMask = this.SoftMask;
				if (softMask != null)
				{
					softMask.Dispose();
				}
			}
			this.IsDisposed = true;
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x0005C064 File Offset: 0x0005A264
		~PdfImageObjectModel()
		{
			this.Dispose(false);
		}

		// Token: 0x04000809 RID: 2057
		private bool IsDisposed;
	}
}
