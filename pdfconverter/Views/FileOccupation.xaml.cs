using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfconverter.Properties;
using pdfconverter.Utils;

namespace pdfconverter.Views
{
	// Token: 0x02000027 RID: 39
	public partial class FileOccupation : Window
	{
		// Token: 0x06000213 RID: 531 RVA: 0x00007BA9 File Offset: 0x00005DA9
		public FileOccupation(string fileName)
		{
			this.InitializeComponent();
			this.FileName = fileName;
			this.textBlockExplain.Text = pdfconverter.Properties.Resources.WinFileOccupationExplain.Replace("xxx", Path.GetFileName(fileName));
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00007BEC File Offset: 0x00005DEC
		private void Retry_Click(object sender, RoutedEventArgs e)
		{
			if (ConToPDFUtils.CheckAccess(this.FileName).GetValueOrDefault())
			{
				base.DialogResult = new bool?(true);
				base.Close();
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00007C20 File Offset: 0x00005E20
		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x04000134 RID: 308
		private string FileName = string.Empty;
	}
}
