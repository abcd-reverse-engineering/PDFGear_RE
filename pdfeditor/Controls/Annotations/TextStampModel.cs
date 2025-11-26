using System;
using System.Windows;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002AB RID: 683
	public class TextStampModel
	{
		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x06002762 RID: 10082 RVA: 0x000BA432 File Offset: 0x000B8632
		// (set) Token: 0x06002763 RID: 10083 RVA: 0x000BA43A File Offset: 0x000B863A
		public double TextFontSize { get; set; }

		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x06002764 RID: 10084 RVA: 0x000BA443 File Offset: 0x000B8643
		// (set) Token: 0x06002765 RID: 10085 RVA: 0x000BA44B File Offset: 0x000B864B
		public string TextFontFamily { get; set; }

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x06002766 RID: 10086 RVA: 0x000BA454 File Offset: 0x000B8654
		// (set) Token: 0x06002767 RID: 10087 RVA: 0x000BA45C File Offset: 0x000B865C
		public FontWeight TextFontWeight { get; set; }

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x06002768 RID: 10088 RVA: 0x000BA465 File Offset: 0x000B8665
		// (set) Token: 0x06002769 RID: 10089 RVA: 0x000BA46D File Offset: 0x000B866D
		public FontStyle TextFontStyle { get; set; }

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x0600276A RID: 10090 RVA: 0x000BA476 File Offset: 0x000B8676
		// (set) Token: 0x0600276B RID: 10091 RVA: 0x000BA47E File Offset: 0x000B867E
		public string Text { get; set; }

		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x0600276C RID: 10092 RVA: 0x000BA487 File Offset: 0x000B8687
		// (set) Token: 0x0600276D RID: 10093 RVA: 0x000BA48F File Offset: 0x000B868F
		public double TextWidth { get; set; }

		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x0600276E RID: 10094 RVA: 0x000BA498 File Offset: 0x000B8698
		// (set) Token: 0x0600276F RID: 10095 RVA: 0x000BA4A0 File Offset: 0x000B86A0
		public double TextHeight { get; set; }

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x06002770 RID: 10096 RVA: 0x000BA4A9 File Offset: 0x000B86A9
		// (set) Token: 0x06002771 RID: 10097 RVA: 0x000BA4B1 File Offset: 0x000B86B1
		public string BorderBrush { get; set; }

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x06002772 RID: 10098 RVA: 0x000BA4BA File Offset: 0x000B86BA
		// (set) Token: 0x06002773 RID: 10099 RVA: 0x000BA4C2 File Offset: 0x000B86C2
		public string Foreground { get; set; }

		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x06002774 RID: 10100 RVA: 0x000BA4CB File Offset: 0x000B86CB
		// (set) Token: 0x06002775 RID: 10101 RVA: 0x000BA4D3 File Offset: 0x000B86D3
		public double PageScale { get; set; }

		// Token: 0x17000C18 RID: 3096
		// (get) Token: 0x06002776 RID: 10102 RVA: 0x000BA4DC File Offset: 0x000B86DC
		// (set) Token: 0x06002777 RID: 10103 RVA: 0x000BA4E4 File Offset: 0x000B86E4
		public string TimeFormat { get; set; }
	}
}
