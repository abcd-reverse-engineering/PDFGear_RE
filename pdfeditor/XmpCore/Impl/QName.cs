using System;

namespace XmpCore.Impl
{
	// Token: 0x02000034 RID: 52
	public sealed class QName
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x0000671C File Offset: 0x0000491C
		public QName(string qname)
		{
			int num = qname.IndexOf(':');
			if (num >= 0)
			{
				this.Prefix = qname.Substring(0, num);
				this.LocalName = qname.Substring(num + 1);
				return;
			}
			this.Prefix = string.Empty;
			this.LocalName = qname;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000676C File Offset: 0x0000496C
		public QName(string prefix, string localName)
		{
			this.Prefix = prefix;
			this.LocalName = localName;
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x00006782 File Offset: 0x00004982
		public bool HasPrefix
		{
			get
			{
				return !string.IsNullOrEmpty(this.Prefix);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x00006792 File Offset: 0x00004992
		public string LocalName { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000679A File Offset: 0x0000499A
		public string Prefix { get; }
	}
}
