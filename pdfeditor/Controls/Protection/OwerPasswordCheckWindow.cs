using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfeditor.Models;
using pdfeditor.Properties;
using pdfeditor.Utils;

namespace pdfeditor.Controls.Protection
{
	// Token: 0x02000222 RID: 546
	public class OwerPasswordCheckWindow : Window, IComponentConnector
	{
		// Token: 0x06001E8D RID: 7821 RVA: 0x00087DA2 File Offset: 0x00085FA2
		public OwerPasswordCheckWindow(DocumentWrapper document)
		{
			this.InitializeComponent();
			this.Pdf = document;
		}

		// Token: 0x06001E8E RID: 7822 RVA: 0x00087DB8 File Offset: 0x00085FB8
		private void OK_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.passwordBox.Password))
			{
				this.tbError.Visibility = Visibility.Visible;
				this.tbError.Text = pdfeditor.Properties.Resources.WinPwdPasswordCheckEmptyContent;
				return;
			}
			try
			{
				if (EncryptUtils.VerifyOwerpassword(this.Pdf.DocumentPath, this.passwordBox.Password))
				{
					base.DialogResult = new bool?(true);
					this.Pdf.EncryptManage.UpdateOwerPassword(this.passwordBox.Password);
					this.Pdf.EncryptManage.IsRequiredOwerPassword = false;
					base.Close();
				}
				else
				{
					this.tbError.Text = pdfeditor.Properties.Resources.WinPwdWrongCheckContent;
					this.tbError.Visibility = Visibility.Visible;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001E8F RID: 7823 RVA: 0x00087E84 File Offset: 0x00086084
		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x06001E90 RID: 7824 RVA: 0x00087E98 File Offset: 0x00086098
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/pdfeditor;component/controls/protect/owerpasswordcheckwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001E91 RID: 7825 RVA: 0x00087EC8 File Offset: 0x000860C8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.passwordBox = (PasswordBox)target;
				return;
			case 2:
				this.tbError = (TextBlock)target;
				return;
			case 3:
				this.btnCancel = (Button)target;
				this.btnCancel.Click += this.Cancel_Click;
				return;
			case 4:
				this.btnOK = (Button)target;
				this.btnOK.Click += this.OK_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000BC6 RID: 3014
		private DocumentWrapper Pdf;

		// Token: 0x04000BC7 RID: 3015
		internal PasswordBox passwordBox;

		// Token: 0x04000BC8 RID: 3016
		internal TextBlock tbError;

		// Token: 0x04000BC9 RID: 3017
		internal Button btnCancel;

		// Token: 0x04000BCA RID: 3018
		internal Button btnOK;

		// Token: 0x04000BCB RID: 3019
		private bool _contentLoaded;
	}
}
