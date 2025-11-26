using System;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000028 RID: 40
	public class SaveFolderTextBox : PathTextBox
	{
		// Token: 0x060001F7 RID: 503 RVA: 0x000076AE File Offset: 0x000058AE
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
