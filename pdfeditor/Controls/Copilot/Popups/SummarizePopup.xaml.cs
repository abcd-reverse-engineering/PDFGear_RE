using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.Utils.Copilot;

namespace pdfeditor.Controls.Copilot.Popups
{
	// Token: 0x02000294 RID: 660
	public partial class SummarizePopup : PopupWindow
	{
		// Token: 0x060025F3 RID: 9715 RVA: 0x000B10B0 File Offset: 0x000AF2B0
		public SummarizePopup(CopilotHelper copilotHelper, string input)
		{
			this.copilotHelper = copilotHelper;
			this.input = input;
			this.cts = new CancellationTokenSource();
			this.InitializeComponent();
			this.SourceText.Text = input;
			base.Loaded += this.SummarizePopup_Loaded;
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x000B1100 File Offset: 0x000AF300
		private async void SummarizePopup_Loaded(object sender, RoutedEventArgs e)
		{
			SummarizePopup.<>c__DisplayClass4_0 CS$<>8__locals1 = new SummarizePopup.<>c__DisplayClass4_0();
			CS$<>8__locals1.<>4__this = this;
			this.Test.Text = "Loading...";
			try
			{
				CS$<>8__locals1.sb = new StringBuilder();
				await this.copilotHelper.SummarizeAsync(this.input, delegate(string s, CancellationToken ct)
				{
					SummarizePopup.<>c__DisplayClass4_0.<<SummarizePopup_Loaded>b__0>d <<SummarizePopup_Loaded>b__0>d;
					<<SummarizePopup_Loaded>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<SummarizePopup_Loaded>b__0>d.<>4__this = CS$<>8__locals1;
					<<SummarizePopup_Loaded>b__0>d.s = s;
					<<SummarizePopup_Loaded>b__0>d.<>1__state = -1;
					<<SummarizePopup_Loaded>b__0>d.<>t__builder.Start<SummarizePopup.<>c__DisplayClass4_0.<<SummarizePopup_Loaded>b__0>d>(ref <<SummarizePopup_Loaded>b__0>d);
					return <<SummarizePopup_Loaded>b__0>d.<>t__builder.Task;
				}, this.cts.Token);
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex)
			{
				Log.WriteLog(ex.ToString());
				this.ResultText.Text = "Error";
			}
			this.Test.Text = "Loaded";
		}

		// Token: 0x04001061 RID: 4193
		private readonly CopilotHelper copilotHelper;

		// Token: 0x04001062 RID: 4194
		private readonly string input;

		// Token: 0x04001063 RID: 4195
		private CancellationTokenSource cts;
	}
}
