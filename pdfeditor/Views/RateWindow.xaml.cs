using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Views;

namespace pdfeditor.Views
{
	// Token: 0x0200004E RID: 78
	public partial class RateWindow : Window
	{
		// Token: 0x060003F9 RID: 1017 RVA: 0x00014D60 File Offset: 0x00012F60
		public RateWindow()
		{
			this.InitializeComponent();
			GAManager.SendEvent("RateWindow", "Show", "Count", 1L);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00014D84 File Offset: 0x00012F84
		private void RateButton_Click(object sender, RoutedEventArgs e)
		{
			string text = CultureInfoUtils.ActualAppLanguage;
			string text2 = "https://www.pdfgear.com/";
			string text3 = "review-us";
			if (text.ToLower() == "ko")
			{
				text = "kr";
			}
			if (text.ToLower() == "zh-cn")
			{
				text = "zh";
			}
			if (text.ToLower() == "ja")
			{
				text = "jp";
			}
			string gearRate = Path.Combine(text2, text3).Replace("\\", "/");
			if (text != "en")
			{
				gearRate = Path.Combine(text2, text.ToLower(), text3).Replace("\\", "/");
			}
			object locker = new object();
			bool result = false;
			new Thread(delegate
			{
				try
				{
					Process.Start(gearRate);
					result = true;
				}
				catch
				{
					result = false;
				}
				finally
				{
					object locker2 = locker;
					lock (locker2)
					{
						Monitor.PulseAll(locker);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
			object locker3 = locker;
			lock (locker3)
			{
				Monitor.Wait(locker, 5000);
				bool result2 = result;
				GAManager.SendEvent("RateWindow", "BlockExit", "Count", 1L);
			}
			base.Close();
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00014ED4 File Offset: 0x000130D4
		public bool GenerateBoolRandom()
		{
			bool[] array = new bool[2];
			array[0] = true;
			Random random = new Random();
			return array[random.Next(2)];
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00014EFC File Offset: 0x000130FC
		private void FeedBackButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("RateWindow", "FeedbackBtn", "Count", 1L);
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.HideFaq();
			feedbackWindow.Owner = this;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			feedbackWindow.source = "Rate";
			feedbackWindow.ShowDialog();
			base.Close();
		}
	}
}
