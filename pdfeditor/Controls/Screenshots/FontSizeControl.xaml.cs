using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200020C RID: 524
	public partial class FontSizeControl : UserControl
	{
		// Token: 0x06001D1A RID: 7450 RVA: 0x0007DE10 File Offset: 0x0007C010
		public FontSizeControl()
		{
			this.InitializeComponent();
			this.popupBtn_fontSize.Loaded += this.PopupBtn_fontSize_Loaded;
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x0007DF3B File Offset: 0x0007C13B
		// (set) Token: 0x06001D1C RID: 7452 RVA: 0x0007DF4D File Offset: 0x0007C14D
		public double SelectedFontSize
		{
			get
			{
				return (double)base.GetValue(FontSizeControl.SelectedFontSizeProperty);
			}
			set
			{
				base.SetValue(FontSizeControl.SelectedFontSizeProperty, value);
			}
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06001D1D RID: 7453 RVA: 0x0007DF60 File Offset: 0x0007C160
		// (set) Token: 0x06001D1E RID: 7454 RVA: 0x0007DF72 File Offset: 0x0007C172
		public string FontSizeUnit
		{
			get
			{
				return (string)base.GetValue(FontSizeControl.FontSizeUnitProperty);
			}
			set
			{
				base.SetValue(FontSizeControl.FontSizeUnitProperty, value);
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06001D1F RID: 7455 RVA: 0x0007DF80 File Offset: 0x0007C180
		// (set) Token: 0x06001D20 RID: 7456 RVA: 0x0007DF92 File Offset: 0x0007C192
		public bool IsShowAuto
		{
			get
			{
				return (bool)base.GetValue(FontSizeControl.IsShowAutoProperty);
			}
			set
			{
				base.SetValue(FontSizeControl.IsShowAutoProperty, value);
			}
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06001D21 RID: 7457 RVA: 0x0007DFA5 File Offset: 0x0007C1A5
		// (set) Token: 0x06001D22 RID: 7458 RVA: 0x0007DFB7 File Offset: 0x0007C1B7
		public bool IsAuto
		{
			get
			{
				return (bool)base.GetValue(FontSizeControl.IsAutoProperty);
			}
			set
			{
				base.SetValue(FontSizeControl.IsAutoProperty, value);
			}
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x0007DFCA File Offset: 0x0007C1CA
		private void PopupBtn_fontSize_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.popupBtn_fontSize.InnerPopup != null)
			{
				this.popupBtn_fontSize.InnerPopup.Opened += this.InnerPopup_Opened;
			}
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x0007DFF5 File Offset: 0x0007C1F5
		private void InnerPopup_Opened(object sender, EventArgs e)
		{
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x0007DFF7 File Offset: 0x0007C1F7
		public IReadOnlyList<double> FontSizeList { get; } = new List<double>
		{
			8.0, 10.0, 12.0, 14.0, 16.0, 18.0, 20.0, 22.0, 24.0, 26.0,
			28.0, 36.0, 48.0, 56.0, 72.0, 96.0
		};

		// Token: 0x06001D26 RID: 7462 RVA: 0x0007DFFF File Offset: 0x0007C1FF
		private void btn_fontSize_item_Click(object sender, RoutedEventArgs e)
		{
			this.IsAuto = false;
			this.SelectedFontSize = (double)(sender as FrameworkElement).DataContext;
			this.popupBtn_fontSize.ClosePopup();
		}

		// Token: 0x06001D27 RID: 7463 RVA: 0x0007E029 File Offset: 0x0007C229
		private void btn_auto_Click(object sender, RoutedEventArgs e)
		{
			this.IsAuto = true;
			this.popupBtn_fontSize.ClosePopup();
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x0007E040 File Offset: 0x0007C240
		private void btn_fontSize_item_Loaded(object sender, RoutedEventArgs e)
		{
			if (!this.IsAuto)
			{
				FrameworkElement frameworkElement = sender as FrameworkElement;
				if ((double)frameworkElement.DataContext == this.SelectedFontSize)
				{
					frameworkElement.BringIntoView();
				}
			}
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x0007E075 File Offset: 0x0007C275
		private void btn_auto_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.IsAuto)
			{
				this.btn_auto.BringIntoView();
			}
		}

		// Token: 0x04000AEF RID: 2799
		public static readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register("SelectedFontSize", typeof(double), typeof(FontSizeControl), new PropertyMetadata(12.0));

		// Token: 0x04000AF0 RID: 2800
		public static readonly DependencyProperty FontSizeUnitProperty = DependencyProperty.Register("FontSizeUnit", typeof(string), typeof(FontSizeControl), new PropertyMetadata(""));

		// Token: 0x04000AF1 RID: 2801
		public static readonly DependencyProperty IsShowAutoProperty = DependencyProperty.Register("IsShowAuto", typeof(bool), typeof(FontSizeControl), new PropertyMetadata(false));

		// Token: 0x04000AF2 RID: 2802
		public static readonly DependencyProperty IsAutoProperty = DependencyProperty.Register("IsAuto", typeof(bool), typeof(FontSizeControl), new PropertyMetadata(false));
	}
}
