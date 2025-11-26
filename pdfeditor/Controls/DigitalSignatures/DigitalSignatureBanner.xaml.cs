using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Controls;
using pdfeditor.Properties;
using pdfeditor.Utils.DigitalSignatures;
using PDFKit.Utils;
using PDFKit.Utils.DigitalSignatures;
using Syncfusion.Pdf.Security;

namespace pdfeditor.Controls.DigitalSignatures
{
	// Token: 0x02000284 RID: 644
	public partial class DigitalSignatureBanner : UserControl
	{
		// Token: 0x06002531 RID: 9521 RVA: 0x000AD341 File Offset: 0x000AB541
		public DigitalSignatureBanner()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x06002532 RID: 9522 RVA: 0x000AD34F File Offset: 0x000AB54F
		// (set) Token: 0x06002533 RID: 9523 RVA: 0x000AD361 File Offset: 0x000AB561
		public DigitalSignatureHelper DigitalSignatureHelper
		{
			get
			{
				return (DigitalSignatureHelper)base.GetValue(DigitalSignatureBanner.DigitalSignatureHelperProperty);
			}
			set
			{
				base.SetValue(DigitalSignatureBanner.DigitalSignatureHelperProperty, value);
			}
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x000AD370 File Offset: 0x000AB570
		private async Task UpdateHelper(DigitalSignatureHelper newValue, DigitalSignatureHelper oldValue)
		{
			if (oldValue != null)
			{
				foreach (PdfDigitalSignatureField pdfDigitalSignatureField in await oldValue.GetDigitalSignaturesAsync())
				{
					if (!(((pdfDigitalSignatureField != null) ? new bool?(pdfDigitalSignatureField.IsBroken) : null) ?? true))
					{
						WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(pdfDigitalSignatureField, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnSignPropertyChanged));
					}
				}
			}
			if (newValue != null)
			{
				foreach (PdfDigitalSignatureField pdfDigitalSignatureField2 in await newValue.GetDigitalSignaturesAsync())
				{
					if (!(((pdfDigitalSignatureField2 != null) ? new bool?(pdfDigitalSignatureField2.IsBroken) : null) ?? true))
					{
						WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(pdfDigitalSignatureField2, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnSignPropertyChanged));
					}
				}
			}
		}

		// Token: 0x06002535 RID: 9525 RVA: 0x000AD3C4 File Offset: 0x000AB5C4
		private async void OnSignPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LastValidateResult")
			{
				await this.UpdateBannerContent();
			}
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x000AD404 File Offset: 0x000AB604
		private async Task UpdateBannerContent()
		{
			DigitalSignatureHelper helper = this.DigitalSignatureHelper;
			if (helper != null && helper.HasDigitalSignatures)
			{
				IReadOnlyList<PdfSignatureValidationResult> readOnlyList = await this.GetSignatureValidateResults(helper);
				IReadOnlyList<PdfSignatureValidationResult> results = readOnlyList;
				int count = (await helper.GetDigitalSignaturesAsync()).Count;
				int num = 0;
				int num2 = 0;
				int num3 = helper.Locations.Count((PdfDigitalSignatureLocation c) => !c.HasSigned);
				for (int i = 0; i < results.Count; i++)
				{
					if (results[i].IsValid(true))
					{
						num++;
					}
					else if (results[i].SignatureStatus == SignatureStatus.Unknown)
					{
						num2++;
					}
				}
				int num4 = count - num - num2 - num3;
				StringBuilder stringBuilder = new StringBuilder();
				if (num2 == 0 && num4 == 0 && num3 == 0)
				{
					stringBuilder.Append(pdfeditor.Properties.Resources.DigSignBannerMessage_Valid);
				}
				else if (num4 != 0 || num2 != 0)
				{
					stringBuilder.Append(pdfeditor.Properties.Resources.DigSignBannerMessage_Invalid);
				}
				else if (num3 != 0)
				{
					stringBuilder.Append(pdfeditor.Properties.Resources.ResourceManager.GetString("DigSignBannerMessage_NeedSign"));
				}
				this.TextPresenter.Text = stringBuilder.ToString();
				results = null;
			}
			else
			{
				this.TextPresenter.Text = "";
			}
		}

		// Token: 0x06002537 RID: 9527 RVA: 0x000AD448 File Offset: 0x000AB648
		private async Task<IReadOnlyList<PdfSignatureValidationResult>> GetSignatureValidateResults(DigitalSignatureHelper helper)
		{
			IReadOnlyList<PdfSignatureValidationResult> readOnlyList2;
			if (helper != null && helper.HasDigitalSignatures)
			{
				List<PdfSignatureValidationResult> list = new List<PdfSignatureValidationResult>();
				IReadOnlyList<PdfDigitalSignatureField> readOnlyList = await helper.GetDigitalSignaturesAsync();
				IReadOnlyList<PdfDigitalSignatureField> digitalSignatures = readOnlyList;
				for (int i = 0; i < digitalSignatures.Count; i++)
				{
					try
					{
						if (digitalSignatures[i].LastValidateResult != null)
						{
							list.Add(digitalSignatures[i].LastValidateResult);
						}
						else
						{
							PdfSignatureValidationResult pdfSignatureValidationResult = await digitalSignatures[i].ValidateSignatureAsync(true, true);
							if (pdfSignatureValidationResult != null)
							{
								list.Add(pdfSignatureValidationResult);
							}
						}
					}
					catch
					{
					}
				}
				readOnlyList2 = list;
			}
			else
			{
				readOnlyList2 = Array.Empty<PdfSignatureValidationResult>();
			}
			return readOnlyList2;
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x000AD48C File Offset: 0x000AB68C
		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (((ScrollViewer)sender).ScrollableWidth > 0.0)
			{
				double horizontalOffset = ((ScrollViewer)sender).HorizontalOffset;
				MouseTiltEventArgs mouseTiltEventArgs = e as MouseTiltEventArgs;
				int num;
				if (mouseTiltEventArgs != null)
				{
					num = mouseTiltEventArgs.Delta / 2;
				}
				else
				{
					num = -e.Delta / 2;
				}
				((ScrollViewer)sender).ScrollToHorizontalOffset(horizontalOffset + (double)num);
			}
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x000AD4EB File Offset: 0x000AB6EB
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignature", "DigitalSignatureBanner", "CloseBtn", 1L);
			base.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x000AD50A File Offset: 0x000AB70A
		private void ShowButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("DigitalSignature", "DigitalSignatureBanner", "ShowBtn", 1L);
			EventHandler showButtonClick = this.ShowButtonClick;
			if (showButtonClick == null)
			{
				return;
			}
			showButtonClick(this, EventArgs.Empty);
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x0600253B RID: 9531 RVA: 0x000AD538 File Offset: 0x000AB738
		// (remove) Token: 0x0600253C RID: 9532 RVA: 0x000AD570 File Offset: 0x000AB770
		public event EventHandler ShowButtonClick;

		// Token: 0x04000FEB RID: 4075
		public static readonly DependencyProperty DigitalSignatureHelperProperty = DependencyProperty.Register("DigitalSignatureHelper", typeof(DigitalSignatureHelper), typeof(DigitalSignatureBanner), new PropertyMetadata(null, async delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			DigitalSignatureBanner sender = s as DigitalSignatureBanner;
			if (sender != null && !object.Equals(a.NewValue, a.OldValue))
			{
				DigitalSignatureHelper digitalSignatureHelper = a.NewValue as DigitalSignatureHelper;
				if (digitalSignatureHelper != null && digitalSignatureHelper.HasDigitalSignatures)
				{
					sender.Visibility = Visibility.Visible;
				}
				await sender.UpdateHelper(a.NewValue as DigitalSignatureHelper, a.OldValue as DigitalSignatureHelper);
				await sender.UpdateBannerContent();
			}
		}));
	}
}
