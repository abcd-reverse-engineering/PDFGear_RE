using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Cache;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using CommonLib.Common;

namespace PDFLauncher
{
	// Token: 0x0200000D RID: 13
	public partial class EOSWindow : Window
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002954 File Offset: 0x00000B54
		public EOSWindow(PromoteAd promoteAd)
		{
			this.InitializeComponent();
			this.ad = promoteAd;
			if (this.ad != null)
			{
				GAManager.SendEvent("AdWin", "Show." + this.ad.adType.ToString(), this.ad.adID.ToString(), 1L);
				ConfigManager.setAdShowCount(ConfigManager.getAdShowCount() + 1U);
				this.uTitle.Text = this.ad.strTitle;
				this.uDesc.Text = this.ad.strDesc;
				this.uBtn.Content = this.ad.strBtn;
				try
				{
					RequestCachePolicy requestCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
					if (this.ad.strImg.Length > 0)
					{
						this.uImg.Source = new BitmapImage(new Uri(this.ad.strImg), requestCachePolicy);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002A5C File Offset: 0x00000C5C
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (this.ad != null)
			{
				GAManager.SendEvent("AdWin", "ClickBtn." + this.ad.adType.ToString(), this.ad.adID.ToString(), 1L);
				Process.Start(this.ad.strUrl);
			}
		}

		// Token: 0x04000013 RID: 19
		private PromoteAd ad;
	}
}
