using System;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfeditor.Views;

namespace pdfeditor.Utils
{
	// Token: 0x0200009D RID: 157
	internal class RateUtils
	{
		// Token: 0x060009FC RID: 2556 RVA: 0x00032A50 File Offset: 0x00030C50
		public static bool CheckAndShowRate(string file)
		{
			if (!ConfigManager.GetCouldRateFlag())
			{
				return false;
			}
			if (!ConfigManager.GetSubscriptionFlag())
			{
				ConfigManager.SetSubscriptionFlag(true);
				RateWindow rateWindow = new RateWindow();
				rateWindow.Owner = (MainView)Application.Current.MainWindow;
				rateWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				rateWindow.ShowDialog();
				rateWindow.Activate();
				return false;
			}
			if (!ConfigManager.GetPhoneQRCodeFlag())
			{
				GAManager.SendEvent("Ads", "GearForMobile", "ClosePDF", 1L);
				ConfigManager.SetPhoneQRCodeFlag(true);
				GearForMobilephone gearForMobilephone = new GearForMobilephone();
				gearForMobilephone.Owner = (MainView)Application.Current.MainWindow;
				gearForMobilephone.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				gearForMobilephone.ShowDialog();
				gearForMobilephone.Activate();
				return false;
			}
			if (RateUtils.IsNeedToShowSurvey())
			{
				ConfigManager.SetSurveyFlag(true);
				Survey survey = new Survey();
				survey.Owner = (MainView)Application.Current.MainWindow;
				survey.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				survey.ShowDialog();
				survey.Activate();
				return false;
			}
			return false;
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00032B34 File Offset: 0x00030D34
		public static bool IsNeedToShowSurvey()
		{
			return !ConfigManager.GetSurveyFlag() && CultureInfoUtils.ActualAppLanguage != "zh-CN" && ConfigManager.getAppLaunchCount() > 10L;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00032B5C File Offset: 0x00030D5C
		public static async Task<bool> ShowRatingReviewDialog()
		{
			return false;
		}
	}
}
