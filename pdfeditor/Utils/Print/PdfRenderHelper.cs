using System;
using System.Buffers;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000C2 RID: 194
	internal static class PdfRenderHelper
	{
		// Token: 0x06000B9A RID: 2970 RVA: 0x0003DAB1 File Offset: 0x0003BCB1
		internal static IDisposable CreateHideFlagContext(PdfPage page, bool isAnnotationVisible)
		{
			return new PdfRenderHelper.AnnotationFlagContext(page, isAnnotationVisible);
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0003DABF File Offset: 0x0003BCBF
		internal static bool CanAnnotationHide(PdfAnnotation annot)
		{
			return !(annot == null) && !(annot is PdfWidgetAnnotation);
		}

		// Token: 0x020004E1 RID: 1249
		internal struct AnnotationFlagContext : IDisposable
		{
			// Token: 0x17000CC9 RID: 3273
			// (get) Token: 0x06002F0E RID: 12046 RVA: 0x000E71A8 File Offset: 0x000E53A8
			// (set) Token: 0x06002F0F RID: 12047 RVA: 0x000E71B0 File Offset: 0x000E53B0
			public PdfPage Page { get; set; }

			// Token: 0x06002F10 RID: 12048 RVA: 0x000E71BC File Offset: 0x000E53BC
			public AnnotationFlagContext(PdfPage page, bool isAnnotationVisible)
			{
				this.Page = page;
				this.isAnnotationVisible = isAnnotationVisible;
				if (page.Annots == null)
				{
					this.isAnnotationVisible = true;
				}
				this.visibleAnnotDict = null;
				this.dictLength = 0;
				if (this.isAnnotationVisible)
				{
					return;
				}
				this.dictLength = page.Annots.Count;
				this.visibleAnnotDict = ArrayPool<bool>.Shared.Rent(this.dictLength);
				for (int i = 0; i < this.dictLength; i++)
				{
					if (PdfRenderHelper.CanAnnotationHide(page.Annots[i]))
					{
						this.visibleAnnotDict[i] = (page.Annots[i].Flags & AnnotationFlags.Hidden) == AnnotationFlags.None;
						page.Annots[i].Flags |= AnnotationFlags.Hidden;
					}
				}
			}

			// Token: 0x06002F11 RID: 12049 RVA: 0x000E7280 File Offset: 0x000E5480
			public void Dispose()
			{
				if (!this.isAnnotationVisible)
				{
					PdfPage page = this.Page;
					if (((page != null) ? page.Annots : null) != null)
					{
						for (int i = 0; i < this.dictLength; i++)
						{
							if (this.visibleAnnotDict[i] && PdfRenderHelper.CanAnnotationHide(this.Page.Annots[i]))
							{
								this.Page.Annots[i].Flags &= ~AnnotationFlags.Hidden;
							}
						}
						this.isAnnotationVisible = true;
						ArrayPool<bool>.Shared.Return(this.visibleAnnotDict, false);
						this.visibleAnnotDict = null;
						return;
					}
				}
			}

			// Token: 0x04001B29 RID: 6953
			private bool isAnnotationVisible;

			// Token: 0x04001B2A RID: 6954
			private bool[] visibleAnnotDict;

			// Token: 0x04001B2B RID: 6955
			private readonly int dictLength;
		}
	}
}
