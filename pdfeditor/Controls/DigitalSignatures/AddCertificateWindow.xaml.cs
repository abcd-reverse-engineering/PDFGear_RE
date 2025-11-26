using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x0200027F RID: 639
	public partial class AddCertificateWindow : Window
	{
		// Token: 0x060024E1 RID: 9441 RVA: 0x000AA8D8 File Offset: 0x000A8AD8
		public AddCertificateWindow()
		{
			this.InitializeComponent();
			List<AddCertificateWindow.ISO_3166_ID_SHORT_Model> list = new List<AddCertificateWindow.ISO_3166_ID_SHORT_Model>
			{
				new AddCertificateWindow.ISO_3166_ID_SHORT_Model()
			};
			list.AddRange(AddCertificateWindow.ISO_3166_ID_SHORT_Models);
			this.CreateNew_CountryOrRegion.ItemsSource = list;
			this.CreateNew_CountryOrRegion.SelectedIndex = 0;
			this.UpdateStep();
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x060024E2 RID: 9442 RVA: 0x000AA941 File Offset: 0x000A8B41
		// (set) Token: 0x060024E3 RID: 9443 RVA: 0x000AA949 File Offset: 0x000A8B49
		public X509Certificate2 Certificate { get; private set; }

		// Token: 0x060024E4 RID: 9444 RVA: 0x000AA954 File Offset: 0x000A8B54
		private void CommandButton_Back_Click(object sender, RoutedEventArgs e)
		{
			if (this.step == "StepContainer_CreateNew" || this.step == "StepContainer_ImportFile")
			{
				this.step = "StepContainer_Init";
			}
			else if (this.step == "StepContainer_SaveFile")
			{
				this.step = "StepContainer_CreateNew";
			}
			this.UpdateStep();
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x000AA9B8 File Offset: 0x000A8BB8
		private void CommandButton_Next_Click(object sender, RoutedEventArgs e)
		{
			if (this.step == "StepContainer_Init")
			{
				bool? isChecked = this.SourceRadioButton_ExistingFile.IsChecked;
				if (isChecked != null && isChecked.GetValueOrDefault())
				{
					GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_ImportFromFile", "Init", 1L);
					this.step = "StepContainer_ImportFile";
				}
				else
				{
					GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_CreateNew", "Init", 1L);
					this.step = "StepContainer_CreateNew";
				}
			}
			else if (this.step == "StepContainer_CreateNew")
			{
				if (string.IsNullOrEmpty(this.CreateNew_Name.Text))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertProps_NameEmptyMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else if (!string.IsNullOrEmpty(this.CreateNew_Email.Text) && !UtilManager.IsEmailValid(this.CreateNew_Email.Text))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertProps_EmailInvalidMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_CreateNew", "CreateNewCertWindowShow", 1L);
					this.step = "StepContainer_SaveFile";
					string text = this.CreateNew_Name.Text;
					char[] array = new char[Math.Min(text.Length, 100)];
					char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
					for (int i = 0; i < array.Length; i++)
					{
						if (invalidFileNameChars.Contains(text[i]))
						{
							array[i] = '_';
						}
						else
						{
							array[i] = text[i];
						}
					}
					text = new string(array, 0, array.Length);
					if (string.IsNullOrEmpty(this.SaveFile_FilePathTextBox.Text))
					{
						string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						for (int j = 0; j < 2147483647; j++)
						{
							string text2;
							if (j == 0)
							{
								text2 = text + ".pfx";
							}
							else
							{
								text2 = string.Format("{0} ({1}).pfx", text, j);
							}
							string text3 = Path.Combine(folderPath, text2);
							if (!File.Exists(text3))
							{
								this.suggestFileName = text2;
								this.SaveFile_FilePathTextBox.Text = text3;
								break;
							}
						}
						if (string.IsNullOrEmpty(this.suggestFileName))
						{
							this.suggestFileName = Guid.NewGuid().ToString("N").Substring(0, 8) + ".pfx";
						}
					}
				}
			}
			else if (this.step == "StepContainer_SaveFile")
			{
				string text4 = "";
				try
				{
					text4 = Path.GetFullPath(this.SaveFile_FilePathTextBox.Text);
					Uri uri;
					if (!Uri.TryCreate(this.SaveFile_FilePathTextBox.Text, UriKind.Absolute, out uri))
					{
						text4 = "";
					}
				}
				catch
				{
				}
				if (string.IsNullOrEmpty(this.SaveFile_FilePathTextBox.Text))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertFile_FilenameEmptyMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else if (string.IsNullOrEmpty(text4))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertFile_FilenameInvalidMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else if (this.SaveFile_PasswordBox.Password.Length < 6 || this.SaveFile_PasswordBox.Password.Length > 32)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("WinPwdPasswordTooltipContent"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else if (this.SaveFile_PasswordBox.Password != this.SaveFile_ConfirmPasswordBox.Password)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("WinPwdPasswordCheckMatchContent"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					CertificateCreator.CertificateData certificateData = new CertificateCreator.CertificateData
					{
						Subject = this.CreateNew_Name.Text,
						OrganizationalUnit = this.CreateNew_OrganizationalUnit.Text,
						OrganizationalName = this.CreateNew_OrganizationalName.Text,
						Email = this.CreateNew_Email.Text
					};
					AddCertificateWindow.ISO_3166_ID_SHORT_Model iso_3166_ID_SHORT_Model = this.CreateNew_CountryOrRegion.SelectedItem as AddCertificateWindow.ISO_3166_ID_SHORT_Model;
					if (iso_3166_ID_SHORT_Model != null && !string.IsNullOrEmpty(iso_3166_ID_SHORT_Model.alpha2))
					{
						certificateData.CountryRegion = iso_3166_ID_SHORT_Model.alpha2;
					}
					try
					{
						X509Certificate2 x509Certificate = CertificateCreator.GenerateCertificate(certificateData);
						byte[] array2 = x509Certificate.Export(X509ContentType.Pfx, this.SaveFile_PasswordBox.Password);
						using (FileStream fileStream = File.OpenWrite(text4))
						{
							if (CertificateManager.SignatureCertificateStorage.SaveCertificate(array2, this.SaveFile_PasswordBox.Password))
							{
								fileStream.Write(array2, 0, array2.Length);
								this.Certificate = x509Certificate;
								GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_CreateNew", "SaveSucc", 1L);
								Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
								base.DialogResult = new bool?(true);
								return;
							}
						}
					}
					catch
					{
						GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_CreateNew", "SaveFail", 1L);
					}
					ModernMessageBox.Show(this, pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertFile_FailedMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
			}
			else if (this.step == "StepContainer_ImportFile")
			{
				try
				{
					string fullPath = Path.GetFullPath(this.ImportFile_FilePathTextBox.Text);
					if (!string.IsNullOrEmpty(fullPath))
					{
						byte[] array3 = File.ReadAllBytes(fullPath);
						string password = this.ImportFile_PasswordBox.Password;
						X509Certificate2 x509Certificate2 = new X509Certificate2(array3, password, X509KeyStorageFlags.Exportable);
						if (!SignatureValidateHelper.IsSupportedSignCertificate(x509Certificate2))
						{
							GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_ImportFromFile", "ImportFail2", 1L);
							ModernMessageBox.Show(pdfeditor.Properties.Resources.CreateDigSignImportCertErrorMessage_NotSupport, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						if (CertificateManager.SignatureCertificateStorage.SaveCertificate(array3, password))
						{
							this.Certificate = x509Certificate2;
							GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_ImportFromFile", "ImportSucc", 1L);
							Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
							base.DialogResult = new bool?(true);
							return;
						}
						GAManager.SendEvent("DigitalSignatureMgmt", "AddDigitalId_ImportFromFile", "ImportFail1", 1L);
					}
				}
				catch
				{
				}
				ModernMessageBox.Show(this, pdfeditor.Properties.Resources.CreateDigSignImportCertErrorMessage_InvalidPassword, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			this.UpdateStep();
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x000AB000 File Offset: 0x000A9200
		private void UpdateStep()
		{
			this.StepContainer_Init.Visibility = Visibility.Collapsed;
			this.StepContainer_CreateNew.Visibility = Visibility.Collapsed;
			this.StepContainer_SaveFile.Visibility = Visibility.Collapsed;
			this.StepContainer_ImportFile.Visibility = Visibility.Collapsed;
			if (string.IsNullOrEmpty(this.step))
			{
				this.step = "StepContainer_Init";
			}
			((FrameworkElement)base.FindName(this.step)).Visibility = Visibility.Visible;
			this.CommandButton_Back.Visibility = Visibility.Collapsed;
			this.CommandButton_Next.Visibility = Visibility.Collapsed;
			this.CommandButton_OK.Visibility = Visibility.Collapsed;
			if (this.step == "StepContainer_Init")
			{
				this.CommandButton_Next.Visibility = Visibility.Visible;
				return;
			}
			if (this.step == "StepContainer_CreateNew")
			{
				this.CommandButton_Back.Visibility = Visibility.Visible;
				this.CommandButton_Next.Visibility = Visibility.Visible;
				return;
			}
			if (this.step == "StepContainer_SaveFile")
			{
				this.CommandButton_Back.Visibility = Visibility.Visible;
				this.CommandButton_OK.Visibility = Visibility.Visible;
				return;
			}
			if (this.step == "StepContainer_ImportFile")
			{
				this.CommandButton_Back.Visibility = Visibility.Visible;
				this.CommandButton_OK.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x000AB12F File Offset: 0x000A932F
		private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateOkBtnEnabled();
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x000AB138 File Offset: 0x000A9338
		private void SaveFile_BrowserButton_Click(object sender, RoutedEventArgs e)
		{
			string text = "";
			string text2 = "";
			try
			{
				if (!string.IsNullOrEmpty(this.SaveFile_FilePathTextBox.Text))
				{
					text = Path.GetDirectoryName(this.SaveFile_FilePathTextBox.Text);
					text2 = Path.GetFileName(this.SaveFile_FilePathTextBox.Text);
				}
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = this.suggestFileName ?? "";
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "*.pfx|*.pfx|*.p12|*.p12",
				InitialDirectory = text,
				FileName = text2
			};
			if (saveFileDialog.ShowDialog(this).GetValueOrDefault())
			{
				this.SaveFile_FilePathTextBox.Text = saveFileDialog.FileName ?? "";
			}
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x000AB20C File Offset: 0x000A940C
		private void ImportFile_BrowserButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "*.pfx,*.p12|*.pfx;*.p12",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
			};
			if (openFileDialog.ShowDialog(this).GetValueOrDefault())
			{
				this.ImportFile_FilePathTextBox.Text = openFileDialog.FileName ?? "";
			}
		}

		// Token: 0x060024EA RID: 9450 RVA: 0x000AB261 File Offset: 0x000A9461
		private void UpdateOkBtnEnabled()
		{
		}

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x060024EB RID: 9451 RVA: 0x000AB264 File Offset: 0x000A9464
		private static IReadOnlyList<AddCertificateWindow.ISO_3166_ID_SHORT_Model> ISO_3166_ID_SHORT_Models
		{
			get
			{
				if (AddCertificateWindow._ISO_3166_ID_SHORT_Models == null)
				{
					try
					{
						using (Stream manifestResourceStream = typeof(AddCertificateWindow.ISO_3166_ID_SHORT_Model).Assembly.GetManifestResourceStream("pdfeditor.Utils.DigitalSignatures.ISO-3166-ID-SHORT.json"))
						{
							using (StreamReader streamReader = new StreamReader(manifestResourceStream))
							{
								using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
								{
									AddCertificateWindow._ISO_3166_ID_SHORT_Models = (from c in JToken.Load(jsonTextReader).ToObject<AddCertificateWindow.ISO_3166_ID_SHORT_Model[]>()
										orderby c.alpha2
										select c).ToArray<AddCertificateWindow.ISO_3166_ID_SHORT_Model>();
								}
							}
						}
					}
					catch
					{
						AddCertificateWindow._ISO_3166_ID_SHORT_Models = Array.Empty<AddCertificateWindow.ISO_3166_ID_SHORT_Model>();
					}
				}
				return AddCertificateWindow._ISO_3166_ID_SHORT_Models;
			}
		}

		// Token: 0x060024EC RID: 9452 RVA: 0x000AB348 File Offset: 0x000A9548
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox passwordBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<PasswordBox>().First<PasswordBox>();
			TextBox textBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<TextBox>().First<TextBox>();
			string password = passwordBox.Password;
			if (textBox.Text != password)
			{
				textBox.Text = password;
			}
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x000AB3B0 File Offset: 0x000A95B0
		private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			PasswordBox passwordBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<PasswordBox>().First<PasswordBox>();
			TextBox textBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<TextBox>().First<TextBox>();
			string password = passwordBox.Password;
			if (textBox.Text != password)
			{
				passwordBox.Password = textBox.Text;
			}
			this.UpdateOkBtnEnabled();
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x000AB424 File Offset: 0x000A9624
		private void ShowPwdBth_MouseDown(object sender, MouseButtonEventArgs e)
		{
			PasswordBox passwordBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<PasswordBox>().First<PasswordBox>();
			TextBox textBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<TextBox>().First<TextBox>();
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				((Button)sender).CaptureMouse();
				passwordBox.Visibility = Visibility.Collapsed;
				textBox.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x000AB494 File Offset: 0x000A9694
		private void ShowPwdBth_MouseUp(object sender, MouseButtonEventArgs e)
		{
			PasswordBox passwordBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<PasswordBox>().First<PasswordBox>();
			TextBox textBox = ((Grid)((FrameworkElement)sender).Parent).Children.OfType<TextBox>().First<TextBox>();
			if (e.LeftButton == MouseButtonState.Released)
			{
				((Button)sender).ReleaseMouseCapture();
				passwordBox.Visibility = Visibility.Visible;
				textBox.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x04000F96 RID: 3990
		private string step = "";

		// Token: 0x04000F97 RID: 3991
		private string suggestFileName = "";

		// Token: 0x04000F99 RID: 3993
		private static IReadOnlyList<AddCertificateWindow.ISO_3166_ID_SHORT_Model> _ISO_3166_ID_SHORT_Models;

		// Token: 0x02000729 RID: 1833
		private class ISO_3166_ID_SHORT_Model
		{
			// Token: 0x17000D60 RID: 3424
			// (get) Token: 0x060035FA RID: 13818 RVA: 0x0010FE31 File Offset: 0x0010E031
			// (set) Token: 0x060035FB RID: 13819 RVA: 0x0010FE39 File Offset: 0x0010E039
			public string alpha2 { get; set; }

			// Token: 0x17000D61 RID: 3425
			// (get) Token: 0x060035FC RID: 13820 RVA: 0x0010FE42 File Offset: 0x0010E042
			// (set) Token: 0x060035FD RID: 13821 RVA: 0x0010FE4A File Offset: 0x0010E04A
			public string alpha3 { get; set; }

			// Token: 0x17000D62 RID: 3426
			// (get) Token: 0x060035FE RID: 13822 RVA: 0x0010FE53 File Offset: 0x0010E053
			// (set) Token: 0x060035FF RID: 13823 RVA: 0x0010FE5B File Offset: 0x0010E05B
			public string name { get; set; }

			// Token: 0x17000D63 RID: 3427
			// (get) Token: 0x06003600 RID: 13824 RVA: 0x0010FE64 File Offset: 0x0010E064
			// (set) Token: 0x06003601 RID: 13825 RVA: 0x0010FE6C File Offset: 0x0010E06C
			public string num { get; set; }

			// Token: 0x06003602 RID: 13826 RVA: 0x0010FE78 File Offset: 0x0010E078
			public override string ToString()
			{
				if (string.IsNullOrEmpty(this.alpha2) || string.IsNullOrEmpty(this.name))
				{
					return "<" + pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertProps_CountryRegionNoneValue") + ">";
				}
				return this.alpha2 + " " + this.name;
			}
		}
	}
}
