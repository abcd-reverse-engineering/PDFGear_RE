using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Utils.DigitalSignatures;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000289 RID: 649
	public partial class SignatureValidationStatusDialog : Window
	{
		// Token: 0x06002569 RID: 9577 RVA: 0x000AE218 File Offset: 0x000AC418
		public SignatureValidationStatusDialog(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			this.InitializeComponent();
			this.field = field;
			this.result = result;
			this.ValidationIcon.ValidationResult = result;
			this.Title.Text = SignatureFormatHelper.BuildValidateResultTitle(field, result);
			this.ContentItems.ItemsSource = SignatureFormatHelper.BuildValidateResultStatusContent(field, result);
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x000AE26F File Offset: 0x000AC46F
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x04001012 RID: 4114
		private readonly PdfDigitalSignatureField field;

		// Token: 0x04001013 RID: 4115
		private readonly PdfSignatureValidationResult result;
	}
}
