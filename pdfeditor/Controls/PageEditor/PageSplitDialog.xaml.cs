using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Utils;
using pdfeditor.Utils.Behaviors;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000252 RID: 594
	public partial class PageSplitDialog : Window
	{
		// Token: 0x06002261 RID: 8801 RVA: 0x000A1C74 File Offset: 0x0009FE74
		public PageSplitDialog(string docPath, PdfDocument document)
		{
			this.InitializeComponent();
			this.SavePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			this.MaxPageCountTextBehavior.Text = string.Format("{0}", this.maxPageCount);
			this.docPath = docPath;
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.document = document;
			if (document.Pages.Count == 1)
			{
				this.PageRangeTextBox.Text = "1";
				return;
			}
			if (document.Pages.Count == 2)
			{
				this.PageRangeTextBox.Text = "1,2";
				return;
			}
			int num = document.Pages.Count / 2;
			string text;
			if (num - 1 == 0)
			{
				text = "1";
			}
			else
			{
				text = string.Format("1-{0}", num);
			}
			string text2 = string.Format("{0}-{1}", num + 1, document.Pages.Count);
			this.PageRangeTextBox.Text = text + "," + text2;
		}

		// Token: 0x06002262 RID: 8802 RVA: 0x000A1D94 File Offset: 0x0009FF94
		private void MaxPageCountTextBehavior_TextChanged(object sender, EventArgs e)
		{
			int num;
			if (int.TryParse(this.MaxPageCountTextBehavior.Text, out num) && num > 0)
			{
				this.maxPageCount = num;
				return;
			}
			this.MaxPageCountTextBehavior.Text = string.Format("{0}", this.maxPageCount);
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x000A1DE1 File Offset: 0x0009FFE1
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x000A1DEC File Offset: 0x0009FFEC
		private async void OKButton_Click(object sender, RoutedEventArgs e)
		{
			base.IsEnabled = false;
			try
			{
				string text2;
				string text = this.CreateSaveDirectoryName(out text2);
				if (string.IsNullOrEmpty(text))
				{
					base.IsEnabled = true;
					return;
				}
				if (this.MaxPageCountRadioButton.IsChecked.GetValueOrDefault())
				{
					if (this.maxPageCount < 0)
					{
						base.IsEnabled = true;
						return;
					}
					if (!this.CreateSaveDirectory(text))
					{
						base.IsEnabled = true;
						return;
					}
					for (int i = 0; i < this.document.Pages.Count; i += this.maxPageCount)
					{
						int num = i;
						int num2 = Math.Min(i + this.maxPageCount, this.document.Pages.Count - 1);
						string text3 = string.Format("{0}-{1}", num + 1, num2 + 1);
						this.CreateRangeDocument(text3, text2, text);
					}
				}
				else if (this.PageRangeRadioButton.IsChecked.GetValueOrDefault())
				{
					if (this.PageRangeTextBox.HasError || this.PageRangeTextBox.PageIndexes.Count == 0 || this.PageRangeTextBox.PageIndexes.Last<int>() >= this.document.Pages.Count)
					{
						base.IsEnabled = true;
						return;
					}
					int[][] array;
					int num3;
					if (!PdfObjectExtensions.TryParsePageRange2(this.PageRangeTextBox.Text, out array, out num3))
					{
						base.IsEnabled = true;
						return;
					}
					if (!this.CreateSaveDirectory(text))
					{
						base.IsEnabled = true;
						return;
					}
					int[][] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						string text4 = array2[j].ConvertToRange();
						this.CreateRangeDocument(text4, text2, text);
					}
				}
				if (Directory.Exists(text))
				{
					await ExplorerUtils.OpenFolderAsync(text, default(CancellationToken));
					base.IsEnabled = true;
					base.DialogResult = new bool?(true);
				}
			}
			catch
			{
			}
			base.IsEnabled = true;
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x000A1E24 File Offset: 0x000A0024
		private bool CreateSaveDirectory(string saveFolderPath)
		{
			try
			{
				Directory.CreateDirectory(saveFolderPath);
				return true;
			}
			catch
			{
				base.IsEnabled = true;
			}
			return false;
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x000A1E5C File Offset: 0x000A005C
		private void CreateRangeDocument(string range, string docName, string savePath)
		{
			string text = docName + " [" + range + "].pdf";
			string text2 = Path.Combine(savePath, text);
			if (!File.Exists(text2))
			{
				try
				{
					using (FileStream fileStream = new FileStream(text2, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
					{
						using (PdfDocument pdfDocument = PdfDocument.CreateNew(null))
						{
							pdfDocument.Pages.ImportPages(this.document, range, 0);
							pdfDocument.Save(fileStream, SaveFlags.NoIncremental, 0);
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x000A1F00 File Offset: 0x000A0100
		private string CreateSaveDirectoryName(out string docName)
		{
			docName = string.Empty;
			string text = this.SavePath.Text;
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			if (!Directory.Exists(text))
			{
				return string.Empty;
			}
			FileInfo fileInfo = new FileInfo(this.docPath);
			docName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
			if (string.IsNullOrEmpty(docName))
			{
				docName = "Pdf Split";
			}
			string text2 = Path.Combine(text, docName);
			int num = 1;
			while (Directory.Exists(text2))
			{
				text2 = Path.Combine(text, string.Format("{0} ({1})", docName, num));
				num++;
			}
			return text2;
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x000A1FB0 File Offset: 0x000A01B0
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !base.IsEnabled;
			base.OnClosing(e);
		}

		// Token: 0x04000E8F RID: 3727
		private int maxPageCount = 5;

		// Token: 0x04000E90 RID: 3728
		private readonly string docPath;

		// Token: 0x04000E91 RID: 3729
		private readonly PdfDocument document;
	}
}
