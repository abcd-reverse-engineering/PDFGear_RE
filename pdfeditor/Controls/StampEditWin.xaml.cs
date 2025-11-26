using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Common;
using pdfeditor.Properties;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls
{
	// Token: 0x020001DD RID: 477
	public partial class StampEditWin : Window
	{
		// Token: 0x06001AFB RID: 6907 RVA: 0x0006C536 File Offset: 0x0006A736
		public StampEditWin()
		{
			this.InitializeComponent();
			this.stampText = new StampTextModel
			{
				FontColor = "#FF20C48F"
			};
			this.Init();
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x0006C560 File Offset: 0x0006A760
		public StampEditWin(StampTextModel textModel)
		{
			this.InitializeComponent();
			base.Title = pdfeditor.Properties.Resources.WinCustomizeStampEditTiltle;
			this.stampText = textModel;
			this.Init();
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x06001AFD RID: 6909 RVA: 0x0006C586 File Offset: 0x0006A786
		// (set) Token: 0x06001AFE RID: 6910 RVA: 0x0006C58E File Offset: 0x0006A78E
		public IStampTextModel StampTextModel { get; private set; }

		// Token: 0x06001AFF RID: 6911 RVA: 0x0006C598 File Offset: 0x0006A798
		private void Init()
		{
			this.txt_Text.Text = this.stampText.TextContent ?? "";
			this.txt_Text.SelectAll();
			this.txt_Text.Focus();
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				SolidColorBrush solidColorBrush2 = this.colorSelecters.Children.OfType<RadioButton>().FirstOrDefault((RadioButton c) => c.IsChecked.GetValueOrDefault()).Background as SolidColorBrush;
				Color? color2 = ((solidColorBrush2 != null) ? new Color?(solidColorBrush2.Color) : null);
				if (color2 == null)
				{
					color2 = new Color?((Color)ColorConverter.ConvertFromString("#FF20C48F"));
				}
				Color value = color2.Value;
				this.stampText.TextContent = this.txt_Text.Text.Trim();
				this.stampText.FontColor = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { value.A, value.R, value.G, value.B });
				if (string.IsNullOrEmpty(this.stampText.TextContent))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkTextEmptyMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				if (this.stampText.TextContent.Trim().Length > 50)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinCustomizeStampMaxCharactersMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				this.StampTextModel = this.stampText;
				base.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				this.StampTextModel = null;
				base.DialogResult = new bool?(false);
			};
			Color? color = null;
			try
			{
				if (!string.IsNullOrEmpty(this.stampText.FontColor))
				{
					color = new Color?((Color)ColorConverter.ConvertFromString(this.stampText.FontColor));
				}
			}
			catch
			{
			}
			if (color == null)
			{
				color = new Color?((Color)ColorConverter.ConvertFromString("#FF20C48F"));
			}
			IEnumerable<RadioButton> enumerable = from c in this.colorSelecters.Children.OfType<RadioButton>()
				where c != this.CustomColorRadioButton
				select c;
			bool flag = false;
			foreach (RadioButton radioButton in enumerable)
			{
				SolidColorBrush solidColorBrush = radioButton.Background as SolidColorBrush;
				if (solidColorBrush != null && StampEditWin.IsSameColor(solidColorBrush.Color, color.Value))
				{
					radioButton.IsChecked = new bool?(true);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.CustomColorRadioButton.Background = new SolidColorBrush(color.Value);
				this.CustomColorRadioButton.Visibility = Visibility.Visible;
				this.CustomColorRadioButton.IsChecked = new bool?(true);
			}
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x0006C734 File Offset: 0x0006A934
		private static bool IsSameColor(Color left, Color right)
		{
			return Math.Abs((int)(left.A - right.A)) < 2 && Math.Abs((int)(left.R - right.R)) < 2 && Math.Abs((int)(left.G - right.G)) < 2 && Math.Abs((int)(left.B - right.B)) < 2;
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x0006C79F File Offset: 0x0006A99F
		private void txt_Text_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.btnOk.IsEnabled = !string.IsNullOrEmpty(this.txt_Text.Text);
		}

		// Token: 0x0400095E RID: 2398
		private StampTextModel stampText;
	}
}
