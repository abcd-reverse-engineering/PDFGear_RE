using System;
using System.Threading;
using System.Windows;

namespace pdfconverter
{
	// Token: 0x02000022 RID: 34
	public class FlowDirectionContext
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x0000470C File Offset: 0x0000290C
		public FlowDirectionContext()
		{
			try
			{
				this.flowDirection = (Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
			}
			catch
			{
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00004754 File Offset: 0x00002954
		public FlowDirection FlowDirection
		{
			get
			{
				return this.flowDirection;
			}
		}

		// Token: 0x04000103 RID: 259
		private FlowDirection flowDirection;
	}
}
