using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;

namespace pdfeditor.Views
{
	// Token: 0x02000051 RID: 81
	public partial class SubscribeWindow : Window
	{
		// Token: 0x06000410 RID: 1040 RVA: 0x00015354 File Offset: 0x00013554
		public SubscribeWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00015362 File Offset: 0x00013562
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			GAManager.SendEvent("YTSub", "Show", "Count", 1L);
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00015384 File Offset: 0x00013584
		private void OpenChannel()
		{
			GAManager.SendEvent("YTSub", "Subscribe", "Count", 1L);
			try
			{
				Process.Start("https://www.youtube.com/@pdfgear?sub_confirmation=1");
			}
			catch
			{
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000153C8 File Offset: 0x000135C8
		private void OpenTwitterChannel()
		{
			GAManager.SendEvent("TwitterSub", "Subscribe", "Count", 1L);
			try
			{
				Process.Start("https://twitter.com/intent/user?screen_name=PDFgear");
			}
			catch
			{
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001540C File Offset: 0x0001360C
		private void OpenTrustpliot()
		{
			GAManager.SendEvent("TrustpliotSub", "Subscribe", "Count", 1L);
			try
			{
				Process.Start("https://www.trustpilot.com/evaluate/pdfgear.com");
			}
			catch
			{
			}
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00015450 File Offset: 0x00013650
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.OpenChannel();
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00015458 File Offset: 0x00013658
		private void TrustpliotButton_Click(object sender, RoutedEventArgs e)
		{
			this.OpenTrustpliot();
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00015460 File Offset: 0x00013660
		private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.OpenChannel();
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00015468 File Offset: 0x00013668
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.OpenTwitterChannel();
		}
	}
}
