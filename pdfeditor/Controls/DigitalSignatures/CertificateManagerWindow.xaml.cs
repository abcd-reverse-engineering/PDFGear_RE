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
using System.Windows.Interop;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using pdfeditor.ViewModels;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000280 RID: 640
	public partial class CertificateManagerWindow : Window
	{
		// Token: 0x060024F3 RID: 9459 RVA: 0x000AB874 File Offset: 0x000A9A74
		private static IEnumerable<CertificateManagerWindow.CertModel> GetSignatureCertModels()
		{
			IEnumerable<CertificateManagerWindow.CertModel> enumerable = from c in CertificateManager.SignatureCertificateStorage.GetAllCertificates()
				select new CertificateManagerWindow.CertModel(c.Certificate, CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore);
			return enumerable.Concat(from c in CertificateStore.CreateUserStore().Certificates
				where SignatureValidateHelper.IsSupportedSignCertificate(c)
				select new CertificateManagerWindow.CertModel(c, CertificateManagerWindow.CertificateStorageMechanism.WindowsCertificateStore));
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x000AB90C File Offset: 0x000A9B0C
		private static IEnumerable<CertificateManagerWindow.CertModel> GetVerificationCertModels()
		{
			IEnumerable<CertificateManagerWindow.CertModel> enumerable = from c in CertificateManager.VerificationCertificateStorage.GetAllCertificates()
				select new CertificateManagerWindow.CertModel(c.Certificate, CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore);
			return enumerable.Concat(from c in CertificateStore.CreateRootStore().Certificates
				where SignatureValidateHelper.IsSupportedValidCertificate(c)
				select new CertificateManagerWindow.CertModel(c, CertificateManagerWindow.CertificateStorageMechanism.WindowsCertificateStore)).Concat(from c in CertificateStore.CreateUserStore().Certificates
				where SignatureValidateHelper.IsSupportedValidCertificate(c)
				select new CertificateManagerWindow.CertModel(c, CertificateManagerWindow.CertificateStorageMechanism.WindowsCertificateStore));
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x000AB9F9 File Offset: 0x000A9BF9
		public static IEnumerable<CertificateManagerWindow.X509CertificateModel> GetSignatureCertificate()
		{
			return from c in CertificateManagerWindow.GetSignatureCertModels()
				select c.ToX509CertificateModel();
		}

		// Token: 0x060024F6 RID: 9462 RVA: 0x000ABA24 File Offset: 0x000A9C24
		public static IEnumerable<CertificateManagerWindow.X509CertificateModel> GetVerificationCertificate()
		{
			return from c in CertificateManagerWindow.GetVerificationCertModels()
				select c.ToX509CertificateModel();
		}

		// Token: 0x060024F7 RID: 9463 RVA: 0x000ABA4F File Offset: 0x000A9C4F
		public CertificateManagerWindow(CertificateManagerWindow.DefaultSelectedTab defaultSelectedTab = CertificateManagerWindow.DefaultSelectedTab.SignatureCertificates)
		{
			this.InitializeComponent();
			this.Signature_RefreshButton_Click(null, null);
			this.Verification_RefreshButton_Click(null, null);
			if (defaultSelectedTab == CertificateManagerWindow.DefaultSelectedTab.SignatureCertificates)
			{
				this.mainTabControl.SelectedIndex = 0;
				return;
			}
			if (defaultSelectedTab == CertificateManagerWindow.DefaultSelectedTab.VerificationCertificates)
			{
				this.mainTabControl.SelectedIndex = 1;
			}
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x000ABA90 File Offset: 0x000A9C90
		private void Signature_AddButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "AddBtn", 1L);
			AddCertificateWindow addCertificateWindow = new AddCertificateWindow();
			addCertificateWindow.Owner = this;
			addCertificateWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			bool? flag = addCertificateWindow.ShowDialog();
			if (flag != null && flag.GetValueOrDefault())
			{
				CertificateManagerWindow.<>c__DisplayClass6_0 CS$<>8__locals1 = new CertificateManagerWindow.<>c__DisplayClass6_0();
				this.Signature_RefreshButton_Click(null, null);
				CertificateManagerWindow.<>c__DisplayClass6_0 CS$<>8__locals2 = CS$<>8__locals1;
				X509Certificate2 certificate = addCertificateWindow.Certificate;
				CS$<>8__locals2.thumbprint = ((certificate != null) ? certificate.Thumbprint : null);
				if (!string.IsNullOrEmpty(CS$<>8__locals1.thumbprint))
				{
					try
					{
						CertificateManagerWindow.CertModel certModel = this.Signature_ListView.ItemsSource.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault((CertificateManagerWindow.CertModel c) => c.Certificate.Thumbprint == CS$<>8__locals1.thumbprint);
						if (certModel != null)
						{
							this.Signature_ListView.SelectedItem = certModel;
							this.Signature_ListView.ScrollIntoView(certModel);
						}
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x060024F9 RID: 9465 RVA: 0x000ABB68 File Offset: 0x000A9D68
		private void Signature_DetailsButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "DetailBtn", 1L);
			CertificateManagerWindow.CertModel certModel = this.Signature_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault<CertificateManagerWindow.CertModel>();
			X509Certificate2 x509Certificate = ((certModel != null) ? certModel.Certificate : null);
			HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (x509Certificate != null && hwndSource != null && hwndSource.Handle != IntPtr.Zero)
			{
				using (X509Certificate2 x509Certificate2 = new X509Certificate2(x509Certificate.Export(X509ContentType.Cert)))
				{
					x509Certificate2.ShowCertificateDialog(hwndSource.Handle, null);
				}
			}
		}

		// Token: 0x060024FA RID: 9466 RVA: 0x000ABC08 File Offset: 0x000A9E08
		private void Signature_ExportButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "ExportBtn", 1L);
			CertificateManagerWindow.CertModel certModel = this.Signature_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault<CertificateManagerWindow.CertModel>();
			if (certModel != null && this.ExportCert(certModel))
			{
				GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "ExportSucc", 1L);
			}
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x000ABC64 File Offset: 0x000A9E64
		private void Signature_RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			CertificateManagerWindow.CertModel[] array = this.Signature_ListView.ItemsSource as CertificateManagerWindow.CertModel[];
			this.Signature_ListView.UnselectAll();
			this.Signature_ListView.ItemsSource = CertificateManagerWindow.GetSignatureCertModels().ToArray<CertificateManagerWindow.CertModel>();
			this.UpdateSignatureButtons();
			if (array != null)
			{
				CertificateManagerWindow.CertModel[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Certificate.Dispose();
				}
			}
		}

		// Token: 0x060024FC RID: 9468 RVA: 0x000ABCC8 File Offset: 0x000A9EC8
		private void Signature_RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "RemoveBtn", 1L);
			CertificateManagerWindow.CertModel[] array = (from c in this.Signature_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>()
				where c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore
				select c).ToArray<CertificateManagerWindow.CertModel>();
			if (array.Length != 0 && ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_Remove_Message"), UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.No, null, false) == MessageBoxResult.Yes)
			{
				GAManager.SendEvent("DigitalSignatureMgmt", "DigitalIdMgmt", "RemoveSucc", 1L);
				for (int i = 0; i < array.Length; i++)
				{
					CertificateManager.SignatureCertificateStorage.DeleteCertificate(array[i].Certificate.Thumbprint);
				}
				this.Signature_RefreshButton_Click(null, null);
				Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
			}
		}

		// Token: 0x060024FD RID: 9469 RVA: 0x000ABDA4 File Offset: 0x000A9FA4
		private void Signature_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateSignatureButtons();
			if (this.Signature_ListView.SelectedItems.Count == 1)
			{
				this.Signature_CertDetail.Content = this.Signature_ListView.SelectedItem;
				this.Signature_CertDetail.Visibility = Visibility.Visible;
				return;
			}
			this.Signature_CertDetail.Content = null;
			this.Signature_CertDetail.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060024FE RID: 9470 RVA: 0x000ABE08 File Offset: 0x000AA008
		private void UpdateSignatureButtons()
		{
			this.Signature_AddButton.IsEnabled = true;
			this.Signature_RefreshButton.IsEnabled = true;
			this.Signature_DetailsButton.IsEnabled = false;
			this.Signature_ExportButton.IsEnabled = false;
			this.Signature_RemoveButton.IsEnabled = false;
			CertificateManagerWindow.CertModel[] array = this.Signature_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().ToArray<CertificateManagerWindow.CertModel>();
			if (array != null && array.Length != 0)
			{
				if (array.Length == 1)
				{
					this.Signature_DetailsButton.IsEnabled = true;
					if (!array[0].IsExpired)
					{
						this.Signature_ExportButton.IsEnabled = true;
					}
					if (array[0].CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore)
					{
						this.Signature_RemoveButton.IsEnabled = true;
						return;
					}
				}
				else if (array.Any((CertificateManagerWindow.CertModel c) => c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore))
				{
					this.Signature_RemoveButton.IsEnabled = true;
				}
			}
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x000ABEE0 File Offset: 0x000AA0E0
		private void Verification_AddButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "AddBtn", 1L);
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "*.cer|*.cer",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
			};
			bool? flag = openFileDialog.ShowDialog(this);
			if (flag != null && flag.GetValueOrDefault())
			{
				try
				{
					byte[] array = File.ReadAllBytes(openFileDialog.FileName);
					X509Certificate2 cert = new X509Certificate2(array);
					if (CertificateManager.VerificationCertificateStorage.SaveCertificate(array, null))
					{
						this.Verification_RefreshButton_Click(null, null);
						Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
						GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "AddSucc", 1L);
					}
					CertificateManagerWindow.CertModel certModel = this.Verification_ListView.ItemsSource.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault((CertificateManagerWindow.CertModel c) => c.Certificate.Thumbprint == cert.Thumbprint);
					if (certModel != null)
					{
						this.Verification_ListView.SelectedItem = certModel;
						this.Verification_ListView.ScrollIntoView(certModel);
					}
				}
				catch
				{
					GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "AddFail", 1L);
					ModernMessageBox.Show(this, pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_AddFile_Invalid_Message"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
			}
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x000AC02C File Offset: 0x000AA22C
		private void Verification_DetailsButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "DetailBtn", 1L);
			CertificateManagerWindow.CertModel certModel = this.Verification_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault<CertificateManagerWindow.CertModel>();
			X509Certificate2 x509Certificate = ((certModel != null) ? certModel.Certificate : null);
			HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (x509Certificate != null && hwndSource != null && hwndSource.Handle != IntPtr.Zero)
			{
				if (x509Certificate.HasPrivateKey)
				{
					using (X509Certificate2 x509Certificate2 = new X509Certificate2(x509Certificate.Export(X509ContentType.Cert)))
					{
						x509Certificate2.ShowCertificateDialog(hwndSource.Handle, null);
						return;
					}
				}
				x509Certificate.ShowCertificateDialog(hwndSource.Handle, null);
			}
		}

		// Token: 0x06002501 RID: 9473 RVA: 0x000AC0E4 File Offset: 0x000AA2E4
		private void Verification_ExportButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "ExportBtn", 1L);
			CertificateManagerWindow.CertModel certModel = this.Verification_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().FirstOrDefault<CertificateManagerWindow.CertModel>();
			if (certModel != null && this.ExportCert(certModel))
			{
				GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "ExportSucc", 1L);
			}
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x000AC140 File Offset: 0x000AA340
		private void Verification_RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			CertificateManagerWindow.CertModel[] array = this.Verification_ListView.ItemsSource as CertificateManagerWindow.CertModel[];
			this.Verification_ListView.UnselectAll();
			this.Verification_ListView.ItemsSource = CertificateManagerWindow.GetVerificationCertModels().ToArray<CertificateManagerWindow.CertModel>();
			this.UpdateVerificationButtons();
			if (array != null)
			{
				CertificateManagerWindow.CertModel[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Certificate.Dispose();
				}
			}
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x000AC1A4 File Offset: 0x000AA3A4
		private void Verification_RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignatureMgmt", "TrustCertificateMgmt", "RemoveBtn", 1L);
			CertificateManagerWindow.CertModel[] array = (from c in this.Verification_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>()
				where c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore
				select c).ToArray<CertificateManagerWindow.CertModel>();
			if (array.Length != 0 && ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_Remove_Message"), UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.No, null, false) == MessageBoxResult.Yes)
			{
				for (int i = 0; i < array.Length; i++)
				{
					CertificateManager.VerificationCertificateStorage.DeleteCertificate(array[i].Certificate.Thumbprint);
				}
				this.Verification_RefreshButton_Click(null, null);
				Ioc.Default.GetRequiredService<MainViewModel>().DocumentWrapper.ReloadDigitalSignatureHelper();
			}
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x000AC268 File Offset: 0x000AA468
		private void Verification_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateVerificationButtons();
			if (this.Verification_ListView.SelectedItems.Count == 1)
			{
				this.Verification_CertDetail.Content = this.Verification_ListView.SelectedItem;
				this.Verification_CertDetail.Visibility = Visibility.Visible;
				return;
			}
			this.Verification_CertDetail.Content = null;
			this.Verification_CertDetail.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x000AC2CC File Offset: 0x000AA4CC
		private void UpdateVerificationButtons()
		{
			this.Verification_AddButton.IsEnabled = true;
			this.Verification_RefreshButton.IsEnabled = true;
			this.Verification_DetailsButton.IsEnabled = false;
			this.Verification_ExportButton.IsEnabled = false;
			this.Verification_RemoveButton.IsEnabled = false;
			CertificateManagerWindow.CertModel[] array = this.Verification_ListView.SelectedItems.OfType<CertificateManagerWindow.CertModel>().ToArray<CertificateManagerWindow.CertModel>();
			if (array != null && array.Length != 0)
			{
				if (array.Length == 1)
				{
					this.Verification_DetailsButton.IsEnabled = true;
					if (!array[0].IsExpired)
					{
						this.Verification_ExportButton.IsEnabled = true;
					}
					if (array[0].CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore)
					{
						this.Verification_RemoveButton.IsEnabled = true;
						return;
					}
				}
				else if (array.Any((CertificateManagerWindow.CertModel c) => c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore))
				{
					this.Verification_RemoveButton.IsEnabled = true;
				}
			}
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x000AC3A4 File Offset: 0x000AA5A4
		private bool ExportCert(CertificateManagerWindow.CertModel certModel)
		{
			if (certModel == null)
			{
				return false;
			}
			try
			{
				byte[] array = certModel.Certificate.Export(X509ContentType.Cert);
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
					Filter = "*.cer|*.cer"
				};
				if (!(saveFileDialog.ShowDialog(this) ?? false))
				{
					return false;
				}
				string fileName = saveFileDialog.FileName;
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				File.WriteAllBytes(fileName, array);
				ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_ExportSucceeded_Message"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return true;
			}
			catch
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_ExportFailed_Message"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			return false;
		}

		// Token: 0x0200072B RID: 1835
		public enum DefaultSelectedTab
		{
			// Token: 0x04002471 RID: 9329
			SignatureCertificates,
			// Token: 0x04002472 RID: 9330
			VerificationCertificates
		}

		// Token: 0x0200072C RID: 1836
		public class X509CertificateModel
		{
			// Token: 0x06003607 RID: 13831 RVA: 0x0010FEF8 File Offset: 0x0010E0F8
			public X509CertificateModel(X509Certificate2 certificate, CertificateManagerWindow.CertificateStorageMechanism certificateStorageMechanism)
			{
				this.Certificate = certificate;
				this.CertificateStorageMechanism = certificateStorageMechanism;
			}

			// Token: 0x17000D64 RID: 3428
			// (get) Token: 0x06003608 RID: 13832 RVA: 0x0010FF0E File Offset: 0x0010E10E
			public X509Certificate2 Certificate { get; }

			// Token: 0x17000D65 RID: 3429
			// (get) Token: 0x06003609 RID: 13833 RVA: 0x0010FF16 File Offset: 0x0010E116
			public CertificateManagerWindow.CertificateStorageMechanism CertificateStorageMechanism { get; }
		}

		// Token: 0x0200072D RID: 1837
		private class CertModel
		{
			// Token: 0x0600360A RID: 13834 RVA: 0x0010FF1E File Offset: 0x0010E11E
			public CertModel(X509Certificate2 certificate, CertificateManagerWindow.CertificateStorageMechanism certificateStorageMechanism)
			{
				this.Certificate = certificate;
				this.CertificateStorageMechanism = certificateStorageMechanism;
			}

			// Token: 0x17000D66 RID: 3430
			// (get) Token: 0x0600360B RID: 13835 RVA: 0x0010FF34 File Offset: 0x0010E134
			public X509Certificate2 Certificate { get; }

			// Token: 0x17000D67 RID: 3431
			// (get) Token: 0x0600360C RID: 13836 RVA: 0x0010FF3C File Offset: 0x0010E13C
			public CertificateManagerWindow.CertificateName Subject
			{
				get
				{
					CertificateManagerWindow.CertificateName certificateName;
					if ((certificateName = this.subject) == null)
					{
						certificateName = (this.subject = new CertificateManagerWindow.CertificateName(this.Certificate.Subject));
					}
					return certificateName;
				}
			}

			// Token: 0x17000D68 RID: 3432
			// (get) Token: 0x0600360D RID: 13837 RVA: 0x0010FF6C File Offset: 0x0010E16C
			public CertificateManagerWindow.CertificateName Issuer
			{
				get
				{
					CertificateManagerWindow.CertificateName certificateName;
					if ((certificateName = this.issuer) == null)
					{
						certificateName = (this.issuer = new CertificateManagerWindow.CertificateName(this.Certificate.Issuer));
					}
					return certificateName;
				}
			}

			// Token: 0x17000D69 RID: 3433
			// (get) Token: 0x0600360E RID: 13838 RVA: 0x0010FF9C File Offset: 0x0010E19C
			public CertificateManagerWindow.CertificateStorageMechanism CertificateStorageMechanism { get; }

			// Token: 0x17000D6A RID: 3434
			// (get) Token: 0x0600360F RID: 13839 RVA: 0x0010FFA4 File Offset: 0x0010E1A4
			public DateTimeOffset NotBefore
			{
				get
				{
					return this.Certificate.NotBefore;
				}
			}

			// Token: 0x17000D6B RID: 3435
			// (get) Token: 0x06003610 RID: 13840 RVA: 0x0010FFB6 File Offset: 0x0010E1B6
			public DateTimeOffset NotAfter
			{
				get
				{
					return this.Certificate.NotAfter;
				}
			}

			// Token: 0x17000D6C RID: 3436
			// (get) Token: 0x06003611 RID: 13841 RVA: 0x0010FFC8 File Offset: 0x0010E1C8
			public bool IsExpired
			{
				get
				{
					DateTimeOffset now = DateTimeOffset.Now;
					return now < this.NotBefore || now > this.NotAfter;
				}
			}

			// Token: 0x17000D6D RID: 3437
			// (get) Token: 0x06003612 RID: 13842 RVA: 0x0010FFF8 File Offset: 0x0010E1F8
			public string StorageMechanismDisplayName
			{
				get
				{
					CertificateManagerWindow.CertificateStorageMechanism certificateStorageMechanism = this.CertificateStorageMechanism;
					if (certificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore)
					{
						return pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_Storage_LocalCertificateStore");
					}
					if (certificateStorageMechanism != CertificateManagerWindow.CertificateStorageMechanism.WindowsCertificateStore)
					{
						return "";
					}
					return pdfeditor.Properties.Resources.ResourceManager.GetString("CertificateManager_Storage_WindowsCertificateStore");
				}
			}

			// Token: 0x06003613 RID: 13843 RVA: 0x0011003A File Offset: 0x0010E23A
			internal CertificateManagerWindow.X509CertificateModel ToX509CertificateModel()
			{
				return new CertificateManagerWindow.X509CertificateModel(this.Certificate, this.CertificateStorageMechanism);
			}

			// Token: 0x04002475 RID: 9333
			private CertificateManagerWindow.CertificateName subject;

			// Token: 0x04002476 RID: 9334
			private CertificateManagerWindow.CertificateName issuer;
		}

		// Token: 0x0200072E RID: 1838
		public enum CertificateStorageMechanism
		{
			// Token: 0x0400247A RID: 9338
			LocalCertificateStore,
			// Token: 0x0400247B RID: 9339
			WindowsCertificateStore
		}

		// Token: 0x0200072F RID: 1839
		public class CertificateName
		{
			// Token: 0x06003614 RID: 13844 RVA: 0x00110050 File Offset: 0x0010E250
			public CertificateName(string name)
			{
				this.name = name;
				this.dict = (from c in name.Split(new char[] { ',' })
					select c.Trim().Split(new char[] { '=' }) into c
					where c.Length == 2
					group c by c[0].ToUpperInvariant()).ToDictionary((IGrouping<string, string[]> c) => c.Key, (IGrouping<string, string[]> c) => c.FirstOrDefault<string[]>()[1]);
			}

			// Token: 0x17000D6E RID: 3438
			// (get) Token: 0x06003615 RID: 13845 RVA: 0x00110130 File Offset: 0x0010E330
			public string CommonName
			{
				get
				{
					return this.GetValue("CN");
				}
			}

			// Token: 0x17000D6F RID: 3439
			// (get) Token: 0x06003616 RID: 13846 RVA: 0x0011013D File Offset: 0x0010E33D
			public string Email
			{
				get
				{
					return this.GetValue("E");
				}
			}

			// Token: 0x17000D70 RID: 3440
			// (get) Token: 0x06003617 RID: 13847 RVA: 0x0011014A File Offset: 0x0010E34A
			public string OrganizationUnit
			{
				get
				{
					return this.GetValue("OU");
				}
			}

			// Token: 0x17000D71 RID: 3441
			// (get) Token: 0x06003618 RID: 13848 RVA: 0x00110157 File Offset: 0x0010E357
			public string OrganizationName
			{
				get
				{
					return this.GetValue("O");
				}
			}

			// Token: 0x17000D72 RID: 3442
			// (get) Token: 0x06003619 RID: 13849 RVA: 0x00110164 File Offset: 0x0010E364
			public string LocalityName
			{
				get
				{
					return this.GetValue("L");
				}
			}

			// Token: 0x17000D73 RID: 3443
			// (get) Token: 0x0600361A RID: 13850 RVA: 0x00110171 File Offset: 0x0010E371
			public string StateName
			{
				get
				{
					return this.GetValue("S");
				}
			}

			// Token: 0x17000D74 RID: 3444
			// (get) Token: 0x0600361B RID: 13851 RVA: 0x0011017E File Offset: 0x0010E37E
			public string Country
			{
				get
				{
					return this.GetValue("C");
				}
			}

			// Token: 0x17000D75 RID: 3445
			// (get) Token: 0x0600361C RID: 13852 RVA: 0x0011018C File Offset: 0x0010E38C
			public string DisplayName
			{
				get
				{
					string text = "";
					if (!string.IsNullOrEmpty(this.CommonName))
					{
						text = this.CommonName;
					}
					else if (!string.IsNullOrEmpty(this.OrganizationUnit))
					{
						text = this.OrganizationUnit;
					}
					else if (!string.IsNullOrEmpty(this.OrganizationName))
					{
						text = this.OrganizationName;
					}
					if (!string.IsNullOrEmpty(this.Email) && !string.IsNullOrEmpty(text))
					{
						return text + " <" + this.Email + ">";
					}
					if (!string.IsNullOrEmpty(this.Email))
					{
						return this.Email;
					}
					return text;
				}
			}

			// Token: 0x0600361D RID: 13853 RVA: 0x00110220 File Offset: 0x0010E420
			private string GetValue(string name)
			{
				string text;
				if (this.dict.TryGetValue(name, out text))
				{
					return text.Trim(new char[] { '"' }).Trim();
				}
				return "";
			}

			// Token: 0x0600361E RID: 13854 RVA: 0x00110259 File Offset: 0x0010E459
			public override string ToString()
			{
				return this.name;
			}

			// Token: 0x0400247C RID: 9340
			private readonly string name;

			// Token: 0x0400247D RID: 9341
			private readonly IReadOnlyDictionary<string, string> dict;
		}
	}
}
