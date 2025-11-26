using System;
using System.Drawing.Printing;
using Newtonsoft.Json;
using pdfeditor.Utils.Print;

namespace pdfeditor.Models.Print
{
	// Token: 0x0200014B RID: 331
	public class PrintSettings
	{
		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x060013BC RID: 5052 RVA: 0x0004F719 File Offset: 0x0004D919
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x0004F721 File Offset: 0x0004D921
		[JsonIgnore]
		public string Device { get; set; }

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x0004F72A File Offset: 0x0004D92A
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x0004F732 File Offset: 0x0004D932
		public PaperSize Paper { get; set; }

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x0004F73B File Offset: 0x0004D93B
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x0004F743 File Offset: 0x0004D943
		public bool IsGrayscale { get; set; }

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0004F74C File Offset: 0x0004D94C
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x0004F754 File Offset: 0x0004D954
		public Duplex Duplex { get; set; } = Duplex.Simplex;

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x060013C4 RID: 5060 RVA: 0x0004F75D File Offset: 0x0004D95D
		// (set) Token: 0x060013C5 RID: 5061 RVA: 0x0004F765 File Offset: 0x0004D965
		public bool Landscape { get; set; }

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x060013C6 RID: 5062 RVA: 0x0004F76E File Offset: 0x0004D96E
		// (set) Token: 0x060013C7 RID: 5063 RVA: 0x0004F776 File Offset: 0x0004D976
		public PrintSizeMode PrintSizeMode { get; set; }

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x060013C8 RID: 5064 RVA: 0x0004F77F File Offset: 0x0004D97F
		// (set) Token: 0x060013C9 RID: 5065 RVA: 0x0004F787 File Offset: 0x0004D987
		public int PapersPerSheet { get; set; } = 4;

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x0004F790 File Offset: 0x0004D990
		// (set) Token: 0x060013CB RID: 5067 RVA: 0x0004F798 File Offset: 0x0004D998
		public int PaperRowNum { get; set; } = 2;

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x060013CC RID: 5068 RVA: 0x0004F7A1 File Offset: 0x0004D9A1
		// (set) Token: 0x060013CD RID: 5069 RVA: 0x0004F7A9 File Offset: 0x0004D9A9
		public int PaperColumnNum { get; set; } = 2;

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x060013CE RID: 5070 RVA: 0x0004F7B2 File Offset: 0x0004D9B2
		// (set) Token: 0x060013CF RID: 5071 RVA: 0x0004F7BA File Offset: 0x0004D9BA
		public PrintTypeSettingModel TypesettingModel { get; set; }

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x060013D0 RID: 5072 RVA: 0x0004F7C3 File Offset: 0x0004D9C3
		// (set) Token: 0x060013D1 RID: 5073 RVA: 0x0004F7CB File Offset: 0x0004D9CB
		public PageOrder PageOrder { get; set; }

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x060013D2 RID: 5074 RVA: 0x0004F7D4 File Offset: 0x0004D9D4
		// (set) Token: 0x060013D3 RID: 5075 RVA: 0x0004F7DC File Offset: 0x0004D9DC
		public BookletBindingDirection BindingDirection { get; set; }

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x0004F7E5 File Offset: 0x0004D9E5
		// (set) Token: 0x060013D5 RID: 5077 RVA: 0x0004F7ED File Offset: 0x0004D9ED
		public BookletSubset bookletSubset { get; set; }

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x060013D6 RID: 5078 RVA: 0x0004F7F6 File Offset: 0x0004D9F6
		// (set) Token: 0x060013D7 RID: 5079 RVA: 0x0004F7FE File Offset: 0x0004D9FE
		public double TilePageZoom { get; set; }

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060013D8 RID: 5080 RVA: 0x0004F807 File Offset: 0x0004DA07
		// (set) Token: 0x060013D9 RID: 5081 RVA: 0x0004F80F File Offset: 0x0004DA0F
		public double TileOverlap { get; set; }

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060013DA RID: 5082 RVA: 0x0004F818 File Offset: 0x0004DA18
		// (set) Token: 0x060013DB RID: 5083 RVA: 0x0004F820 File Offset: 0x0004DA20
		public bool TileCutMasks { get; set; }

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060013DC RID: 5084 RVA: 0x0004F829 File Offset: 0x0004DA29
		// (set) Token: 0x060013DD RID: 5085 RVA: 0x0004F831 File Offset: 0x0004DA31
		public bool TileLabels { get; set; }

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060013DE RID: 5086 RVA: 0x0004F83A File Offset: 0x0004DA3A
		// (set) Token: 0x060013DF RID: 5087 RVA: 0x0004F842 File Offset: 0x0004DA42
		public bool PrintBorder { get; set; }

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060013E0 RID: 5088 RVA: 0x0004F84B File Offset: 0x0004DA4B
		// (set) Token: 0x060013E1 RID: 5089 RVA: 0x0004F853 File Offset: 0x0004DA53
		public double PageMargins { get; set; }

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060013E2 RID: 5090 RVA: 0x0004F85C File Offset: 0x0004DA5C
		// (set) Token: 0x060013E3 RID: 5091 RVA: 0x0004F864 File Offset: 0x0004DA64
		public bool PrintReverse { get; set; }

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060013E4 RID: 5092 RVA: 0x0004F86D File Offset: 0x0004DA6D
		// (set) Token: 0x060013E5 RID: 5093 RVA: 0x0004F875 File Offset: 0x0004DA75
		public double MutilplePageMargins { get; set; }

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060013E6 RID: 5094 RVA: 0x0004F87E File Offset: 0x0004DA7E
		// (set) Token: 0x060013E7 RID: 5095 RVA: 0x0004F886 File Offset: 0x0004DA86
		public double TilePageMargins { get; set; }
	}
}
