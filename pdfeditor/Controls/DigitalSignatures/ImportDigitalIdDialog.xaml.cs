using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using Microsoft.Win32;
using pdfeditor.Properties;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000287 RID: 647
	public partial class ImportDigitalIdDialog : Window
	{
		// Token: 0x06002552 RID: 9554 RVA: 0x000ADA09 File Offset: 0x000ABC09
		public ImportDigitalIdDialog(string fileName)
		{
			this.InitializeComponent();
			this.FilePathTextBox.Text = fileName ?? "";
		}

		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x06002553 RID: 9555 RVA: 0x000ADA2C File Offset: 0x000ABC2C
		// (set) Token: 0x06002554 RID: 9556 RVA: 0x000ADA34 File Offset: 0x000ABC34
		public X509Certificate2 Certificate { get; private set; }

		// Token: 0x06002555 RID: 9557 RVA: 0x000ADA40 File Offset: 0x000ABC40
		private void BrowserButton_Click(object sender, RoutedEventArgs e)
		{
			string text;
			if (ImportDigitalIdDialog.BrowserFile(this, out text))
			{
				this.FilePathTextBox.Text = text;
				this.UpdateOkBtnEnabled();
			}
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x000ADA69 File Offset: 0x000ABC69
		private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateOkBtnEnabled();
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x000ADA74 File Offset: 0x000ABC74
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordTextBox.Text = password;
			}
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x000ADAAC File Offset: 0x000ABCAC
		private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordBox.Password = this.PasswordTextBox.Text;
			}
			if (string.IsNullOrEmpty(this.PasswordTextBox.Text))
			{
				this.Placeholder.Visibility = Visibility.Visible;
			}
			else
			{
				this.Placeholder.Visibility = Visibility.Collapsed;
			}
			this.UpdateOkBtnEnabled();
		}

		// Token: 0x06002559 RID: 9561 RVA: 0x000ADB20 File Offset: 0x000ABD20
		private void ShowPwdBth_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				((Button)sender).CaptureMouse();
				this.PasswordBox.Visibility = Visibility.Collapsed;
				this.PasswordTextBox.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x000ADB4F File Offset: 0x000ABD4F
		private void ShowPwdBth_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
			{
				((Button)sender).ReleaseMouseCapture();
				this.PasswordBox.Visibility = Visibility.Visible;
				this.PasswordTextBox.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x0600255B RID: 9563 RVA: 0x000ADB7C File Offset: 0x000ABD7C
		public static bool BrowserFile(Window ownerWindow, out string fileName)
		{
			fileName = null;
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "*.pfx,*.p12|*.pfx;*.p12",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
			};
			bool? flag = openFileDialog.ShowDialog(ownerWindow);
			if (flag.GetValueOrDefault())
			{
				fileName = openFileDialog.FileName;
			}
			return flag.GetValueOrDefault();
		}

		// Token: 0x0600255C RID: 9564 RVA: 0x000ADBC9 File Offset: 0x000ABDC9
		private void UpdateOkBtnEnabled()
		{
			this.btnOk.IsEnabled = !string.IsNullOrEmpty(this.FilePathTextBox.Text);
		}

		// Token: 0x0600255D RID: 9565 RVA: 0x000ADBEC File Offset: 0x000ABDEC
		private X509Certificate2 OpenCertFile(string file)
		{
			try
			{
				byte[] array = File.ReadAllBytes(file);
				string password = this.PasswordBox.Password;
				try
				{
					return new X509Certificate2(array, password);
				}
				catch
				{
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x000ADC40 File Offset: 0x000ABE40
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			this.Certificate = this.OpenCertFile(this.FilePathTextBox.Text);
			if (this.Certificate != null)
			{
				base.DialogResult = new bool?(true);
				return;
			}
			ModernMessageBox.Show(this, pdfeditor.Properties.Resources.CreateDigSignImportCertErrorMessage_InvalidPassword, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
		}
	}
}
