using System;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000026 RID: 38
	public class PathTextBoxPathChangedEventArgs : EventArgs
	{
		// Token: 0x060001EF RID: 495 RVA: 0x00007610 File Offset: 0x00005810
		public PathTextBoxPathChangedEventArgs(string newPath, string oldPath)
		{
			this.NewPath = newPath;
			this.OldPath = oldPath;
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x00007626 File Offset: 0x00005826
		public string NewPath { get; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000762E File Offset: 0x0000582E
		public string OldPath { get; }
	}
}
