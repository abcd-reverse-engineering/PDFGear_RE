using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.IAP;
using pdfeditor.Controls.Users;

namespace pdfeditor.Utils
{
	// Token: 0x0200007C RID: 124
	public static class IAPUtils
	{
		// Token: 0x060008D9 RID: 2265 RVA: 0x0002C2A4 File Offset: 0x0002A4A4
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

		// Token: 0x060008DA RID: 2266 RVA: 0x0002C2D3 File Offset: 0x0002A4D3
		public static void ShowPurchaseWindows(string source, string ext)
		{
			if (IAPHelper.ShowActivationWindow(source, ext))
			{
				DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
				{
					Window mainWindow = App.Current.MainWindow;
					if (mainWindow != null)
					{
						UserInfoControl userInfoControl = mainWindow.FindName("UserInfoControl") as UserInfoControl;
						if (userInfoControl == null)
						{
							return;
						}
						userInfoControl.Open();
					}
				}));
			}
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0002C309 File Offset: 0x0002A509
		public static void ShowPaidWindows()
		{
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x0002C30B File Offset: 0x0002A50B
		public static bool IsPaidUserWrapper()
		{
			return IAPHelper.IsPaidUser;
		}
	}
}
