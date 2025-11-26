using System;
using System.ComponentModel;
using pdfeditor.Properties;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000BB RID: 187
	internal class ScannerResourceAttribute : DescriptionAttribute
	{
		// Token: 0x06000B2B RID: 2859 RVA: 0x00039B56 File Offset: 0x00037D56
		public ScannerResourceAttribute(string name)
		{
			this.name = name;
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000B2C RID: 2860 RVA: 0x00039B65 File Offset: 0x00037D65
		public override string Description
		{
			get
			{
				return Resources.ResourceManager.GetString(this.name) ?? this.name;
			}
		}

		// Token: 0x040004D1 RID: 1233
		private readonly string name;
	}
}
