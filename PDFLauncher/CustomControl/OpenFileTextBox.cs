using System;
using System.Windows;
using Microsoft.Win32;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000027 RID: 39
	public class OpenFileTextBox : PathTextBox
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x00007636 File Offset: 0x00005836
		protected override object CreateDialog(string initialDirectory, string filename)
		{
			return new OpenFileDialog
			{
				InitialDirectory = initialDirectory,
				FileName = filename,
				Filter = this.Filter
			};
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00007657 File Offset: 0x00005857
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x00007669 File Offset: 0x00005869
		public string Filter
		{
			get
			{
				return (string)base.GetValue(OpenFileTextBox.FilterProperty);
			}
			set
			{
				base.SetValue(OpenFileTextBox.FilterProperty, value);
			}
		}

		// Token: 0x040000E9 RID: 233
		public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(OpenFileTextBox), new PropertyMetadata(string.Empty));
	}
}
