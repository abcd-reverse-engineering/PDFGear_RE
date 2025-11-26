using System;
using System.Windows;
using Microsoft.Win32;

namespace pdfeditor.Controls
{
	// Token: 0x020001C0 RID: 448
	public class OpenFileTextBox : PathTextBox
	{
		// Token: 0x06001986 RID: 6534 RVA: 0x0006524A File Offset: 0x0006344A
		protected override object CreateDialog(string initialDirectory, string filename)
		{
			return new OpenFileDialog
			{
				InitialDirectory = initialDirectory,
				FileName = filename,
				Filter = this.Filter
			};
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x0006526B File Offset: 0x0006346B
		// (set) Token: 0x06001988 RID: 6536 RVA: 0x0006527D File Offset: 0x0006347D
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

		// Token: 0x040008C7 RID: 2247
		public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(OpenFileTextBox), new PropertyMetadata(string.Empty));
	}
}
