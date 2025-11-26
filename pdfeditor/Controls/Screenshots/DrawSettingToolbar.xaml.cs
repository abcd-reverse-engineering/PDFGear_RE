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
	// Token: 0x0200020B RID: 523
	public partial class DrawSettingToolbar : UserControl, INotifyPropertyChanged
	{
		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06001D0A RID: 7434 RVA: 0x0007DC34 File Offset: 0x0007BE34
		// (set) Token: 0x06001D0B RID: 7435 RVA: 0x0007DC46 File Offset: 0x0007BE46
		public DrawControlMode DrawControlMode
		{
			get
			{
				return (DrawControlMode)base.GetValue(DrawSettingToolbar.DrawControlModeProperty);
			}
			set
			{
				base.SetValue(DrawSettingToolbar.DrawControlModeProperty, value);
				this.RaisePropertyChanged("DrawControlMode");
			}
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06001D0C RID: 7436 RVA: 0x0007DC64 File Offset: 0x0007BE64
		// (set) Token: 0x06001D0D RID: 7437 RVA: 0x0007DC6C File Offset: 0x0007BE6C
		public double Thickness
		{
			get
			{
				return this.thickness;
			}
			set
			{
				this.thickness = value;
				this.RaisePropertyChanged("Thickness");
				ConfigManager.SetScreenshotThickness(this.thickness);
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06001D0E RID: 7438 RVA: 0x0007DC8B File Offset: 0x0007BE8B
		// (set) Token: 0x06001D0F RID: 7439 RVA: 0x0007DC93 File Offset: 0x0007BE93
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.RaisePropertyChanged("Color");
				ConfigManager.SetScreenshotColor(this.color);
			}
		}

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06001D10 RID: 7440 RVA: 0x0007DCB2 File Offset: 0x0007BEB2
		// (set) Token: 0x06001D11 RID: 7441 RVA: 0x0007DCBA File Offset: 0x0007BEBA
		public double DrawFontSize
		{
			get
			{
				return this.drawFontSize;
			}
			set
			{
				this.drawFontSize = value;
				this.RaisePropertyChanged("DrawFontSize");
				ConfigManager.SetScreenshotFontSize(this.drawFontSize);
			}
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x0007DCD9 File Offset: 0x0007BED9
		public DrawSettingToolbar()
		{
			this.InitializeComponent();
			this.Thickness = ConfigManager.GetScreenshotThickness();
			this.Color = ConfigManager.GetScreenshotColor();
			this.DrawFontSize = ConfigManager.GetScreenshotFontSize();
		}

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x06001D13 RID: 7443 RVA: 0x0007DD08 File Offset: 0x0007BF08
		// (remove) Token: 0x06001D14 RID: 7444 RVA: 0x0007DD40 File Offset: 0x0007BF40
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001D15 RID: 7445 RVA: 0x0007DD75 File Offset: 0x0007BF75
		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x04000AE8 RID: 2792
		public static readonly DependencyProperty DrawControlModeProperty = DependencyProperty.Register("DrawControlMode", typeof(DrawControlMode), typeof(DrawSettingToolbar), new PropertyMetadata(null));

		// Token: 0x04000AE9 RID: 2793
		private double thickness;

		// Token: 0x04000AEA RID: 2794
		private Color color;

		// Token: 0x04000AEB RID: 2795
		private double drawFontSize;
	}
}
