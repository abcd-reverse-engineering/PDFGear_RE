using System;

namespace pdfeditor.Controls
{
	// Token: 0x020001BF RID: 447
	public class PathTextBoxPathChangedEventArgs : EventArgs
	{
		// Token: 0x06001983 RID: 6531 RVA: 0x00065224 File Offset: 0x00063424
		public PathTextBoxPathChangedEventArgs(string newPath, string oldPath)
		{
			this.NewPath = newPath;
			this.OldPath = oldPath;
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06001984 RID: 6532 RVA: 0x0006523A File Offset: 0x0006343A
		public string NewPath { get; }

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06001985 RID: 6533 RVA: 0x00065242 File Offset: 0x00063442
		public string OldPath { get; }
	}
}
