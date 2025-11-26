using System;

namespace XmpCore.Impl.XPath
{
	// Token: 0x02000045 RID: 69
	public sealed class XmpPathSegment
	{
		// Token: 0x0600031A RID: 794 RVA: 0x0000F20A File Offset: 0x0000D40A
		public XmpPathSegment(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000F219 File Offset: 0x0000D419
		public XmpPathSegment(string name, XmpPathStepType kind)
		{
			this.Name = name;
			this.Kind = kind;
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000F22F File Offset: 0x0000D42F
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0000F237 File Offset: 0x0000D437
		public XmpPathStepType Kind { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0000F240 File Offset: 0x0000D440
		// (set) Token: 0x0600031F RID: 799 RVA: 0x0000F248 File Offset: 0x0000D448
		public string Name { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0000F251 File Offset: 0x0000D451
		// (set) Token: 0x06000321 RID: 801 RVA: 0x0000F259 File Offset: 0x0000D459
		public bool IsAlias { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0000F262 File Offset: 0x0000D462
		// (set) Token: 0x06000323 RID: 803 RVA: 0x0000F26A File Offset: 0x0000D46A
		public int AliasForm { get; set; }

		// Token: 0x06000324 RID: 804 RVA: 0x0000F274 File Offset: 0x0000D474
		public override string ToString()
		{
			XmpPathStepType kind = this.Kind;
			if (kind - XmpPathStepType.StructFieldStep <= 3)
			{
				return this.Name;
			}
			if (kind - XmpPathStepType.QualSelectorStep > 1)
			{
				return this.Name;
			}
			return this.Name;
		}
	}
}
