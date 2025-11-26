using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using pdfeditor.Utils.DocumentOcr;
using PDFKit.Utils;

namespace pdfeditor.Controls.OcrComponents
{
	// Token: 0x0200025A RID: 602
	public partial class NeedRecognitionBanner : UserControl
	{
		// Token: 0x060022C0 RID: 8896 RVA: 0x000A3F52 File Offset: 0x000A2152
		public NeedRecognitionBanner()
		{
			this.InitializeComponent();
			base.Visibility = Visibility.Collapsed;
		}

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x060022C1 RID: 8897 RVA: 0x000A3F67 File Offset: 0x000A2167
		// (set) Token: 0x060022C2 RID: 8898 RVA: 0x000A3F79 File Offset: 0x000A2179
		public string DocumentPath
		{
			get
			{
				return (string)base.GetValue(NeedRecognitionBanner.DocumentPathProperty);
			}
			set
			{
				base.SetValue(NeedRecognitionBanner.DocumentPathProperty, value);
			}
		}

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000A3F87 File Offset: 0x000A2187
		// (set) Token: 0x060022C4 RID: 8900 RVA: 0x000A3F99 File Offset: 0x000A2199
		public PdfContentType DocumentContentType
		{
			get
			{
				return (PdfContentType)base.GetValue(NeedRecognitionBanner.DocumentContentTypeProperty);
			}
			set
			{
				base.SetValue(NeedRecognitionBanner.DocumentContentTypeProperty, value);
			}
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x000A3FAC File Offset: 0x000A21AC
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.documentClosed = true;
			this.UpdateVisibilityState();
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x000A3FBC File Offset: 0x000A21BC
		private async Task UpdateVisibilityState()
		{
			Visibility visibility = base.Visibility;
			bool flag = false;
			if (!this.documentClosed && !string.IsNullOrEmpty(this.DocumentPath) && ConfigManager.GetLaunchAPPShowFlag("LaunchOCR"))
			{
				TaskAwaiter<string> taskAwaiter = ConfigManager.GetDocumentPropertiesAsync(this.DocumentPath, "IgnoreRecognitionBanner", default(CancellationToken)).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<string> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<string>);
				}
				if (!(taskAwaiter.GetResult() == "1"))
				{
					flag = this.DocumentContentType == PdfContentType.ImageOrPath;
				}
			}
			base.Visibility = (flag ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x000A4000 File Offset: 0x000A2200
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

		// Token: 0x060022C8 RID: 8904 RVA: 0x000A4060 File Offset: 0x000A2260
		private void RecognitionButton_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			try
			{
				EventHandler recognitionRequested = this.RecognitionRequested;
				if (recognitionRequested != null)
				{
					recognitionRequested(this, EventArgs.Empty);
				}
			}
			finally
			{
				((Button)sender).IsEnabled = true;
			}
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x000A40B0 File Offset: 0x000A22B0
		private async void DoNotShowButton_Click(object sender, RoutedEventArgs e)
		{
			string documentPath = this.DocumentPath;
			this.documentClosed = true;
			GAManager.SendEvent("OCR", "BannerNotShow", "Count", 1L);
			this.UpdateVisibilityState();
			if (!string.IsNullOrEmpty(documentPath))
			{
				await ConfigManager.SetDocumentPropertiesAsync(documentPath, "IgnoreRecognitionBanner", "1");
			}
		}

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x060022CA RID: 8906 RVA: 0x000A40E8 File Offset: 0x000A22E8
		// (remove) Token: 0x060022CB RID: 8907 RVA: 0x000A4120 File Offset: 0x000A2320
		public event EventHandler RecognitionRequested;

		// Token: 0x04000ECF RID: 3791
		private bool documentClosed;

		// Token: 0x04000ED0 RID: 3792
		public static readonly DependencyProperty DocumentPathProperty = DependencyProperty.Register("DocumentPath", typeof(string), typeof(NeedRecognitionBanner), new PropertyMetadata(string.Empty, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			NeedRecognitionBanner needRecognitionBanner = s as NeedRecognitionBanner;
			if (needRecognitionBanner != null && !object.Equals(a.NewValue, a.OldValue))
			{
				needRecognitionBanner.Visibility = Visibility.Collapsed;
				needRecognitionBanner.documentClosed = false;
				needRecognitionBanner.UpdateVisibilityState();
			}
		}));

		// Token: 0x04000ED1 RID: 3793
		public static readonly DependencyProperty DocumentContentTypeProperty = DependencyProperty.Register("DocumentContentType", typeof(PdfContentType), typeof(NeedRecognitionBanner), new PropertyMetadata(PdfContentType.Text, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			NeedRecognitionBanner needRecognitionBanner2 = s as NeedRecognitionBanner;
			if (needRecognitionBanner2 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				needRecognitionBanner2.UpdateVisibilityState();
			}
		}));
	}
}
