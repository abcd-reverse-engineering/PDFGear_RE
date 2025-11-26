using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models;
using pdfeditor.Properties;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Protection
{
	// Token: 0x02000221 RID: 545
	public class EncryptWindow : Window, IComponentConnector
	{
		// Token: 0x06001E81 RID: 7809 RVA: 0x000876BC File Offset: 0x000858BC
		public EncryptWindow(DocumentWrapper pdf)
		{
			this.InitializeComponent();
			this.Pdf = pdf;
			if (!string.IsNullOrWhiteSpace(this.Pdf.EncryptManage.UserPassword))
			{
				this.tbOpenpwd.Password = this.Pdf.EncryptManage.UserPassword;
				this.tbOpenpwdConfirm.Password = this.Pdf.EncryptManage.UserPassword;
			}
			else
			{
				this.tbOpenpwd.Password = this.Pdf.EncryptManage.OwerPassword;
				this.tbOpenpwdConfirm.Password = this.Pdf.EncryptManage.OwerPassword;
			}
			if (!this.Pdf.EncryptManage.IsHaveOwerPassword || pdf.EncryptManage.IsChangedPassword)
			{
				base.Height -= 32.0;
				this.ckboxRetainpwd.Visibility = Visibility.Collapsed;
				this.cboxGridRow.Height = new GridLength(0.0);
			}
			if (this.Pdf.EncryptManage.IsRequiredOwerPassword && this.Pdf.EncryptManage.IsHaveOwerPassword)
			{
				this.ckboxRetainpwd.IsChecked = new bool?(false);
			}
			GAManager.SendEvent("Password", "AddPasswordShow", "Count", 1L);
		}

		// Token: 0x06001E82 RID: 7810 RVA: 0x00087808 File Offset: 0x00085A08
		private void Encrypt_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.VarifyOpenpwd())
				{
					if (this.Pdf != null)
					{
						string text = ((this.ckboxRetainpwd.IsChecked.GetValueOrDefault() && this.ckboxRetainpwd.Visibility == Visibility.Visible) ? this.Pdf.EncryptManage.OwerPassword : this.tbOpenpwdConfirm.Password.Trim());
						DocumentWrapper pdf = this.Pdf;
						if (pdf != null)
						{
							pdf.EncryptManage.SetPassword(this.tbOpenpwdConfirm.Password.Trim(), text);
						}
						Ioc.Default.GetRequiredService<MainViewModel>().SetCanSaveFlag("Password", false);
						if (!ConfigManager.GetPasswordSaveNoMorePromptFlag())
						{
							new PasswordSaveTipWindow().ShowDialog();
						}
						GAManager.SendEvent("Password", "AddPassword", "Count", 1L);
						base.Close();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x000878F4 File Offset: 0x00085AF4
		private bool VarifyOpenpwd()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(this.tbOpenpwd.Password))
				{
					this.tbpasswordNotMatch.Visibility = Visibility.Visible;
					this.tbpasswordNotMatch.Text = pdfeditor.Properties.Resources.WinPwdPasswordCheckEmptyContent;
					return false;
				}
				if (this.tbOpenpwd.Password != this.tbOpenpwdConfirm.Password)
				{
					this.tbpasswordNotMatch.Visibility = Visibility.Visible;
					this.tbpasswordNotMatch.Text = pdfeditor.Properties.Resources.WinPwdPasswordCheckMatchContent;
					return false;
				}
				if (this.tbOpenpwd.Password.Length < 6)
				{
					this.tbpasswordNotMatch.Visibility = Visibility.Visible;
					this.tbpasswordNotMatch.Text = pdfeditor.Properties.Resources.WinPwdMinCharacterCheckContent;
					return false;
				}
				if (this.tbOpenpwd.Password.Length > 32)
				{
					this.tbpasswordNotMatch.Visibility = Visibility.Visible;
					this.tbpasswordNotMatch.Text = pdfeditor.Properties.Resources.WinPwdMaxCharacterCheckContent;
					return false;
				}
				if (!this.VerifyIsANSIString(this.tbOpenpwd.Password))
				{
					this.tbpasswordNotMatch.Visibility = Visibility.Visible;
					this.tbpasswordNotMatch.Text = pdfeditor.Properties.Resources.WinPwdillegalsymbolsCheckContent;
					return false;
				}
			}
			catch
			{
				return false;
			}
			this.tbpasswordNotMatch.Visibility = Visibility.Hidden;
			return true;
		}

		// Token: 0x06001E84 RID: 7812 RVA: 0x00087A44 File Offset: 0x00085C44
		private bool VerifyIsANSIString(string pwd)
		{
			bool flag = true;
			foreach (char c in pwd)
			{
				if (c < '!' || c > '~')
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x00087A7D File Offset: 0x00085C7D
		private void Cacel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
			base.Close();
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x00087A94 File Offset: 0x00085C94
		private void ckboxRetainpwd_Checked(object sender, RoutedEventArgs e)
		{
			if (this.Pdf == null)
			{
				return;
			}
			if (!this.Pdf.EncryptManage.IsRequiredOwerPassword)
			{
				return;
			}
			bool? flag = new OwerPasswordCheckWindow(this.Pdf).ShowDialog();
			bool flag2 = false;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				this.ckboxRetainpwd.IsChecked = new bool?(false);
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x00087AF5 File Offset: 0x00085CF5
		private void ShowPwdBth_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				((Button)sender).CaptureMouse();
				this.tbOpenpwd.Visibility = Visibility.Collapsed;
				this.PasswordTextBox.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x00087B24 File Offset: 0x00085D24
		private void ShowPwdBth_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
			{
				((Button)sender).ReleaseMouseCapture();
				this.tbOpenpwd.Visibility = Visibility.Visible;
				this.PasswordTextBox.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x00087B54 File Offset: 0x00085D54
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			string password = this.tbOpenpwd.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordTextBox.Text = password;
			}
		}

		// Token: 0x06001E8A RID: 7818 RVA: 0x00087B8C File Offset: 0x00085D8C
		private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = this.tbOpenpwd.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.tbOpenpwd.Password = this.PasswordTextBox.Text;
			}
			this.btnOK.IsEnabled = !string.IsNullOrEmpty(password);
		}

		// Token: 0x06001E8B RID: 7819 RVA: 0x00087BE4 File Offset: 0x00085DE4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/pdfeditor;component/controls/protect/encryptwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001E8C RID: 7820 RVA: 0x00087C14 File Offset: 0x00085E14
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.window = (EncryptWindow)target;
				return;
			case 2:
				this.cboxGridRow = (RowDefinition)target;
				return;
			case 3:
				this.tbDocmentOpenText = (TextBlock)target;
				return;
			case 4:
				this.tbDocmentConfirmText = (TextBlock)target;
				return;
			case 5:
				this.tbOpenpwd = (PasswordBox)target;
				this.tbOpenpwd.PasswordChanged += this.PasswordBox_PasswordChanged;
				return;
			case 6:
				this.PasswordTextBox = (TextBox)target;
				this.PasswordTextBox.TextChanged += this.PasswordTextBox_TextChanged;
				return;
			case 7:
				this.ShowPwdBth = (Button)target;
				this.ShowPwdBth.PreviewMouseDown += this.ShowPwdBth_MouseDown;
				this.ShowPwdBth.PreviewMouseUp += this.ShowPwdBth_MouseUp;
				return;
			case 8:
				this.tbOpenpwdConfirm = (PasswordBox)target;
				return;
			case 9:
				this.ckboxRetainpwd = (CheckBox)target;
				this.ckboxRetainpwd.Checked += this.ckboxRetainpwd_Checked;
				return;
			case 10:
				this.tbpasswordNotMatch = (TextBlock)target;
				return;
			case 11:
				this.btnCancel = (Button)target;
				this.btnCancel.Click += this.Cacel_Click;
				return;
			case 12:
				this.btnOK = (Button)target;
				this.btnOK.Click += this.Encrypt_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000BB8 RID: 3000
		private DocumentWrapper Pdf;

		// Token: 0x04000BB9 RID: 3001
		internal EncryptWindow window;

		// Token: 0x04000BBA RID: 3002
		internal RowDefinition cboxGridRow;

		// Token: 0x04000BBB RID: 3003
		internal TextBlock tbDocmentOpenText;

		// Token: 0x04000BBC RID: 3004
		internal TextBlock tbDocmentConfirmText;

		// Token: 0x04000BBD RID: 3005
		internal PasswordBox tbOpenpwd;

		// Token: 0x04000BBE RID: 3006
		internal TextBox PasswordTextBox;

		// Token: 0x04000BBF RID: 3007
		internal Button ShowPwdBth;

		// Token: 0x04000BC0 RID: 3008
		internal PasswordBox tbOpenpwdConfirm;

		// Token: 0x04000BC1 RID: 3009
		internal CheckBox ckboxRetainpwd;

		// Token: 0x04000BC2 RID: 3010
		internal TextBlock tbpasswordNotMatch;

		// Token: 0x04000BC3 RID: 3011
		internal Button btnCancel;

		// Token: 0x04000BC4 RID: 3012
		internal Button btnOK;

		// Token: 0x04000BC5 RID: 3013
		private bool _contentLoaded;
	}
}
