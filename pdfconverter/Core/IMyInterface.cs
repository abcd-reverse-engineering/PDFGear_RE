using System;

namespace pdfconverter.Core
{
	// Token: 0x0200008C RID: 140
	public interface IMyInterface
	{
		// Token: 0x17000244 RID: 580
		// (get) Token: 0x0600068A RID: 1674
		// (set) Token: 0x0600068B RID: 1675
		string OutFolder { get; set; }

		// Token: 0x0600068C RID: 1676
		void ToPDF();
	}
}
