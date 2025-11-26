using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Common;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000216 RID: 534
	public partial class ScreenshotImageToolbar : UserControl
	{
		// Token: 0x06001D96 RID: 7574 RVA: 0x0007F809 File Offset: 0x0007DA09
		public ScreenshotImageToolbar()
		{
			this.InitializeComponent();
			this.DrawSettingToolbar.PropertyChanged += this.DrawSettingToolbar_PropertyChanged;
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06001D97 RID: 7575 RVA: 0x0007F82E File Offset: 0x0007DA2E
		// (set) Token: 0x06001D98 RID: 7576 RVA: 0x0007F840 File Offset: 0x0007DA40
		public ScreenshotDialog ScreenshotDialog
		{
			get
			{
				return (ScreenshotDialog)base.GetValue(ScreenshotImageToolbar.ScreenshotDialogProperty);
			}
			set
			{
				if (this.ScreenshotDialog != null)
				{
					this.ScreenshotDialog.PropertyChanged -= this.ScreenshotDialog_PropertyChanged;
				}
				base.SetValue(ScreenshotImageToolbar.ScreenshotDialogProperty, value);
				this.ScreenshotDialog.PropertyChanged += this.ScreenshotDialog_PropertyChanged;
			}
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x0007F890 File Offset: 0x0007DA90
		private void ScreenshotDialog_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "DrawControlMode")
			{
				DrawControlMode drawControlMode = this.ScreenshotDialog.DrawControlMode;
				this.DrawSettingToolbar.DrawControlMode = drawControlMode;
				if (drawControlMode == DrawControlMode.None)
				{
					this.DrawSettingToolbar.Visibility = Visibility.Collapsed;
					return;
				}
				this.DrawSettingToolbar.Visibility = Visibility.Visible;
				return;
			}
			else
			{
				if (e.PropertyName == "CurrentBrush")
				{
					this.DrawSettingToolbar.Color = ((SolidColorBrush)this.ScreenshotDialog.CurrentBrush).Color;
					return;
				}
				if (e.PropertyName == "CurrentThickness")
				{
					this.DrawSettingToolbar.Thickness = this.ScreenshotDialog.CurrentThickness;
					return;
				}
				if (e.PropertyName == "CurrentFontSize")
				{
					this.DrawSettingToolbar.DrawFontSize = this.ScreenshotDialog.CurrentFontSize;
				}
				return;
			}
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x0007F968 File Offset: 0x0007DB68
		private void DrawSettingToolbar_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			DrawSettingToolbar drawSettingToolbar = sender as DrawSettingToolbar;
			if (drawSettingToolbar != null)
			{
				if (e.PropertyName == "Color")
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush(drawSettingToolbar.Color);
					this.ScreenshotDialog.CurrentBrush = solidColorBrush;
					this.ScreenshotDialog.SetDrawControlBrush(this.ScreenshotDialog.SelectedDrawControl, solidColorBrush, true);
					return;
				}
				if (e.PropertyName == "Thickness")
				{
					this.ScreenshotDialog.CurrentThickness = drawSettingToolbar.Thickness;
					this.ScreenshotDialog.SetDrawControlThickness(this.ScreenshotDialog.SelectedDrawControl, drawSettingToolbar.Thickness, true);
					return;
				}
				if (e.PropertyName == "DrawFontSize")
				{
					this.ScreenshotDialog.CurrentFontSize = drawSettingToolbar.DrawFontSize;
					this.ScreenshotDialog.SetDrawTextFontSize(this.ScreenshotDialog.SelectedDrawControl, drawSettingToolbar.DrawFontSize, true);
				}
			}
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x0007FA48 File Offset: 0x0007DC48
		private async void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			await ((screenshotDialog != null) ? screenshotDialog.SaveImageAsync() : null);
			((Button)sender).IsEnabled = true;
			GAManager.SendEvent("Screenshot", "Save", "Count", 1L);
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x0007FA88 File Offset: 0x0007DC88
		private async void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			await ((screenshotDialog != null) ? screenshotDialog.CopyToClipboardAsync() : null);
			((Button)sender).IsEnabled = true;
			GAManager.SendEvent("Screenshot", "Copy", "Count", 1L);
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x0007FAC8 File Offset: 0x0007DCC8
		private async void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			await ((screenshotDialog != null) ? screenshotDialog.CompleteImageAsync() : null);
			((Button)sender).IsEnabled = true;
			GAManager.SendEvent("Screenshot", "Ok", "Count", 1L);
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x0007FB07 File Offset: 0x0007DD07
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog != null)
			{
				screenshotDialog.Close(null);
			}
			GAManager.SendEvent("Screenshot", "Cancel", "Count", 1L);
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x0007FB31 File Offset: 0x0007DD31
		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog == null)
			{
				return;
			}
			screenshotDialog.UndoDrawControl();
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x0007FB44 File Offset: 0x0007DD44
		private async void ExtractTextBtn_Click(object sender, RoutedEventArgs e)
		{
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog != null)
			{
				screenshotDialog.ExtractText();
			}
		}

		// Token: 0x04000B26 RID: 2854
		public static readonly DependencyProperty ScreenshotDialogProperty = DependencyProperty.Register("ScreenshotDialog", typeof(ScreenshotDialog), typeof(ScreenshotImageToolbar), new PropertyMetadata(null));
	}
}
