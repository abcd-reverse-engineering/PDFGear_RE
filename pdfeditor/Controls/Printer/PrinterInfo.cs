using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using pdfeditor.Utils.Printer;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x0200022B RID: 555
	public class PrinterInfo
	{
		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x06001F1B RID: 7963 RVA: 0x0008C3C6 File Offset: 0x0008A5C6
		// (set) Token: 0x06001F1C RID: 7964 RVA: 0x0008C3CE File Offset: 0x0008A5CE
		public FontFamily FontFamily { get; set; }

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x06001F1D RID: 7965 RVA: 0x0008C3D7 File Offset: 0x0008A5D7
		// (set) Token: 0x06001F1E RID: 7966 RVA: 0x0008C3DF File Offset: 0x0008A5DF
		public FontStyle FontStyle { get; set; }

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06001F1F RID: 7967 RVA: 0x0008C3E8 File Offset: 0x0008A5E8
		// (set) Token: 0x06001F20 RID: 7968 RVA: 0x0008C3F0 File Offset: 0x0008A5F0
		public FontWeight FontWeight { get; set; }

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06001F21 RID: 7969 RVA: 0x0008C3F9 File Offset: 0x0008A5F9
		// (set) Token: 0x06001F22 RID: 7970 RVA: 0x0008C401 File Offset: 0x0008A601
		public FontStretch FontStretch { get; set; }

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x06001F23 RID: 7971 RVA: 0x0008C40A File Offset: 0x0008A60A
		// (set) Token: 0x06001F24 RID: 7972 RVA: 0x0008C412 File Offset: 0x0008A612
		public FlowDirection FlowDirection { get; set; }

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06001F25 RID: 7973 RVA: 0x0008C41B File Offset: 0x0008A61B
		// (set) Token: 0x06001F26 RID: 7974 RVA: 0x0008C423 File Offset: 0x0008A623
		public Brush Foreground { get; set; }

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06001F27 RID: 7975 RVA: 0x0008C42C File Offset: 0x0008A62C
		// (set) Token: 0x06001F28 RID: 7976 RVA: 0x0008C434 File Offset: 0x0008A634
		public double FontSize { get; set; }

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06001F29 RID: 7977 RVA: 0x0008C43D File Offset: 0x0008A63D
		// (set) Token: 0x06001F2A RID: 7978 RVA: 0x0008C445 File Offset: 0x0008A645
		public string PrinterName { get; set; }

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06001F2B RID: 7979 RVA: 0x0008C44E File Offset: 0x0008A64E
		// (set) Token: 0x06001F2C RID: 7980 RVA: 0x0008C456 File Offset: 0x0008A656
		public PrintDevModeHandle PrintDevMode { get; set; }

		// Token: 0x06001F2D RID: 7981 RVA: 0x0008C460 File Offset: 0x0008A660
		public override string ToString()
		{
			if (this.PrinterName.Length > 40)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.PrinterName.Substring(0, 18));
				stringBuilder.Append("...");
				stringBuilder.Append(this.PrinterName.Substring(this.PrinterName.Length - 18, 18));
				return stringBuilder.ToString();
			}
			return this.PrinterName;
		}
	}
}
