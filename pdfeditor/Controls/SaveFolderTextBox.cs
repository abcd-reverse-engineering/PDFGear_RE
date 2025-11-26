using System;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace pdfeditor.Controls
{
	// Token: 0x020001C1 RID: 449
	public class SaveFolderTextBox : PathTextBox
	{
		// Token: 0x0600198B RID: 6539 RVA: 0x000652C2 File Offset: 0x000634C2
		protected override object CreateDialog(string initialDirectory, string filename)
		{
			return new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = initialDirectory
			};
		}
	}
}
