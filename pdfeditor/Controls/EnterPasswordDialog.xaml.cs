using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace pdfeditor.Controls
{
	// Token: 0x020001B6 RID: 438
	public partial class EnterPasswordDialog : Window
	{
		// Token: 0x060018EE RID: 6382 RVA: 0x000605B3 File Offset: 0x0005E7B3
		public EnterPasswordDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x000605C4 File Offset: 0x0005E7C4
		public EnterPasswordDialog(string fileName)
			: this()
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				string title = base.Title;
				base.Title = title + " - " + fileName;
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x060018F0 RID: 6384 RVA: 0x000605F8 File Offset: 0x0005E7F8
		// (set) Token: 0x060018F1 RID: 6385 RVA: 0x00060600 File Offset: 0x0005E800
		public string Password { get; private set; }

		// Token: 0x060018F2 RID: 6386 RVA: 0x0006060C File Offset: 0x0005E80C
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordTextBox.Text = password;
			}
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x00060644 File Offset: 0x0005E844
		private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string password = this.PasswordBox.Password;
			if (this.PasswordTextBox.Text != password)
			{
				this.PasswordBox.Password = this.PasswordTextBox.Text;
			}
			this.OkBtn.IsEnabled = !string.IsNullOrEmpty(password);
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0006069A File Offset: 0x0005E89A
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x000606A2 File Offset: 0x0005E8A2
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.Password = this.PasswordTextBox.Text;
			base.DialogResult = new bool?(true);
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x000606C1 File Offset: 0x0005E8C1
		private void ShowPwdBth_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				((Button)sender).CaptureMouse();
				this.PasswordBox.Visibility = Visibility.Collapsed;
				this.PasswordTextBox.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x000606F0 File Offset: 0x0005E8F0
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
