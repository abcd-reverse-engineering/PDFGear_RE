using System;
using System.Runtime.CompilerServices;
using CommonLib.Common;
using CommonLib.IAP;

namespace PDFLauncher.Utils
{
	// Token: 0x02000016 RID: 22
	public static class IAPUtils
	{
		// Token: 0x06000171 RID: 369 RVA: 0x00006554 File Offset: 0x00004754
		public static async void GetIAPProductsAsync()
		{
			try
			{
				TaskAwaiter<bool> taskAwaiter = IAPHelper.ShouldRenewAsync().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					await IAPHelper.RenewAccessTokenAsync();
				}
				else
				{
					await IAPHelper.UpdateUserInfoAsync();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00006583 File Offset: 0x00004783
		public static void ShowPurchaseWindows(string source, string ext)
		{
			IAPHelper.ShowActivationWindow(source, ext);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000658D File Offset: 0x0000478D
		public static void ShowPaidWindows()
		{
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000658F File Offset: 0x0000478F
		public static bool IsPaidUserWrapper()
		{
			return IAPHelper.IsPaidUser;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00006598 File Offset: 0x00004798
		public static bool NeedToShowIAP()
		{
			if (IAPUtils.IsPaidUserWrapper())
			{
				return false;
			}
			if ((DateTime.Now - new DateTime(2000, 1, 1, 0, 0, 0)).TotalSeconds - (double)ConfigManager.GetLastShowIAPTimestamp() < 86400.0)
			{
				if (ConfigManager.GetStartupShowIAPOneDayCount() >= 3L)
				{
					return false;
				}
			}
			else
			{
				ConfigManager.SetStartupShowIAPOneDayCount(0L);
			}
			long num = ConfigManager.GetStartupShowIAPOneDayCount() + 1L;
			ConfigManager.SetStartupShowIAPOneDayCount(num);
			if (num == 1L)
			{
				ConfigManager.SetLastShowIAPTimestamp((long)(DateTime.Now - new DateTime(2000, 1, 1, 0, 0, 0)).TotalSeconds);
			}
			return true;
		}
	}
}
