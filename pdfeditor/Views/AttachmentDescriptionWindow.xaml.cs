using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Attachments;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;

namespace pdfeditor.Views
{
	// Token: 0x02000049 RID: 73
	public partial class AttachmentDescriptionWindow : Window
	{
		// Token: 0x0600035B RID: 859 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
		public AttachmentDescriptionWindow(MainViewModel vm, PDFAttachmentWrapper attachmentWrapper)
		{
			this.InitializeComponent();
			this.viewModel = vm;
			this.wrapper = attachmentWrapper;
			base.Owner = Application.Current.MainWindow;
			base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (this.wrapper != null && this.wrapper.Attachment != null)
			{
				this.TextBox_Description.Text = this.wrapper.Description;
				if (!string.IsNullOrEmpty(this.TextBox_Description.Text))
				{
					this.TextBox_Description.Focus();
					this.TextBox_Description.SelectAll();
				}
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000FF32 File Offset: 0x0000E132
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000FF40 File Offset: 0x0000E140
		private async void btnOk_Click(object sender, RoutedEventArgs e)
		{
			string descriptionBackup = this.wrapper.Description;
			if (this.wrapper != null && descriptionBackup != this.TextBox_Description.Text && (!string.IsNullOrEmpty(this.TextBox_Description.Text) || descriptionBackup != null))
			{
				if (this.TextBox_Description.Text.Length > 256)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.Msg_DescriptionOverMaxCharsNum, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				if (this.viewModel != null)
				{
					MainViewModel mainViewModel = this.viewModel;
					if (((mainViewModel != null) ? mainViewModel.Document : null) != null)
					{
						if (this.wrapper.Attachment is PdfAttachment)
						{
							string description = this.TextBox_Description.Text;
							this.wrapper.Description = description;
							await this.viewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
							{
								this.wrapper.Description = descriptionBackup;
							}, delegate(PdfDocument doc)
							{
								this.wrapper.Description = description;
							}, "");
							goto IL_0225;
						}
						PdfFileAttachmentAnnotation pdfFileAttachmentAnnotation = this.wrapper.Attachment as PdfFileAttachmentAnnotation;
						if (pdfFileAttachmentAnnotation == null || pdfFileAttachmentAnnotation.Page == null)
						{
							goto IL_0225;
						}
						using (this.viewModel.OperationManager.TraceAnnotationChange(pdfFileAttachmentAnnotation.Page, ""))
						{
							this.wrapper.Description = this.TextBox_Description.Text;
						}
						PageEditorViewModel pageEditors = this.viewModel.PageEditors;
						if (pageEditors == null)
						{
							goto IL_0225;
						}
						pageEditors.NotifyPageAnnotationChanged(pdfFileAttachmentAnnotation.Page.PageIndex);
						goto IL_0225;
					}
				}
				return;
			}
			IL_0225:
			base.DialogResult = new bool?(true);
		}

		// Token: 0x0400015B RID: 347
		private PDFAttachmentWrapper wrapper;

		// Token: 0x0400015C RID: 348
		private MainViewModel viewModel;
	}
}
