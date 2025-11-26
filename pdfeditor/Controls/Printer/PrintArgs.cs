using System;
using System.Drawing.Printing;
using Patagames.Pdf.Net;
using pdfeditor.Utils.Print;
using pdfeditor.Utils.Printer;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x0200022D RID: 557
	public class PrintArgs
	{
		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x06001F35 RID: 7989 RVA: 0x0008C570 File Offset: 0x0008A770
		// (set) Token: 0x06001F36 RID: 7990 RVA: 0x0008C578 File Offset: 0x0008A778
		public bool Grayscale { get; set; }

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x06001F37 RID: 7991 RVA: 0x0008C581 File Offset: 0x0008A781
		// (set) Token: 0x06001F38 RID: 7992 RVA: 0x0008C589 File Offset: 0x0008A789
		public bool AutoCenter { get; set; }

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06001F39 RID: 7993 RVA: 0x0008C592 File Offset: 0x0008A792
		// (set) Token: 0x06001F3A RID: 7994 RVA: 0x0008C59A File Offset: 0x0008A79A
		public bool AutoRotate { get; set; }

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06001F3B RID: 7995 RVA: 0x0008C5A3 File Offset: 0x0008A7A3
		// (set) Token: 0x06001F3C RID: 7996 RVA: 0x0008C5AB File Offset: 0x0008A7AB
		public int Copies { get; set; } = 1;

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06001F3D RID: 7997 RVA: 0x0008C5B4 File Offset: 0x0008A7B4
		public int AllCount
		{
			get
			{
				PrintPageIndexMapper printPageIndexMapper = this.PrintPageIndexMapper;
				if (printPageIndexMapper == null)
				{
					return 0;
				}
				return printPageIndexMapper.PrintPageCount;
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06001F3E RID: 7998 RVA: 0x0008C5C7 File Offset: 0x0008A7C7
		// (set) Token: 0x06001F3F RID: 7999 RVA: 0x0008C5CF File Offset: 0x0008A7CF
		public int TotalPages { get; set; } = 1;

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06001F40 RID: 8000 RVA: 0x0008C5D8 File Offset: 0x0008A7D8
		// (set) Token: 0x06001F41 RID: 8001 RVA: 0x0008C5E0 File Offset: 0x0008A7E0
		public PrintSizeMode PrintSizeMode { get; set; }

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06001F42 RID: 8002 RVA: 0x0008C5E9 File Offset: 0x0008A7E9
		// (set) Token: 0x06001F43 RID: 8003 RVA: 0x0008C5F1 File Offset: 0x0008A7F1
		public int Scale { get; set; } = 100;

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x06001F44 RID: 8004 RVA: 0x0008C5FA File Offset: 0x0008A7FA
		// (set) Token: 0x06001F45 RID: 8005 RVA: 0x0008C602 File Offset: 0x0008A802
		public string PrinterName { get; set; }

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x06001F46 RID: 8006 RVA: 0x0008C60B File Offset: 0x0008A80B
		// (set) Token: 0x06001F47 RID: 8007 RVA: 0x0008C613 File Offset: 0x0008A813
		public string DocumentPath { get; set; }

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x06001F48 RID: 8008 RVA: 0x0008C61C File Offset: 0x0008A81C
		// (set) Token: 0x06001F49 RID: 8009 RVA: 0x0008C624 File Offset: 0x0008A824
		public bool Collate { get; set; }

		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x06001F4A RID: 8010 RVA: 0x0008C62D File Offset: 0x0008A82D
		// (set) Token: 0x06001F4B RID: 8011 RVA: 0x0008C635 File Offset: 0x0008A835
		public Duplex Duplex { get; set; }

		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06001F4C RID: 8012 RVA: 0x0008C63E File Offset: 0x0008A83E
		// (set) Token: 0x06001F4D RID: 8013 RVA: 0x0008C646 File Offset: 0x0008A846
		public PaperSize PaperSize { get; set; }

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x06001F4E RID: 8014 RVA: 0x0008C64F File Offset: 0x0008A84F
		// (set) Token: 0x06001F4F RID: 8015 RVA: 0x0008C657 File Offset: 0x0008A857
		public bool Landscape { get; set; }

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x06001F50 RID: 8016 RVA: 0x0008C660 File Offset: 0x0008A860
		// (set) Token: 0x06001F51 RID: 8017 RVA: 0x0008C668 File Offset: 0x0008A868
		public PdfDocument Document { get; set; }

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x06001F52 RID: 8018 RVA: 0x0008C671 File Offset: 0x0008A871
		// (set) Token: 0x06001F53 RID: 8019 RVA: 0x0008C679 File Offset: 0x0008A879
		public PrintDevModeHandle PrintDevMode { get; set; }

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x06001F54 RID: 8020 RVA: 0x0008C682 File Offset: 0x0008A882
		// (set) Token: 0x06001F55 RID: 8021 RVA: 0x0008C68A File Offset: 0x0008A88A
		public PrintPageIndexMapper PrintPageIndexMapper { get; set; }

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x06001F56 RID: 8022 RVA: 0x0008C693 File Offset: 0x0008A893
		// (set) Token: 0x06001F57 RID: 8023 RVA: 0x0008C69B File Offset: 0x0008A89B
		public string DocumentTitle { get; set; }

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x06001F58 RID: 8024 RVA: 0x0008C6A4 File Offset: 0x0008A8A4
		// (set) Token: 0x06001F59 RID: 8025 RVA: 0x0008C6AC File Offset: 0x0008A8AC
		public int PapersPerSheet { get; set; } = 4;

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06001F5A RID: 8026 RVA: 0x0008C6B5 File Offset: 0x0008A8B5
		// (set) Token: 0x06001F5B RID: 8027 RVA: 0x0008C6BD File Offset: 0x0008A8BD
		public int PaperRowNum { get; set; } = 2;

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06001F5C RID: 8028 RVA: 0x0008C6C6 File Offset: 0x0008A8C6
		// (set) Token: 0x06001F5D RID: 8029 RVA: 0x0008C6CE File Offset: 0x0008A8CE
		public int PaperColumnNum { get; set; } = 2;

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06001F5E RID: 8030 RVA: 0x0008C6D7 File Offset: 0x0008A8D7
		// (set) Token: 0x06001F5F RID: 8031 RVA: 0x0008C6DF File Offset: 0x0008A8DF
		public PrintTypeSettingModel TypesettingModel { get; set; }

		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06001F60 RID: 8032 RVA: 0x0008C6E8 File Offset: 0x0008A8E8
		// (set) Token: 0x06001F61 RID: 8033 RVA: 0x0008C6F0 File Offset: 0x0008A8F0
		public PageOrder PageOrder { get; set; }

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06001F62 RID: 8034 RVA: 0x0008C6F9 File Offset: 0x0008A8F9
		// (set) Token: 0x06001F63 RID: 8035 RVA: 0x0008C701 File Offset: 0x0008A901
		public BookletBindingDirection BindingDirection { get; set; }

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06001F64 RID: 8036 RVA: 0x0008C70A File Offset: 0x0008A90A
		// (set) Token: 0x06001F65 RID: 8037 RVA: 0x0008C712 File Offset: 0x0008A912
		public BookletSubset bookletSubset { get; set; }

		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x06001F66 RID: 8038 RVA: 0x0008C71B File Offset: 0x0008A91B
		// (set) Token: 0x06001F67 RID: 8039 RVA: 0x0008C723 File Offset: 0x0008A923
		public double TilePageZoom { get; set; }

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x06001F68 RID: 8040 RVA: 0x0008C72C File Offset: 0x0008A92C
		// (set) Token: 0x06001F69 RID: 8041 RVA: 0x0008C734 File Offset: 0x0008A934
		public double TileOverlap { get; set; }

		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06001F6A RID: 8042 RVA: 0x0008C73D File Offset: 0x0008A93D
		// (set) Token: 0x06001F6B RID: 8043 RVA: 0x0008C745 File Offset: 0x0008A945
		public bool TileCutMasks { get; set; }

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x06001F6C RID: 8044 RVA: 0x0008C74E File Offset: 0x0008A94E
		// (set) Token: 0x06001F6D RID: 8045 RVA: 0x0008C756 File Offset: 0x0008A956
		public bool TileLabels { get; set; }

		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x06001F6E RID: 8046 RVA: 0x0008C75F File Offset: 0x0008A95F
		// (set) Token: 0x06001F6F RID: 8047 RVA: 0x0008C767 File Offset: 0x0008A967
		public bool PrintBorder { get; set; }

		// Token: 0x17000ACD RID: 2765
		// (get) Token: 0x06001F70 RID: 8048 RVA: 0x0008C770 File Offset: 0x0008A970
		// (set) Token: 0x06001F71 RID: 8049 RVA: 0x0008C778 File Offset: 0x0008A978
		public double PageMargins { get; set; }

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x06001F72 RID: 8050 RVA: 0x0008C781 File Offset: 0x0008A981
		// (set) Token: 0x06001F73 RID: 8051 RVA: 0x0008C789 File Offset: 0x0008A989
		public bool PrintReverse { get; set; }

		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x06001F74 RID: 8052 RVA: 0x0008C792 File Offset: 0x0008A992
		// (set) Token: 0x06001F75 RID: 8053 RVA: 0x0008C79A File Offset: 0x0008A99A
		public double MutilplePageMargins { get; set; }

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x06001F76 RID: 8054 RVA: 0x0008C7A3 File Offset: 0x0008A9A3
		// (set) Token: 0x06001F77 RID: 8055 RVA: 0x0008C7AB File Offset: 0x0008A9AB
		public double TilePageMargins { get; set; }
	}
}
