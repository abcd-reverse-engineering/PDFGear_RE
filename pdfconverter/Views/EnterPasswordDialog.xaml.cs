using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using pdfconverter.Properties;

namespace pdfconverter.Views
{
	// Token: 0x02000026 RID: 38
	public partial class EnterPasswordDialog : Window
	{
		// Token: 0x06000207 RID: 519 RVA: 0x00007913 File Offset: 0x00005B13
		public EnterPasswordDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00007921 File Offset: 0x00005B21
		public EnterPasswordDialog(string fileName)
			: this()
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				this.PwdTip.Text = pdfconverter.Properties.Resources.WinPwdEnterTipContentFa.Replace("XXX", fileName);
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000794C File Offset: 0x00005B4C
		// (set) Token: 0x0600020A RID: 522 RVA: 0x00007954 File Offset: 0x00005B54
		public string Password { get; private set; }

		// Token: 0x0600020B RID: 523 RVA: 0x00007960 File Offset: 0x00005B60
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordTextBox.Text = password;
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00007998 File Offset: 0x00005B98
		private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordBox.Password = this.PasswordTextBox.Text;
			}
			this.OkBtn.IsEnabled = !string.IsNullOrEmpty(password);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x000079EE File Offset: 0x00005BEE
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x000079F6 File Offset: 0x00005BF6
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.Password = this.PasswordTextBox.Text;
			base.DialogResult = new bool?(true);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00007A15 File Offset: 0x00005C15
		private void ShowPwdBth_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				((Button)sender).CaptureMouse();
				this.PasswordBox.Visibility = Visibility.Collapsed;
				this.PasswordTextBox.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00007A44 File Offset: 0x00005C44
		private void ShowPwdBth_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
			{
				((Button)sender).ReleaseMouseCapture();
				this.PasswordBox.Visibility = Visibility.Visible;
				this.PasswordTextBox.Visibility = Visibility.Collapsed;
			}
		}
	}
}
