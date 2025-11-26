using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf.Net;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Views
{
	// Token: 0x0200004A RID: 74
	public partial class DocumentPropertiesWindow : Window
	{
		// Token: 0x06000360 RID: 864 RVA: 0x00010028 File Offset: 0x0000E228
		public DocumentPropertiesWindow(PdfDocument document, int PageIndex, string FilePath)
		{
			if (document == null)
			{
				return;
			}
			this.pdfDocument = document;
			this.pageSeletedIndex = PageIndex;
			this.InitializeComponent();
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			this.Filename.Text = Path.GetFileName(FilePath);
			this.TotalPages.Text = document.Pages.Count.ToString();
			if (ConfigManager.GetDocumentPropertiesUnit() == 0)
			{
				this.Unit.SelectedIndex = 0;
				this.PageSize.Text = PageHeaderFooterUtils.PdfPointToCm((double)document.Pages[PageIndex - 1].Width).ToString("0.000") + " x " + PageHeaderFooterUtils.PdfPointToCm((double)document.Pages[PageIndex - 1].Height).ToString("0.000");
			}
			else
			{
				this.Unit.SelectedIndex = 1;
				this.PageSize.Text = (this.pdfDocument.Pages[this.pageSeletedIndex - 1].Width / 72f).ToString("0.000") + " x " + (this.pdfDocument.Pages[this.pageSeletedIndex - 1].Height / 72f).ToString("0.000");
			}
			this.Title.Text = requiredService.DocumentWrapper.Metadata.Title;
			if (requiredService.DocumentWrapper.Metadata.Author.Length != 0)
			{
				this.Author.Text = requiredService.DocumentWrapper.Metadata.Author[0];
			}
			else
			{
				this.Author.Text = "";
			}
			this.authorBackup = requiredService.DocumentWrapper.Metadata.Author;
			this.subjectBackup = requiredService.DocumentWrapper.Metadata.Subject;
			this.keywordBackup = requiredService.DocumentWrapper.Metadata.Keywords;
			this.titleBackup = requiredService.DocumentWrapper.Metadata.Title;
			this.Subject.Text = requiredService.DocumentWrapper.Metadata.Subject;
			this.Keyword.Text = requiredService.DocumentWrapper.Metadata.Keywords;
			this.Creator.Text = requiredService.DocumentWrapper.Metadata.Creator;
			this.Producer.Text = requiredService.DocumentWrapper.Metadata.Producer;
			if (this.Producer.Text.Length > 100)
			{
				this.Producer.ToolTip = this.Producer.Text;
			}
			if (this.Creator.Text.Length > 100)
			{
				this.Creator.ToolTip = this.Creator.Text;
			}
			if (this.Filename.Text.Length > 50)
			{
				this.Filename.ToolTip = this.Filename.Text;
				string text = this.Filename.Text;
				for (int i = 30; i < this.Filename.Text.Length; i += 30)
				{
					text = text.Insert(i, "\n");
				}
				this.Filename.ToolTip = text;
			}
			int pdfFileVersion = requiredService.DocumentWrapper.Metadata.PdfFileVersion;
			this.Version.Text = string.Format("PDF-{0}.{1}", pdfFileVersion / 10, pdfFileVersion % 10);
			this.Location.Text = FilePath;
			this.FileSize.Text = DocumentPropertiesWindow.FormatFileSize(DocumentPropertiesWindow.GetFileSize(FilePath));
			if (requiredService.DocumentWrapper.Metadata.CreationDate == default(DateTimeOffset))
			{
				this.Created.Text = "";
			}
			else
			{
				string text2 = requiredService.DocumentWrapper.Metadata.CreationDate.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
				this.Created.Text = text2;
			}
			if (requiredService.DocumentWrapper.Metadata.ModificationDate == default(DateTimeOffset))
			{
				this.Modified.Text = "";
			}
			else
			{
				string text3 = requiredService.DocumentWrapper.Metadata.ModificationDate.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
				this.Modified.Text = text3;
			}
			GAManager.SendEvent("DocumentPropertiesWindow", "Show", "Count", 1L);
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000104BC File Offset: 0x0000E6BC
		public static long GetFileSize(string filePath)
		{
			return new FileInfo(filePath).Length;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000104CC File Offset: 0x0000E6CC
		public static string FormatFileSize(long fileSize)
		{
			string[] array = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
			if (fileSize == 0L)
			{
				return "0" + array[0];
			}
			int num = (int)Math.Floor(Math.Log((double)fileSize, 1024.0));
			double num2 = Math.Round((double)fileSize / Math.Pow(1024.0, (double)num), 2);
			return string.Format("{0} {1}", num2, array[num]);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00010580 File Offset: 0x0000E780
		private async void btnOk_ClickAsync(object sender, RoutedEventArgs e)
		{
			bool flag = false;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			string title = this.Title.Text;
			string[] author = new string[] { this.Author.Text };
			string subject = this.Subject.Text;
			string keyword = this.Keyword.Text;
			if (this.Title.Text != requiredService.DocumentWrapper.Metadata.Title)
			{
				requiredService.DocumentWrapper.Metadata.Title = this.Title.Text;
				flag = true;
			}
			if (((requiredService.DocumentWrapper.Metadata.Author == null || requiredService.DocumentWrapper.Metadata.Author.Length == 0) && !string.IsNullOrEmpty(this.Author.Text)) || (requiredService.DocumentWrapper.Metadata.Author.Length != 0 && this.Author.Text != requiredService.DocumentWrapper.Metadata.Author[0]))
			{
				requiredService.DocumentWrapper.Metadata.Author = new string[] { this.Author.Text };
				flag = true;
			}
			if (this.Subject.Text != requiredService.DocumentWrapper.Metadata.Subject)
			{
				requiredService.DocumentWrapper.Metadata.Subject = this.Subject.Text;
				flag = true;
			}
			if (this.Keyword.Text != requiredService.DocumentWrapper.Metadata.Keywords)
			{
				requiredService.DocumentWrapper.Metadata.Keywords = this.Keyword.Text;
				flag = true;
			}
			if (flag)
			{
				GAManager.SendEvent("DocumentPropertiesWindow", "OKBtn", "Update", 1L);
				await requiredService.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
				{
					MainViewModel mainViewModel = global::PDFKit.PdfControl.GetPdfControl(doc).DataContext as MainViewModel;
					mainViewModel.DocumentWrapper.Metadata.Title = this.titleBackup;
					mainViewModel.DocumentWrapper.Metadata.Author = this.authorBackup;
					mainViewModel.DocumentWrapper.Metadata.Subject = this.subjectBackup;
					mainViewModel.DocumentWrapper.Metadata.Keywords = this.keywordBackup;
				}, delegate(PdfDocument doc)
				{
					MainViewModel mainViewModel2 = global::PDFKit.PdfControl.GetPdfControl(doc).DataContext as MainViewModel;
					mainViewModel2.DocumentWrapper.Metadata.Title = title;
					mainViewModel2.DocumentWrapper.Metadata.Author = author;
					mainViewModel2.DocumentWrapper.Metadata.Subject = subject;
					mainViewModel2.DocumentWrapper.Metadata.Keywords = keyword;
				}, "");
			}
			else
			{
				GAManager.SendEvent("DocumentPropertiesWindow", "OKBtn", "NoUpdate", 1L);
			}
			base.Close();
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000105B7 File Offset: 0x0000E7B7
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DocumentPropertiesWindow", "CancelBtn", "Count", 1L);
			base.Close();
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000105D8 File Offset: 0x0000E7D8
		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DocumentPropertiesWindow", "OpenLocation", "Count", 1L);
			if (!string.IsNullOrEmpty(this.Location.Text))
			{
				string text = this.Location.Text;
				Process.Start("explorer.exe", "/select,\"" + text + "\"");
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00010634 File Offset: 0x0000E834
		private void Unit_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.pageSeletedIndex == -1 || this.pdfDocument == null)
			{
				return;
			}
			ComboBox comboBox = sender as ComboBox;
			if (comboBox.SelectedIndex == 0)
			{
				this.PageSize.Text = PageHeaderFooterUtils.PdfPointToCm((double)this.pdfDocument.Pages[this.pageSeletedIndex - 1].Width).ToString("0.000") + " x " + PageHeaderFooterUtils.PdfPointToCm((double)this.pdfDocument.Pages[this.pageSeletedIndex - 1].Height).ToString("0.000");
			}
			else
			{
				this.PageSize.Text = (this.pdfDocument.Pages[this.pageSeletedIndex - 1].Width / 72f).ToString("0.000") + " x " + (this.pdfDocument.Pages[this.pageSeletedIndex - 1].Height / 72f).ToString("0.000");
			}
			ConfigManager.SetDocumentPropertiesUnit(comboBox.SelectedIndex);
		}

		// Token: 0x04000161 RID: 353
		private PdfDocument pdfDocument;

		// Token: 0x04000162 RID: 354
		private string titleBackup;

		// Token: 0x04000163 RID: 355
		private string[] authorBackup;

		// Token: 0x04000164 RID: 356
		private string subjectBackup;

		// Token: 0x04000165 RID: 357
		private string keywordBackup;

		// Token: 0x04000166 RID: 358
		private int pageSeletedIndex = -1;
	}
}
