using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000286 RID: 646
	public partial class DigitalSignatureValidationIcon : UserControl
	{
		// Token: 0x0600254B RID: 9547 RVA: 0x000AD87B File Offset: 0x000ABA7B
		public DigitalSignatureValidationIcon()
		{
			this.InitializeComponent();
			this.UpdateIconVisibility();
		}

		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x0600254C RID: 9548 RVA: 0x000AD88F File Offset: 0x000ABA8F
		// (set) Token: 0x0600254D RID: 9549 RVA: 0x000AD8A1 File Offset: 0x000ABAA1
		public PdfSignatureValidationResult ValidationResult
		{
			get
			{
				return (PdfSignatureValidationResult)base.GetValue(DigitalSignatureValidationIcon.ValidationResultProperty);
			}
			set
			{
				base.SetValue(DigitalSignatureValidationIcon.ValidationResultProperty, value);
			}
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x000AD8B0 File Offset: 0x000ABAB0
		private void UpdateIconVisibility()
		{
			this.UnknownIcon.Visibility = Visibility.Collapsed;
			this.InvalidIcon.Visibility = Visibility.Collapsed;
			this.ValidIcon.Visibility = Visibility.Collapsed;
			this.UnsignedIcon.Visibility = Visibility.Collapsed;
			PdfSignatureValidationResult validationResult = this.ValidationResult;
			if (validationResult == null)
			{
				this.UnsignedIcon.Visibility = Visibility.Visible;
				return;
			}
			if (validationResult.IsValid(true))
			{
				this.ValidIcon.Visibility = Visibility.Visible;
				return;
			}
			if (validationResult.SignatureStatus == SignatureStatus.Unknown)
			{
				this.UnknownIcon.Visibility = Visibility.Visible;
				return;
			}
			this.InvalidIcon.Visibility = Visibility.Visible;
		}

		// Token: 0x04000FF5 RID: 4085
		public static readonly DependencyProperty ValidationResultProperty = DependencyProperty.Register("ValidationResult", typeof(PdfSignatureValidationResult), typeof(DigitalSignatureValidationIcon), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DigitalSignatureValidationIcon digitalSignatureValidationIcon = s as DigitalSignatureValidationIcon;
			if (digitalSignatureValidationIcon != null && !object.Equals(a.NewValue, a.OldValue))
			{
				digitalSignatureValidationIcon.UpdateIconVisibility();
			}
		}));
	}
}
