using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PDFKit.ToolBars
{
	// Token: 0x02000046 RID: 70
	internal class PrintProgress : Window, IComponentConnector
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000325 RID: 805 RVA: 0x0000F2AC File Offset: 0x0000D4AC
		// (remove) Token: 0x06000326 RID: 806 RVA: 0x0000F2E4 File Offset: 0x0000D4E4
		public event EventHandler NeedStopPrinting;

		// Token: 0x06000327 RID: 807 RVA: 0x0000F319 File Offset: 0x0000D519
		public PrintProgress()
		{
			this.InitializeComponent();
			base.Closing += this.PrintProgress_Closing;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000F33C File Offset: 0x0000D53C
		private void PrintProgress_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			PrintProgress.DelegateType1 delegateType = new PrintProgress.DelegateType1(this.ShowMessageBoxOnClose);
			base.Dispatcher.BeginInvoke(delegateType, Array.Empty<object>());
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000F370 File Offset: 0x0000D570
		private void ShowMessageBoxOnClose()
		{
			PrintProgress.DelegateType1 delegateType = new PrintProgress.DelegateType1(this.SendCancelationEvent);
			if (MessageBox.Show("", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				base.Dispatcher.BeginInvoke(delegateType, Array.Empty<object>());
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000F3B1 File Offset: 0x0000D5B1
		private void SendCancelationEvent()
		{
			if (this.NeedStopPrinting != null)
			{
				this.NeedStopPrinting(this, EventArgs.Empty);
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000F3CC File Offset: 0x0000D5CC
		internal void CloseWithoutPrompt()
		{
			base.Closing -= this.PrintProgress_Closing;
			base.Owner = null;
			base.Close();
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000F3ED File Offset: 0x0000D5ED
		internal void SetText(int pageNumber, int count)
		{
			this.textBlock.Text = string.Format("", pageNumber, count);
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000F410 File Offset: 0x0000D610
		internal void SetText(string txt)
		{
			this.textBlock.Text = txt;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000F41E File Offset: 0x0000D61E
		private void button_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000F428 File Offset: 0x0000D628
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/pdfeditor;component/utils/print/printprogress.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000F458 File Offset: 0x0000D658
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				this.textBlock = (TextBlock)target;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.button = (Button)target;
			this.button.Click += this.button_Click;
		}

		// Token: 0x04000144 RID: 324
		internal TextBlock textBlock;

		// Token: 0x04000145 RID: 325
		internal Button button;

		// Token: 0x04000146 RID: 326
		private bool _contentLoaded;

		// Token: 0x020002D4 RID: 724
		// (Invoke) Token: 0x060028E3 RID: 10467
		private delegate void DelegateType1();
	}
}
