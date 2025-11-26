using System;
using System.Windows.Media;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x02000082 RID: 130
	public class LinkAnnotationModel
	{
		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0002CB4A File Offset: 0x0002AD4A
		// (set) Token: 0x060008F0 RID: 2288 RVA: 0x0002CB52 File Offset: 0x0002AD52
		public LinkSelect Action { get; set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060008F1 RID: 2289 RVA: 0x0002CB5B File Offset: 0x0002AD5B
		// (set) Token: 0x060008F2 RID: 2290 RVA: 0x0002CB63 File Offset: 0x0002AD63
		public BorderStyles BorderStyle { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0002CB6C File Offset: 0x0002AD6C
		// (set) Token: 0x060008F4 RID: 2292 RVA: 0x0002CB74 File Offset: 0x0002AD74
		public float Width { get; set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0002CB7D File Offset: 0x0002AD7D
		// (set) Token: 0x060008F6 RID: 2294 RVA: 0x0002CB85 File Offset: 0x0002AD85
		public Color BorderColor { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x0002CB8E File Offset: 0x0002AD8E
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x0002CB96 File Offset: 0x0002AD96
		public string Uri { get; set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0002CB9F File Offset: 0x0002AD9F
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x0002CBA7 File Offset: 0x0002ADA7
		public int Page { get; set; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x0002CBB0 File Offset: 0x0002ADB0
		// (set) Token: 0x060008FC RID: 2300 RVA: 0x0002CBB8 File Offset: 0x0002ADB8
		public PdfDocument PdfDocument { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x0002CBC1 File Offset: 0x0002ADC1
		// (set) Token: 0x060008FE RID: 2302 RVA: 0x0002CBC9 File Offset: 0x0002ADC9
		public string FileName { get; set; }

		// Token: 0x04000455 RID: 1109
		public string Title = "";
	}
}
