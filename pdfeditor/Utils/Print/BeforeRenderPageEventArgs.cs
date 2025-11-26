using System;
using System.Drawing;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000BC RID: 188
	public class BeforeRenderPageEventArgs : EventArgs
	{
		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x00039B81 File Offset: 0x00037D81
		// (set) Token: 0x06000B2E RID: 2862 RVA: 0x00039B89 File Offset: 0x00037D89
		public Graphics Graphics { get; private set; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x00039B92 File Offset: 0x00037D92
		// (set) Token: 0x06000B30 RID: 2864 RVA: 0x00039B9A File Offset: 0x00037D9A
		public PdfPage Page { get; private set; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000B31 RID: 2865 RVA: 0x00039BA3 File Offset: 0x00037DA3
		// (set) Token: 0x06000B32 RID: 2866 RVA: 0x00039BAB File Offset: 0x00037DAB
		public int X { get; set; }

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x00039BB4 File Offset: 0x00037DB4
		// (set) Token: 0x06000B34 RID: 2868 RVA: 0x00039BBC File Offset: 0x00037DBC
		public int Y { get; set; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000B35 RID: 2869 RVA: 0x00039BC5 File Offset: 0x00037DC5
		// (set) Token: 0x06000B36 RID: 2870 RVA: 0x00039BCD File Offset: 0x00037DCD
		public int Width { get; set; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x00039BD6 File Offset: 0x00037DD6
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x00039BDE File Offset: 0x00037DDE
		public int Height { get; set; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000B39 RID: 2873 RVA: 0x00039BE7 File Offset: 0x00037DE7
		// (set) Token: 0x06000B3A RID: 2874 RVA: 0x00039BEF File Offset: 0x00037DEF
		public PageRotate Rotation { get; private set; }

		// Token: 0x06000B3B RID: 2875 RVA: 0x00039BF8 File Offset: 0x00037DF8
		public BeforeRenderPageEventArgs(Graphics g, PdfPage page, int x, int y, int width, int height, PageRotate rotation)
		{
			this.Graphics = g;
			this.Page = page;
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Rotation = rotation;
		}
	}
}
