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

namespace pdfeditor.Views
{
	// Token: 0x02000052 RID: 82
	public partial class Survey : Window
	{
		// Token: 0x0600041C RID: 1052 RVA: 0x0001552F File Offset: 0x0001372F
		public Survey()
		{
			this.InitializeComponent();
			GAManager.SendEvent("SurveyWindow", "Show", "Count", 1L);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00015554 File Offset: 0x00013754
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("SurveyWindow", "SurveyBtn", "Count", 1L);
			string text = CultureInfoUtils.ActualAppLanguage;
			string text2 = "https://www.pdfgear.com/";
			string text3 = "share";
			if (text.ToLower() == "ko")
			{
				text = "kr";
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
			}
			base.Close();
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0001568C File Offset: 0x0001388C
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("SurveyWindow", "CloseBtn", "Count", 1L);
			base.Close();
		}
	}
}
