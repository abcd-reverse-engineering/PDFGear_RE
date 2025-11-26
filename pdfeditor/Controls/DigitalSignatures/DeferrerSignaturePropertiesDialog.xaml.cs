using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Patagames.Pdf.Net.AcroForms;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000283 RID: 643
	public partial class DeferrerSignaturePropertiesDialog : Window
	{
		// Token: 0x0600252E RID: 9518 RVA: 0x000AD238 File Offset: 0x000AB438
		public DeferrerSignaturePropertiesDialog(PdfDigitalSignatureLocation location)
		{
			DeferrerSignaturePropertiesDialog.<>c__DisplayClass0_0 CS$<>8__locals1 = new DeferrerSignaturePropertiesDialog.<>c__DisplayClass0_0();
			CS$<>8__locals1.location = location;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.InitializeComponent();
			this.NameTextBox.Text = CS$<>8__locals1.location.Name;
			TextBox tipTextBox = this.TipTextBox;
			PdfSignatureField signatureField = CS$<>8__locals1.location.SignatureField;
			tipTextBox.Text = ((signatureField != null) ? signatureField.AlternateName : null) ?? "";
			this.OkBtn.Click += delegate(object s, RoutedEventArgs a)
			{
				DeferrerSignaturePropertiesDialog.<>c__DisplayClass0_0.<<-ctor>b__0>d <<-ctor>b__0>d;
				<<-ctor>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<-ctor>b__0>d.<>4__this = CS$<>8__locals1;
				<<-ctor>b__0>d.<>1__state = -1;
				<<-ctor>b__0>d.<>t__builder.Start<DeferrerSignaturePropertiesDialog.<>c__DisplayClass0_0.<<-ctor>b__0>d>(ref <<-ctor>b__0>d);
			};
		}
	}
}
