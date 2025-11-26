using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls;
using Patagames.Pdf;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using PDFKit.Utils.DigitalSignatures;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000281 RID: 641
	public partial class CreateDigitalSignatureDialog : Window
	{
		// Token: 0x06002509 RID: 9481 RVA: 0x000AC7A4 File Offset: 0x000AA9A4
		public CreateDigitalSignatureDialog(PdfDigitalSignatureLocation pdfDigitalSignatureLocation, IReadOnlyList<CertificateManagerWindow.X509CertificateModel> certificates)
			: this(CreateDigitalSignatureDialog.GetRectSize(pdfDigitalSignatureLocation.Bounds), certificates)
		{
			this.pdfDigitalSignatureLocation = pdfDigitalSignatureLocation;
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x000AC7C0 File Offset: 0x000AA9C0
		public CreateDigitalSignatureDialog(Size size, IReadOnlyList<CertificateManagerWindow.X509CertificateModel> certificates)
		{
			this.InitializeComponent();
			this.signatureSize = size;
			List<CreateDigitalSignatureDialog.CertListModel> list = (from c in certificates.Where(delegate(CertificateManagerWindow.X509CertificateModel c)
				{
					if (c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.WindowsCertificateStore)
					{
						return SignatureValidateHelper.IsSupportedSignCertificate(c.Certificate);
					}
					return c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore;
				})
				select new CreateDigitalSignatureDialog.CertListModel(c.Certificate, c.CertificateStorageMechanism == CertificateManagerWindow.CertificateStorageMechanism.LocalCertificateStore)).OrderBy((CreateDigitalSignatureDialog.CertListModel c) => c.DisplayName, StringComparer.Ordinal).ToList<CreateDigitalSignatureDialog.CertListModel>();
			GAManager.SendEvent("CreateDigitalSignatureDialog", "CertListCount", list.Count.ToString(), 1L);
			this.itemsSource = new ObservableCollection<CreateDigitalSignatureDialog.CertListModel>(list);
			this.CertComboBox.ItemsSource = this.itemsSource;
			if (this.itemsSource.Count > 0)
			{
				bool flag = false;
				string lastSignedCertThumbprint = ConfigManager.GetLastSignedCertThumbprint();
				if (!string.IsNullOrEmpty(lastSignedCertThumbprint))
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!list[i].IsExpired)
						{
							X509Certificate2 certificate = list[i].Certificate;
							if (((certificate != null) ? certificate.Thumbprint : null) == lastSignedCertThumbprint)
							{
								flag = true;
								this.CertComboBox.SelectedIndex = i;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					this.CertComboBox.SelectedItem = list.FirstOrDefault((CreateDigitalSignatureDialog.CertListModel c) => !c.IsExpired);
				}
			}
			this.ReasonComboBox.ItemsSource = this.GetReasonList();
			this.ReasonComboBox.SelectedIndex = 0;
			this.UpdateCertInfoBtnState();
			this.UpdateCanSign();
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x0600250B RID: 9483 RVA: 0x000AC967 File Offset: 0x000AAB67
		// (set) Token: 0x0600250C RID: 9484 RVA: 0x000AC96F File Offset: 0x000AAB6F
		public X509Certificate2 X509Certificate { get; private set; }

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x0600250D RID: 9485 RVA: 0x000AC978 File Offset: 0x000AAB78
		// (set) Token: 0x0600250E RID: 9486 RVA: 0x000AC980 File Offset: 0x000AAB80
		public Thickness DrawingLeftElementMargin { get; set; }

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x0600250F RID: 9487 RVA: 0x000AC989 File Offset: 0x000AAB89
		// (set) Token: 0x06002510 RID: 9488 RVA: 0x000AC991 File Offset: 0x000AAB91
		public Thickness DrawingRightElementMargin { get; set; }

		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x06002511 RID: 9489 RVA: 0x000AC99A File Offset: 0x000AAB9A
		// (set) Token: 0x06002512 RID: 9490 RVA: 0x000AC9A2 File Offset: 0x000AABA2
		public bool Certificated { get; private set; }

		// Token: 0x06002513 RID: 9491 RVA: 0x000AC9AB File Offset: 0x000AABAB
		private void UpdateCertInfoBtnState()
		{
			if (this.CertComboBox.SelectedItem is CreateDigitalSignatureDialog.CertListModel)
			{
				this.CertInfoButton.Visibility = Visibility.Visible;
				return;
			}
			this.CertInfoButton.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x000AC9D8 File Offset: 0x000AABD8
		private IReadOnlyList<string> GetReasonList()
		{
			return new List<string>
			{
				pdfeditor.Properties.Resources.CreateDigSignReason1,
				pdfeditor.Properties.Resources.CreateDigSignReason2,
				pdfeditor.Properties.Resources.CreateDigSignReason3,
				pdfeditor.Properties.Resources.CreateDigSignReason4,
				pdfeditor.Properties.Resources.CreateDigSignReason5,
				pdfeditor.Properties.Resources.CreateDigSignReason6,
				pdfeditor.Properties.Resources.CreateDigSignReason7
			};
		}

		// Token: 0x06002515 RID: 9493 RVA: 0x000ACA38 File Offset: 0x000AAC38
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CreateDigitalSignatureDialog", "OkBtn", "Count", 1L);
			GAManager.SendEvent("DigitalSignature", "AddDigitalSignature", "AddDigitalSignature", 1L);
			CreateDigitalSignatureDialog.CertListModel certListModel = this.CertComboBox.SelectedItem as CreateDigitalSignatureDialog.CertListModel;
			if (certListModel != null && certListModel.Certificate != null)
			{
				if (certListModel.IsExpired)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.CreateDigSignMessage_CertExpired, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				X509Certificate2 x509Certificate = null;
				if (certListModel.NeedPassword)
				{
					X509CertificateFile certificate = CertificateManager.SignatureCertificateStorage.GetCertificate(certListModel.Certificate.Thumbprint);
					if (certificate != null)
					{
						try
						{
							x509Certificate = new X509Certificate2(certificate.FileData, this.CertPasswordBox.Password);
						}
						catch
						{
						}
					}
					if (x509Certificate == null)
					{
						ModernMessageBox.Show(pdfeditor.Properties.Resources.ResourceManager.GetString("CreateDigSignMessage_IncorrectPassword"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return;
					}
				}
				else
				{
					x509Certificate = certListModel.Certificate;
				}
				if (x509Certificate == null)
				{
					ModernMessageBox.Show(this, pdfeditor.Properties.Resources.ResourceManager.GetString("AddCertificateWindow_CreateCertFile_FailedMessage"), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				string text;
				if (certListModel == null)
				{
					text = null;
				}
				else
				{
					X509Certificate2 certificate2 = certListModel.Certificate;
					text = ((certificate2 != null) ? certificate2.Thumbprint : null);
				}
				ConfigManager.SetLastSignedCertThumbprint(text);
				this.Certificated = this.CertificatedCheckBox.IsChecked ?? false;
				this.X509Certificate = x509Certificate;
				base.DialogResult = new bool?(true);
			}
		}

		// Token: 0x06002516 RID: 9494 RVA: 0x000ACBA0 File Offset: 0x000AADA0
		private void BrowserIdFileButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CreateDigitalSignatureDialog", "NewDigitalID", "NewBtnClick", 1L);
			CreateDigitalSignatureDialog.CertListModel certListModel = this.ImportCert();
			if (certListModel != null && !certListModel.IsExpired)
			{
				this.CertComboBox.SelectedItem = certListModel;
			}
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x000ACBE1 File Offset: 0x000AADE1
		private void CertComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.itemsSource.Count == 1)
			{
				this.CertComboBox.SelectedIndex = 0;
			}
			this.UpdateCertInfoBtnState();
			this.UpdatePreviewImage();
			this.UpdateCanSign();
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x000ACC10 File Offset: 0x000AAE10
		private void CertInfoButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CreateDigitalSignatureDialog", "ShowCertificate", "Count", 1L);
			CreateDigitalSignatureDialog.CertListModel certListModel = this.CertComboBox.SelectedItem as CreateDigitalSignatureDialog.CertListModel;
			if (certListModel != null)
			{
				IntPtr intPtr = IntPtr.Zero;
				HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
				if (hwndSource != null)
				{
					intPtr = hwndSource.Handle;
				}
				certListModel.Certificate.ShowCertificateDialog(intPtr, null);
			}
		}

		// Token: 0x06002519 RID: 9497 RVA: 0x000ACC74 File Offset: 0x000AAE74
		private void PreviewImageBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.WidthChanged && e.PreviousSize.Width == 0.0)
			{
				this.UpdatePreviewImage();
			}
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x000ACCA8 File Offset: 0x000AAEA8
		private void LocationText_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x0600251B RID: 9499 RVA: 0x000ACCB0 File Offset: 0x000AAEB0
		private void ReasonComboBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdatePreviewImage();
		}

		// Token: 0x0600251C RID: 9500 RVA: 0x000ACCB8 File Offset: 0x000AAEB8
		private CreateDigitalSignatureDialog.CertListModel ImportCert()
		{
			AddCertificateWindow addCertificateWindow = new AddCertificateWindow
			{
				Owner = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			bool? flag = addCertificateWindow.ShowDialog();
			if (flag != null && flag.GetValueOrDefault() && addCertificateWindow.Certificate != null)
			{
				GAManager.SendEvent("CreateDigitalSignatureDialog", "NewDigitalID", "CertImported", 1L);
				X509Certificate2 cert = addCertificateWindow.Certificate;
				CreateDigitalSignatureDialog.CertListModel certListModel = this.itemsSource.FirstOrDefault((CreateDigitalSignatureDialog.CertListModel c) => c.Certificate.Thumbprint == cert.Thumbprint);
				if (certListModel == null)
				{
					certListModel = new CreateDigitalSignatureDialog.CertListModel(cert, true);
					this.itemsSource.Insert(this.itemsSource.Count, certListModel);
				}
				this.UpdateCanSign();
				return certListModel;
			}
			return null;
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x000ACD70 File Offset: 0x000AAF70
		private void UpdateCanSign()
		{
			UIElement uielement = this.btnOk;
			CreateDigitalSignatureDialog.CertListModel certListModel = this.CertComboBox.SelectedItem as CreateDigitalSignatureDialog.CertListModel;
			uielement.IsEnabled = certListModel != null && certListModel.Certificate != null && !certListModel.IsExpired;
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x000ACDB0 File Offset: 0x000AAFB0
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);
			this.UpdatePreviewImage();
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x000ACDC0 File Offset: 0x000AAFC0
		private void UpdatePreviewImage()
		{
			if (this.PreviewImageBorder.ActualWidth != 0.0 && this.CertComboBox.SelectedItem is CreateDigitalSignatureDialog.CertListModel)
			{
				DpiScale dpi = VisualTreeHelper.GetDpi(this);
				Size size = new Size(this.PreviewImageBorder.ActualWidth, 0.27777777777777779 * this.PreviewImageBorder.ActualWidth);
				DigitalSignatureDrawingHelper digitalSignatureDrawingHelper = this.CreateDrawingHelper();
				BitmapSource bitmapSource = ((digitalSignatureDrawingHelper != null) ? digitalSignatureDrawingHelper.CreateBitmapSource(size, (uint)dpi.PixelsPerInchY) : null);
				this.PreviewImage.Source = bitmapSource;
				return;
			}
			this.PreviewImage.Source = null;
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x000ACE58 File Offset: 0x000AB058
		internal DigitalSignatureDrawingHelper CreateDrawingHelper()
		{
			if (this.PreviewImageBorder.ActualWidth != 0.0)
			{
				CreateDigitalSignatureDialog.CertListModel certListModel = this.CertComboBox.SelectedItem as CreateDigitalSignatureDialog.CertListModel;
				if (certListModel != null)
				{
					DigitalSignatureDrawingHelper digitalSignatureDrawingHelper = new DigitalSignatureDrawingHelper();
					digitalSignatureDrawingHelper.LeftElement = DigitalSignatureDrawingLeftElement.CreateText(certListModel.DisplayName, TextAlignment.Center);
					digitalSignatureDrawingHelper.RightElement = new DigitalSignatureDrawingRightElement
					{
						ElementType = DigitalSignatureDrawingRightElementType.All,
						Name = certListModel.DisplayName,
						DistinguishedName = certListModel.Certificate.Subject,
						Location = this.LocationText.Text,
						Reason = this.ReasonComboBox.Text,
						AutoUpdateDateOnDrawing = true
					};
					digitalSignatureDrawingHelper.LeftElementMargin = this.DrawingLeftElementMargin;
					digitalSignatureDrawingHelper.RightElementMargin = this.DrawingRightElementMargin;
					digitalSignatureDrawingHelper.AppIcon = new BitmapImage(new Uri("pack://application:,,,/pdfeditor;component/Style/Resources/logo.png"));
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["Name"] = pdfeditor.Properties.Resources.DigSignPropPreviewSignedBy;
					dictionary["DistinguishedName"] = "DN: ";
					dictionary["Reason"] = pdfeditor.Properties.Resources.DigSignPropReason;
					dictionary["Location"] = pdfeditor.Properties.Resources.DigSignPropLocation;
					dictionary["Date"] = pdfeditor.Properties.Resources.DigSignPropSignDate;
					dictionary["AppVersion"] = pdfeditor.Properties.Resources.DigSignPropPreviewAppVer;
					digitalSignatureDrawingHelper.LocalizationStrings = dictionary;
					return digitalSignatureDrawingHelper;
				}
			}
			return null;
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000ACF9F File Offset: 0x000AB19F
		public static bool CanCreate(PdfDigitalSignatureLocation pdfDigitalSignatureLocation, Thickness leftThickness, Thickness rightThickness)
		{
			return pdfDigitalSignatureLocation != null && !pdfDigitalSignatureLocation.HasSigned && CreateDigitalSignatureDialog.CanCreate(CreateDigitalSignatureDialog.GetRectSize(pdfDigitalSignatureLocation.Bounds), leftThickness, rightThickness);
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x000ACFC2 File Offset: 0x000AB1C2
		public static bool CanCreate(Size size, Thickness leftThickness, Thickness rightThickness)
		{
			return DigitalSignatureDrawingHelper.CanCreate(size, in leftThickness, in rightThickness);
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x000ACFCE File Offset: 0x000AB1CE
		private static Size GetRectSize(FS_RECTF rect)
		{
			return new Size((double)rect.Width, (double)rect.Height);
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x000ACFE5 File Offset: 0x000AB1E5
		private static string GetCommonName(string name)
		{
			return new CertificateManagerWindow.CertificateName(name).CommonName;
		}

		// Token: 0x04000FD1 RID: 4049
		private ObservableCollection<CreateDigitalSignatureDialog.CertListModel> itemsSource;

		// Token: 0x04000FD2 RID: 4050
		private readonly Size signatureSize;

		// Token: 0x04000FD3 RID: 4051
		private PdfDigitalSignatureLocation pdfDigitalSignatureLocation;

		// Token: 0x02000733 RID: 1843
		private class CertListModel
		{
			// Token: 0x06003633 RID: 13875 RVA: 0x00110348 File Offset: 0x0010E548
			public CertListModel(X509Certificate2 certificate, bool needPassword)
			{
				this.Certificate = certificate;
				this.NeedPassword = needPassword;
				this.DisplayName = CreateDigitalSignatureDialog.GetCommonName(certificate.Subject);
				this.Issuer = CreateDigitalSignatureDialog.GetCommonName(certificate.Issuer);
				this.IsExpired = certificate.NotAfter < DateTime.Now;
				this.ExpirationDateString = certificate.NotAfter.ToString("g");
			}

			// Token: 0x17000D76 RID: 3446
			// (get) Token: 0x06003634 RID: 13876 RVA: 0x001103BA File Offset: 0x0010E5BA
			public X509Certificate2 Certificate { get; }

			// Token: 0x17000D77 RID: 3447
			// (get) Token: 0x06003635 RID: 13877 RVA: 0x001103C2 File Offset: 0x0010E5C2
			public string DisplayName { get; }

			// Token: 0x17000D78 RID: 3448
			// (get) Token: 0x06003636 RID: 13878 RVA: 0x001103CA File Offset: 0x0010E5CA
			public string Issuer { get; }

			// Token: 0x17000D79 RID: 3449
			// (get) Token: 0x06003637 RID: 13879 RVA: 0x001103D2 File Offset: 0x0010E5D2
			public bool IsExpired { get; }

			// Token: 0x17000D7A RID: 3450
			// (get) Token: 0x06003638 RID: 13880 RVA: 0x001103DA File Offset: 0x0010E5DA
			public string ExpirationDateString { get; }

			// Token: 0x17000D7B RID: 3451
			// (get) Token: 0x06003639 RID: 13881 RVA: 0x001103E2 File Offset: 0x0010E5E2
			public bool NeedPassword { get; }

			// Token: 0x0400248F RID: 9359
			private bool? needPassword;

			// Token: 0x04002490 RID: 9360
			private X509Certificate2 certificate;
		}
	}
}
