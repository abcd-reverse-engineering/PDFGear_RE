using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000288 RID: 648
	public partial class SignaturePropertiesDialog : Window
	{
		// Token: 0x06002561 RID: 9569 RVA: 0x000ADE0A File Offset: 0x000AC00A
		public SignaturePropertiesDialog(PdfDigitalSignatureField field, PdfSignatureValidationResult result)
		{
			this.InitializeComponent();
			this.field = field;
			this.result = result;
			this.Refresh();
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x000ADE2C File Offset: 0x000AC02C
		private void Refresh()
		{
			if (this.result.SignatureStatus == SignatureStatus.Valid)
			{
				this.TrustCertificateButton.Visibility = Visibility.Collapsed;
			}
			else if (this.result != null && this.result.Certificates.Count > 0 && CertificateManager.VerificationCertificateStorage.GetCertificate(this.result.Certificates[0].Thumbprint) != null)
			{
				this.TrustCertificateButton.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.TrustCertificateButton.Visibility = Visibility.Visible;
			}
			this.Title.Text = SignatureFormatHelper.BuildValidateResultTitle(this.field, this.result);
			this.ValiditySummaryText.Text = this.GetSignaturePropertiesValidationResultText();
			this.ValidationIcon.ValidationResult = null;
			this.ValidationIcon.ValidationResult = this.result;
			this.UpdateProperties();
		}

		// Token: 0x06002563 RID: 9571 RVA: 0x000ADEFC File Offset: 0x000AC0FC
		private void UpdateProperties()
		{
			this.SignedText.Text = this.field.SignedName;
			TextBox reasonText = this.ReasonText;
			PdfSignature signature = this.field.Signature;
			reasonText.Text = ((signature != null) ? signature.Reason : null) ?? "";
			TextBox dateText = this.DateText;
			PdfSignature signature2 = this.field.Signature;
			dateText.Text = ((signature2 != null) ? signature2.SignedDate.ToString("G") : null);
			TextBox locationText = this.LocationText;
			PdfSignature signature3 = this.field.Signature;
			locationText.Text = ((signature3 != null) ? signature3.LocationInfo : null) ?? "";
			TextBox contractText = this.ContractText;
			PdfSignature signature4 = this.field.Signature;
			contractText.Text = ((!string.IsNullOrEmpty((signature4 != null) ? signature4.ContactInfo : null)) ? this.field.Signature.ContactInfo : pdfeditor.Properties.Resources.DigSignPropContactInfo_NotAvailable);
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x000ADFE4 File Offset: 0x000AC1E4
		private string GetSignaturePropertiesValidationResultText()
		{
			StringBuilder stringBuilder = SignatureFormatHelper.BuildSignaturePropertiesValidationResult(this.field, this.result).Aggregate(new StringBuilder(), (StringBuilder _sb, DigitalSignatureFormattedItem s) => _sb.AppendLine(s.Text).AppendLine());
			while (stringBuilder[stringBuilder.Length - 1] == '\n' || stringBuilder[stringBuilder.Length - 1] == '\r')
			{
				StringBuilder stringBuilder2 = stringBuilder;
				int length = stringBuilder2.Length;
				stringBuilder2.Length = length - 1;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x000AE068 File Offset: 0x000AC268
		private async void ShowCertificateButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				GAManager.SendEvent("DigitalSignature", "ShowCertificate", "ShowSignatureProperties", 1L);
				((Button)sender).IsEnabled = false;
				PdfSignatureValidationResult pdfSignatureValidationResult = await this.field.ValidateSignatureAsync(true, true);
				if (pdfSignatureValidationResult != null && pdfSignatureValidationResult.Certificates.Count > 0)
				{
					IntPtr intPtr = IntPtr.Zero;
					Window mainWindow = App.Current.MainWindow;
					if (mainWindow != null)
					{
						HwndSource hwndSource = PresentationSource.FromVisual(mainWindow) as HwndSource;
						if (hwndSource != null)
						{
							intPtr = hwndSource.Handle;
						}
					}
					pdfSignatureValidationResult.Certificates[0].ShowCertificateDialog(intPtr, null);
				}
			}
			finally
			{
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x000AE0A8 File Offset: 0x000AC2A8
		private async void TrustCertificateButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				((Button)sender).IsEnabled = false;
				PdfSignatureValidationResult pdfSignatureValidationResult = await this.field.ValidateSignatureAsync(true, true);
				if (pdfSignatureValidationResult != null && pdfSignatureValidationResult.Certificates.Count > 0 && ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("SignPropDialogMessage_SaveCert"), UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.No, null, false) == MessageBoxResult.Yes)
				{
					GAManager.SendEvent("DigitalSignature", "TrustCertificate", "Count", 1L);
					if (CertificateManager.VerificationCertificateStorage.SaveCertificate(pdfSignatureValidationResult.Certificates[0].Export(X509ContentType.Cert), null))
					{
						Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
						this.result = await this.field.ValidateSignatureAsync(false, true);
						this.Refresh();
					}
				}
			}
			finally
			{
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x04001005 RID: 4101
		private readonly PdfDigitalSignatureField field;

		// Token: 0x04001006 RID: 4102
		private PdfSignatureValidationResult result;
	}
}
